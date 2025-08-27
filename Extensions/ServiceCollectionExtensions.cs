using Microsoft.EntityFrameworkCore;
using RESTfullAPI.Application.Interfaces;
using RESTfullAPI.Application.Mapping;
using RESTfullAPI.Application.Services;
using RESTfullAPI.Application.Validators;
using RESTfullAPI.Infrastructure.Data;
using RESTfullAPI.Infrastructure.Data.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using RESTfullAPI.Infrastructure.Identity;

namespace RESTfullAPI.Extensions;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services)
	{
		// AutoMapper
		services.AddAutoMapper(typeof(MappingProfile));
		
		// FluentValidation
		services.AddValidatorsFromAssemblyContaining<CreateProductDtoValidator>();
		
		// Services
		services.AddScoped<IProductService, ProductService>();
		services.AddScoped<IItemService, ItemService>();
		services.AddScoped<IAuthService, AuthService>();
		
		return services;
	}
	
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
	{
		// Database
		services.AddDbContext<ApplicationDbContext>(options =>
			options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
		
		// Repositories
		services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
		services.AddScoped<IProductRepository, ProductRepository>();
		services.AddScoped<IItemRepository, ItemRepository>();
		
		// Identity services
		services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
		services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
		services.AddSingleton<IRefreshTokenStore, InMemoryRefreshTokenStore>();
		
		// Bind demo users
		var demoUsers = configuration.GetSection("DemoUsers").Get<DemoUser[]>() ?? Array.Empty<DemoUser>();
		services.AddSingleton<IEnumerable<DemoUser>>(demoUsers);
		
		return services;
	}
	
	public static IServiceCollection AddApiServices(this IServiceCollection services)
	{
		// API Versioning
		services.AddApiVersioning(options =>
		{
			options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
			options.AssumeDefaultVersionWhenUnspecified = true;
			options.ReportApiVersions = true;
		});
		
		services.AddVersionedApiExplorer(options =>
		{
			options.GroupNameFormat = "'v'VVV";
			options.SubstituteApiVersionInUrl = true;
		});
		
		// Swagger
		services.AddSwaggerGen(c =>
		{
			c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
			{
				Title = "RESTful API",
				Version = "v1",
				Description = "A RESTful API for Product and Item management"
			});
			
			c.EnableAnnotations();
		});
		
		// Authentication
		services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidateLifetime = true,
					ValidateIssuerSigningKey = true,
					ClockSkew = TimeSpan.FromMinutes(1),
					ValidIssuer = services.BuildServiceProvider().GetRequiredService<IConfiguration>().GetValue<string>("Jwt:Issuer"),
					ValidAudience = services.BuildServiceProvider().GetRequiredService<IConfiguration>().GetValue<string>("Jwt:Audience"),
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
						services.BuildServiceProvider().GetRequiredService<IConfiguration>().GetValue<string>("Jwt:Key")!))
				};
			});
		
		// Response Compression
		services.AddResponseCompression();
		
		return services;
	}
}
