using BuildingBlocks.Messaging.Events.Base;

namespace BuildingBlocks.Messaging.Events.Domains
{
    public record UserLoginEvent : IntegrationEvent
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime LoginTime { get; set; }
        public bool IsSuccessful { get; set; }
    }

    public record UserLogoutEvent : IntegrationEvent
    {
        public int UserId { get; set; }
        public DateTime LogoutTime { get; set; }
    }
}