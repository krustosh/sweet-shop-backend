using FluentValidation.TestHelper;
using SweetShop.Application.Features.Categories.Requests;
using SweetShop.Application.Features.Categories.Validators;

namespace SweetShop.Application.Tests.Features.Categories.Validators;

/// <summary>
/// Tests validation rules for category creation requests.
/// </summary>
public sealed class CreateCategoryRequestValidatorTests
{
    private readonly CreateCategoryRequestValidator validator = new();

    /// <summary>
    /// Tests that the validator rejects requests with a missing name.
    /// </summary>

    [Fact]
    public void ShouldRejectMissingName()
    {
        var request = new CreateCategoryRequest(
            string.Empty,
            null,
            null,
            0);

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(request => request.Name);
    }

    /// <summary>
    /// Tests that the validator rejects requests with a name longer than 200 characters.
    /// </summary>
    [Fact]
    public void ShouldRejectNameLongerThan200Characters()
    {
        var request = new CreateCategoryRequest(
            new string('A', 201),
            null,
            null,
            0);

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(request => request.Name);
    }

    /// <summary>
    /// Tests that the validator rejects requests with a description longer than 1000 characters.
    /// </summary>
    [Fact]
    public void ShouldRejectDescriptionLongerThan1000Characters()
    {
        var request = new CreateCategoryRequest(
            "Kaju Sweets",
            new string('A', 1001),
            null,
            0);

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(request => request.Description);
    }

    /// <summary>
    /// Tests that the validator rejects requests with an image URL longer than 2048 characters.
    /// </summary>
    [Fact]
    public void ShouldRejectNegativeDisplayOrder()
    {
        var request = new CreateCategoryRequest(
            "Kaju Sweets",
            null,
            null,
            -1);

        var result = validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(request => request.DisplayOrder);
    }

    /// <summary>
    /// Tests that the validator accepts a valid request.
    /// </summary>
    [Fact]
    public void ShouldAcceptValidRequest()
    {
        var request = new CreateCategoryRequest(
            "Kaju Sweets",
            "Traditional kaju sweets.",
            "https://example.com/kaju.jpg",
            1);

        var result = validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}