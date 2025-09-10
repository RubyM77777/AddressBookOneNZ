using AddressBookOneNZ.Middleware;
using AddressBookOneNZ.Repositories;
using AddressBookOneNZ.Repositories.Interfaces;
using AddressBookOneNZ.Services;
using AddressBookOneNZ.Services.Interfaces;
using AddressBookOneNZ.SQLiteDbContext;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register services & repositories
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();

// IdentityServer in-memory - Added APIResources & it fixed 'error="invalid_token",error_description="The audience 'empty' is invalid"'
builder.Services.AddIdentityServer()
    .AddInMemoryApiScopes(new[]
    {
        new ApiScope("addressbook_api", "Address Book API")
    })
    .AddInMemoryApiResources(new[]
    {
        new ApiResource("addressbook_api", "Address Book API")
        {
            Scopes = { "addressbook_api" }
        }
    })
    .AddInMemoryClients(new[]
    {
        new Client
        {
            ClientId = "swagger_client",
            AllowedGrantTypes = GrantTypes.ClientCredentials,
            ClientSecrets = { new Secret("swagger_secret".Sha256()) },
            AllowedScopes = { "addressbook_api" }
        }
    })
    .AddDeveloperSigningCredential();

// JWT Bearer Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:7025"; // same as app URL
        options.Audience = "addressbook_api";
        options.RequireHttpsMetadata = false;
    });

builder.Services.AddAuthorization();

// Swagger + OAuth2
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "AddressBook API", Version = "v1" });

    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            ClientCredentials = new OpenApiOAuthFlow
            {
                TokenUrl = new Uri("https://localhost:7025/connect/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "addressbook_api", "Access AddressBook API" }
                }
            }
        }
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new[] { "addressbook_api" }
        }
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

// IdentityServer
app.UseIdentityServer();

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AddressBookOneNZ v1");
    c.OAuthClientId("swagger_client");
    c.OAuthClientSecret("swagger_secret");
    c.OAuthUsePkce();
});

// Authentication / Authorization
app.UseAuthentication();
app.UseAuthorization();

// Global exception middleware
app.UseMiddleware<GlobalException>();

app.MapControllers();

app.Run();
