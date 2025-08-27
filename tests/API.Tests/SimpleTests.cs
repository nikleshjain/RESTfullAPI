using Xunit;
using RESTfullAPI.Application.DTOs;
using RESTfullAPI.Application.Validators;
using FluentValidation.TestHelper;

namespace API.Tests;

public class SimpleTests
{
    [Fact]
    public void CreateProductDtoValidator_ValidData_ShouldPass()
    {
        // Arrange
        var validator = new CreateProductDtoValidator();
        var dto = new CreateProductDto
        {
            ProductName = "Test Product",
            CreatedBy = "Test User"
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateProductDtoValidator_EmptyProductName_ShouldFail()
    {
        // Arrange
        var validator = new CreateProductDtoValidator();
        var dto = new CreateProductDto
        {
            ProductName = "",
            CreatedBy = "Test User"
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductName);
    }

    [Fact]
    public void CreateProductDtoValidator_EmptyCreatedBy_ShouldFail()
    {
        // Arrange
        var validator = new CreateProductDtoValidator();
        var dto = new CreateProductDto
        {
            ProductName = "Test Product",
            CreatedBy = ""
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CreatedBy);
    }

    [Fact]
    public void CreateItemDtoValidator_ValidData_ShouldPass()
    {
        // Arrange
        var validator = new CreateItemDtoValidator();
        var dto = new CreateItemDto
        {
            ProductId = 1,
            Quantity = 10
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateItemDtoValidator_InvalidProductId_ShouldFail()
    {
        // Arrange
        var validator = new CreateItemDtoValidator();
        var dto = new CreateItemDto
        {
            ProductId = 0,
            Quantity = 10
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.ProductId);
    }

    [Fact]
    public void CreateItemDtoValidator_InvalidQuantity_ShouldFail()
    {
        // Arrange
        var validator = new CreateItemDtoValidator();
        var dto = new CreateItemDto
        {
            ProductId = 1,
            Quantity = 0
        };

        // Act
        var result = validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }
}


