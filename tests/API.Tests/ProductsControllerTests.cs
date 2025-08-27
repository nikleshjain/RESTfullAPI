using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RESTfullAPI.Application.DTOs;
using RESTfullAPI.Infrastructure.Data;
using System.Net;
using System.Text;
using System.Text.Json;
using Xunit;

namespace API.Tests;

public class ProductsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public ProductsControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseContentRoot(Directory.GetCurrentDirectory());
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetProducts_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }

    [Fact]
    public async Task CreateProduct_ReturnsCreatedStatusCode()
    {
        // Arrange
        var createProductDto = new CreateProductDto
        {
            ProductName = "Test Product",
            CreatedBy = "Test User"
        };

        var json = JsonSerializer.Serialize(createProductDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/v1/products", content);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetProduct_WithValidId_ReturnsProduct()
    {
        // Arrange
        var createProductDto = new CreateProductDto
        {
            ProductName = "Test Product",
            CreatedBy = "Test User"
        };

        var json = JsonSerializer.Serialize(createProductDto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var createResponse = await _client.PostAsync("/api/v1/products", content);
        var createdProduct = JsonSerializer.Deserialize<ProductDto>(
            await createResponse.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        // Act
        var response = await _client.GetAsync($"/api/v1/products/{createdProduct!.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var product = JsonSerializer.Deserialize<ProductDto>(
            await response.Content.ReadAsStringAsync(),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        
        Assert.Equal(createProductDto.ProductName, product!.ProductName);
    }

    [Fact]
    public async Task GetProduct_WithInvalidId_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/v1/products/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
