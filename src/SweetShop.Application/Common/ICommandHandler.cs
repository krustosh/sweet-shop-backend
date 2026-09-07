namespace SweetShop.Application.Common;

/// <summary>
/// Handles an application command.
/// </summary>
/// <typeparam name="TCommand">The command type.</typeparam>
/// <typeparam name="TResponse">The command response type.</typeparam>
public interface ICommandHandler<in TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
    /// <summary>
    /// Handles the specified command asynchronously.
    /// </summary>
    /// <param name="command">The command to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The command result.</returns>
    Task<TResponse> HandleAsync(
        TCommand command,
        CancellationToken cancellationToken = default);
}