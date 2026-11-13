using LIMS.Application.Abstractions.Messaging;
using LIMS.Application.Abstractions.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace LIMS.Application.Behaviors;

public sealed class Dispatcher : IDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IValidationService _validationService;

    public Dispatcher(
        IServiceProvider serviceProvider,
        IValidationService validationService)
    {
        _serviceProvider = serviceProvider;
        _validationService = validationService;
    }

    public async Task<TResult> SendAsync<TCommand, TResult>(
        TCommand command,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand<TResult>
    {
        ArgumentNullException.ThrowIfNull(command);

        await _validationService.ValidateAsync(
            command,
            cancellationToken);

        var handler = _serviceProvider.GetRequiredService<
            ICommandHandler<TCommand, TResult>>();

        return await handler.HandleAsync(
            command,
            cancellationToken);
    }

    public async Task<TResult> QueryAsync<TQuery, TResult>(
        TQuery query,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<TResult>
    {
        ArgumentNullException.ThrowIfNull(query);

        await _validationService.ValidateAsync(
            query,
            cancellationToken);

        var handler = _serviceProvider.GetRequiredService<
            IQueryHandler<TQuery, TResult>>();

        return await handler.HandleAsync(
            query,
            cancellationToken);
    }
}
