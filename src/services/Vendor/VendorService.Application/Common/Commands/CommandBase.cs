using VendorService.Application.Common.Commands;

namespace VendorService.application.Common.Commands;
  public class CommandBase
  {
      public Guid Id { get; }

      public CommandBase()
      {
          this.Id = Guid.NewGuid();
      }

      protected CommandBase(Guid id)
      {
          Id = id;
      }
  }

  public abstract class CommandBase<TResult> : ICommand<TResult>
  {
      public Guid Id { get; }

      protected CommandBase()
      {
          this.Id = Guid.NewGuid();
      }

      protected CommandBase(Guid id)
      {
          Id = id;
      }
  }