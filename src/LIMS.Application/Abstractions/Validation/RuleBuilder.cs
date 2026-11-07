using System.Text.RegularExpressions;

namespace LIMS.Application.Abstractions.Validation;

public sealed class RuleBuilder<T, TProperty>
{
    private readonly Func<T, TProperty> _selector;
    private readonly string _propertyName;

    private readonly Func<
        Func<T, bool>,
        string,
        string,
        Action<string>> _addRule;

    private Action<string>? _lastRuleMessageSetter;

    internal RuleBuilder(
        Func<T, TProperty> selector,
        string propertyName,
        Func<Func<T, bool>, string, string, Action<string>> addRule)
    {
        _selector = selector;
        _propertyName = propertyName;
        _addRule = addRule;
    }

    public RuleBuilder<T, TProperty> NotEmpty()
    {
        Add(
            instance => !IsEmpty(_selector(instance)),
            "The value must not be empty.");

        return this;
    }

    public RuleBuilder<T, TProperty> MaximumLength(int maximumLength)
    {
        if (maximumLength < 0)
            throw new ArgumentOutOfRangeException(nameof(maximumLength));

        Add(
            instance =>
            {
                var value = _selector(instance);

                return value is not string text ||
                       text.Length <= maximumLength;
            },
            $"The length must be {maximumLength} characters or fewer.");

        return this;
    }

    public RuleBuilder<T, TProperty> Matches(string pattern)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(pattern);

        var regex = new Regex(
            pattern,
            RegexOptions.Compiled | RegexOptions.CultureInvariant,
            TimeSpan.FromSeconds(2));

        Add(
            instance =>
            {
                var value = _selector(instance);

                return value is not string text ||
                       regex.IsMatch(text);
            },
            "The value has an invalid format.");

        return this;
    }

    public RuleBuilder<T, TProperty> Must(
        Predicate<TProperty> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        Add(
            instance => predicate(_selector(instance)),
            "The value is invalid.");

        return this;
    }

    public RuleBuilder<T, TProperty> Must(
        Func<T, TProperty, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        Add(
            instance => predicate(instance, _selector(instance)),
            "The value is invalid.");

        return this;
    }

    public RuleBuilder<T, TProperty> WithMessage(string message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        if (_lastRuleMessageSetter is null)
            throw new InvalidOperationException(
                "WithMessage must follow a validation rule.");

        _lastRuleMessageSetter(message);

        return this;
    }

    private void Add(
        Func<T, bool> predicate,
        string defaultMessage)
    {
        _lastRuleMessageSetter = _addRule(
            predicate,
            _propertyName,
            defaultMessage);
    }

    private static bool IsEmpty(object? value)
    {
        return value switch
        {
            null => true,
            string text => string.IsNullOrWhiteSpace(text),
            Guid id => id == Guid.Empty,
            _ => false
        };
    }
}
