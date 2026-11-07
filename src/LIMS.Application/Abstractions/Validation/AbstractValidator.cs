using System.Linq.Expressions;

namespace LIMS.Application.Abstractions.Validation;

public abstract class AbstractValidator<T> : IValidator<T>
{
    private readonly List<RuleDefinition> _rules = [];

    protected RuleBuilder<T, TProperty> RuleFor<TProperty>(
        Expression<Func<T, TProperty>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);

        return new RuleBuilder<T, TProperty>(
            expression.Compile(),
            GetPropertyName(expression),
            AddRule);
    }

    protected RuleBuilder<T, TProperty> RuleForSelector<TProperty>(
        Func<T, TProperty> selector,
        string propertyName = "Property")
    {
        ArgumentNullException.ThrowIfNull(selector);

        return new RuleBuilder<T, TProperty>(
            selector,
            propertyName,
            AddRule);
    }

    private Action<string> AddRule(
        Func<T, bool> predicate,
        string propertyName,
        string defaultMessage)
    {
        var rule = new RuleDefinition(
            predicate,
            propertyName,
            defaultMessage);

        _rules.Add(rule);

        return message => rule.CustomMessage = message;
    }

    public virtual Task<ValidationResult> ValidateAsync(
        T instance,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(instance);

        var result = new ValidationResult();

        foreach (var rule in _rules)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!rule.Predicate(instance))
            {
                result.AddError(
                    rule.PropertyName,
                    rule.CustomMessage ?? rule.DefaultMessage);
            }
        }

        return Task.FromResult(result);
    }

    private static string GetPropertyName<TProperty>(
        Expression<Func<T, TProperty>> expression)
    {
        Expression body = expression.Body;

        while (body is UnaryExpression unary &&
               (body.NodeType == ExpressionType.Convert ||
                body.NodeType == ExpressionType.ConvertChecked))
        {
            body = unary.Operand;
        }

        return body is MemberExpression member
            ? member.Member.Name
            : typeof(T).Name;
    }

    private sealed class RuleDefinition(
        Func<T, bool> predicate,
        string propertyName,
        string defaultMessage)
    {
        public Func<T, bool> Predicate { get; } = predicate;
        public string PropertyName { get; } = propertyName;
        public string DefaultMessage { get; } = defaultMessage;
        public string? CustomMessage { get; set; }
    }
}
