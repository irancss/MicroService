using MassTransit;
using BuildingBlocks.Messaging.Events.Contracts;
using BuildingBlocks.Messaging.Events.Domains;

namespace BuildingBlocks.Messaging.EventDriven.Sagas
{
    // State data for the customer onboarding process
    public class CustomerOnboardingState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime? FirstLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // The state machine definition
    public class CustomerOnboardingStateMachine : MassTransitStateMachine<CustomerOnboardingState>
    {
        // States
        public State AwaitingFirstLogin { get; private set; } = null!;

        // Events
        public Event<CustomerRegisteredEvent> CustomerRegistered { get; private set; } = null!;
        public Event<UserLoginEvent> UserLoggedIn { get; private set; } = null!;

        public CustomerOnboardingStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => CustomerRegistered, x => x.CorrelateBy((saga, context) => saga.CustomerId == context.Message.CustomerId).SelectId(ctx => NewId.NextGuid()));
            Event(() => UserLoggedIn, x => x.CorrelateBy((saga, context) => saga.Email == context.Message.Email));

            Initially(
                When(CustomerRegistered)
                    .Then(ctx =>
                    {
                        ctx.Saga.CustomerId = ctx.Message.CustomerId;
                        ctx.Saga.Email = ctx.Message.Email;
                        ctx.Saga.FullName = ctx.Message.FullName;
                        ctx.Saga.CreatedAt = DateTime.UtcNow;
                    })
                    .Publish(ctx => new NotificationRequestEvent(
                        ctx.Message.Email,
                        "Welcome!",
                        $"Hello {ctx.Message.FullName}, welcome to our platform!"
                    ))
                    .TransitionTo(AwaitingFirstLogin)
            );

            During(AwaitingFirstLogin,
                When(UserLoggedIn, ctx => ctx.Message.IsSuccessful && ctx.Saga.FirstLoginAt == null)
                    .Then(ctx => ctx.Saga.FirstLoginAt = ctx.Message.LoginTime)
                    .Publish(ctx => new NotificationRequestEvent(
                        ctx.Saga.Email,
                        "Getting Started Guide",
                        "Here are some tips to get started..."
                    ))
                    .Finalize() // Saga ends here
            );

            SetCompletedWhenFinalized();
        }
    }
}