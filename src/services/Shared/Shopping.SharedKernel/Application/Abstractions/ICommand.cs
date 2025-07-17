using MediatR;

namespace Shopping.SharedKernel.Application.Abstractions;


  public interface ICommand : IRequest
  {
      Guid Id { get; }
  }

  public interface ICommand<out TResult> : IRequest<TResult>
  {
      Guid Id { get; }
  }