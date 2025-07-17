using FluentValidation;

namespace SearchService.Application.Queries.SearchSuggestions;

public class GetSearchSuggestionsQueryValidator : AbstractValidator<GetSearchSuggestionsQuery>
{
    public GetSearchSuggestionsQueryValidator()
    {
        RuleFor(x => x.Query)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Query is required and must not exceed 200 characters");

        RuleFor(x => x.MaxSuggestions)
            .InclusiveBetween(1, 50)
            .WithMessage("Max suggestions must be between 1 and 50");
    }
}
