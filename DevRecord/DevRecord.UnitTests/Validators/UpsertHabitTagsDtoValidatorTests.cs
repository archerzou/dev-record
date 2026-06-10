using DevRecord.Api.DTOs.HabitTags;
using DevRecord.Api.Entities;
using FluentValidation.TestHelper;

namespace DevRecord.UnitTests.Validators;

public sealed class UpsertHabitTagsDtoValidatorTests
{
    private readonly UpsertHabitTagsDtoValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldNotReturnError_WhenTagIdsAreValid()
    {
        // Arrange
        var dto = new UpsertHabitTagsDto
        {
            TagIds = [Tag.NewId(), Tag.NewId()]
        };

        // Act
        TestValidationResult<UpsertHabitTagsDto>? result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenTagIdsAreEmpty()
    {
        // Arrange
        var dto = new UpsertHabitTagsDto
        {
            TagIds = []
        };

        // Act
        TestValidationResult<UpsertHabitTagsDto>? result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenTagIdsContainDuplicates()
    {
        // Arrange
        string tagId = Tag.NewId();
        var dto = new UpsertHabitTagsDto
        {
            TagIds = [tagId, tagId]
        };

        // Act
        TestValidationResult<UpsertHabitTagsDto>? result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TagIds);
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenTagIdHasInvalidFormat()
    {
        // Arrange
        var dto = new UpsertHabitTagsDto
        {
            TagIds = ["invalid-id"]
        };

        // Act
        TestValidationResult<UpsertHabitTagsDto>? result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor("TagIds[0]");
    }

    [Fact]
    public async Task Validate_ShouldNotReturnError_WhenTagIdsContainSingleItem()
    {
        // Arrange
        var dto = new UpsertHabitTagsDto
        {
            TagIds = [Tag.NewId()]
        };

        // Act
        TestValidationResult<UpsertHabitTagsDto>? result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }
}
