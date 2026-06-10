using FluentValidation;

namespace DevRecord.Api.DTOs.HabitTags;

public sealed class UpsertHabitTagsDtoValidator : AbstractValidator<UpsertHabitTagsDto>
{
    public UpsertHabitTagsDtoValidator()
    {
        RuleFor(x => x.TagIds)
            .NotEmpty()
            .WithMessage("At least one tag ID is required.")
            .Must(x => x.Count == x.Distinct().Count())
            .When(x => x.TagIds.Count > 0)
            .WithMessage("Duplicate tag IDs are not allowed.");

        RuleForEach(x => x.TagIds)
            .NotEmpty()
            .Must(x => x.StartsWith("t_", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Invalid tag ID format.");
    }
}
