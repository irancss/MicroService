namespace BuildingBlocks.CrossCutting.Correlation
{
    public interface ICorrelationIdProvider { string CorrelationId { get; } }

    public class CorrelationIdProvider : ICorrelationIdProvider
    {
        public string CorrelationId { get; }
    }
}
