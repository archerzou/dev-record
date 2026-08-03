using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevRecord.Api.DTOs.Tags;
using FluentValidation.TestHelper;

namespace DevRecord.UnitTests.Validators;
public sealed class CreateTagDtoValidatorTests
{
    private readonly CreateTagDtoValidator _validator = new();

    [Fact]
    public async Task Validate_ShouldNotReturnError_WhenNameIsValid()
    {
        // Arrange
        var dto = new CreateTagDto
        {
            Name = "Work"
        };

        // Act
        TestValidationResult<CreateTagDto>? result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task Validate_ShouldReturnError_WhenNameIsInvalid(string? name)
    {
        // Arrange
        var dto = new CreateTagDto
        {
            Name = name!
        };

        // Act
        TestValidationResult<CreateTagDto>? result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var dto = new CreateTagDto
        {
            Name = new string('a', 51)
        };

        // Act
        TestValidationResult<CreateTagDto>? result = await _validator.TestValidateAsync(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenNameIsTooShort()
    {
        var dto = new CreateTagDto { Name = "ab" };
        TestValidationResult<CreateTagDto>? result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Validate_ShouldNotReturnError_WhenDescriptionIsNull()
    {
        var dto = new CreateTagDto { Name = "Work", Description = null };
        TestValidationResult<CreateTagDto>? result = await _validator.TestValidateAsync(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Validate_ShouldReturnError_WhenDescriptionExceedsMaxLength()
    {
        var dto = new CreateTagDto { Name = "Work", Description = new string('a', 501) };
        TestValidationResult<CreateTagDto>? result = await _validator.TestValidateAsync(dto);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
