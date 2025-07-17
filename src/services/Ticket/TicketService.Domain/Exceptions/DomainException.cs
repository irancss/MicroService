

    public abstract class DomainException : Exception
    {
        protected DomainException(string message) : base(message)
        {
        }

        protected DomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public class TicketNotFoundException : DomainException
    {
        public TicketNotFoundException(string message) : base(message)
        {
        }

        public TicketNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }