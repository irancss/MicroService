using BuildingBlocks.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Application.Behaviors;

/// <summary>
/// یک MediatR Pipeline Behavior که اجرای هر ICommand را در یک تراکنش دیتابیس قرار می‌دهد.
/// این Behavior باید بعد از ValidationBehavior در Pipeline قرار گیرد.
/// </summary>
public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly DbContext _dbContext;

    // نکته: ما DbContext را مستقیما تزریق می‌کنیم تا بتوانیم تراکنش را مدیریت کنیم.
    // IUnitOfWork فقط برای SaveChanges استفاده می‌شود.
    public TransactionBehavior(
        IUnitOfWork unitOfWork,
        DbContext dbContext, // این DbContext از طریق ثبت در DI برای هر میکروسرویس resolve می‌شود.
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // فقط ICommand ها را در تراکنش قرار می‌دهیم، نه IQuery ها.
        if (request is not Abstractions.ICommand)
        {
            return await next();
        }

        var typeName = request.GetType().Name;
        _logger.LogInformation("----- Begin transaction for {CommandName}", typeName);

        // اگر از قبل تراکنش فعالی وجود داشته باشد (مثلا در یک سناریوی پیچیده‌تر)، در آن شرکت می‌کنیم.
        if (_dbContext.Database.CurrentTransaction != null)
        {
            return await next();
        }

        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var response = await next();

                // SaveChangesAsync را از طریق UnitOfWork فراخوانی می‌کنیم تا Interceptor ها و Domain Event ها اجرا شوند.
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("----- Commit transaction for {CommandName}", typeName);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Handling transaction for {CommandName} ({Command})", typeName, request);
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }
}