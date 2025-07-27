using Ardalis.GuardClauses;

namespace BuildingBlocks.Domain.ValueObjects
{
    public static class GuardClauseExtensions
    {
        public static string MinLength(this IGuardClause guardClause, string input, int minLength, string parameterName)
        {
            if (input.Length < minLength)
            {
                throw new ArgumentException($"'{parameterName}' must be at least {minLength} characters long.");
            }

            return input;
        }

        public static string MaxLength(this IGuardClause guardClause, string input, int maxLength, string parameterName)
        {
            if (input.Length > maxLength)
            {
                throw new ArgumentException($"'{parameterName}' must be at most {maxLength} characters long.");
            }

            return input;
        }
    }
}
