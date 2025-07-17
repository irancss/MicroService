using MediatR;
using Shopping.SharedKernel.Application.Abstractions;

namespace Shopping.SharedKernel.Application.CQRS.Commands;


  public interface ICommandHandler<in TCommand, TResult> :
      IRequestHandler<TCommand, TResult> where TCommand : ICommand<TResult>
  {
  }

  public interface ICommandHandler<in TCommand> :
      IRequestHandler<TCommand> where TCommand : ICommand
  {
  }