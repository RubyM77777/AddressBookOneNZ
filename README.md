ADDRESS BOOK:

Contact.cs
- First name
- Last name
- Phone number
- Email


Group.cs
- Name
Contact contact

AddGroup / UpdateGroup
AddContact / UpdateContact
Get GroupById / ListOfGroups
Get ContactById / ListOfContacts


Added Packages, EFCore, SQLite, Moq
Added Controllers, Models, Repositories, Services, Interfaces, SQLiteDBContext folders & files



Also include the ability to add a contact to a “group”. Where a contact can belong to
multiple groups; each group just has a name. (many - many)
The app should implement the following:
 An endpoint/API call to add/update a new group
 An endpoint/API call to add/update a new contact
 An endpoint/API call to get a list of groups and an individual group
 An endpoint/API call to get a list of contacts and an individual contact
 Support pagination
 The endpoints should be secured by OAuth client credential flow
You can use SQLite as the db,

cd C:\
dir
dotnet ef migrations add InitialCreate
dotnet ef database update

ef migrations remove

SQLLite:

select * from Contacts

select * from Groups

select * from ContactGroup

https://localhost:****/

https://localhost:****/connect/token


----------------

program.cs  - OAuth client creds

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

// IdentityServer in-memory (for local testing)
builder.Services.AddIdentityServer()
    .AddInMemoryApiScopes(new[] { new ApiScope("addressbook_api", "Address Book API") })
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
        options.Authority = "https://localhost:7025"; // same as your app URL
        options.Audience = "addressbook_api";         // must match ApiScope
        options.RequireHttpsMetadata = false;        // for local dev
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

// Middleware pipeline
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
    c.OAuthUsePkce(); // optional
});

// Authentication / Authorization
app.UseAuthentication();
app.UseAuthorization();

// Global exception middleware
app.UseMiddleware<GlobalException>();

app.MapControllers();

app.Run();



------------------------
program.cs - without oAuth

using AddressBookOneNZ.Middleware;
using AddressBookOneNZ.Repositories;
using AddressBookOneNZ.Repositories.Interfaces;
using AddressBookOneNZ.Services;
using AddressBookOneNZ.Services.Interfaces;
using AddressBookOneNZ.SQLiteDbContext;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register the services and repositories in DI container.
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IGroupRepository, GroupRepository>();
builder.Services.AddScoped<IContactService, ContactService>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();

// Register the DbContext with SQLite provider.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use custom global exception handling middleware.
app.UseMiddleware<GlobalException>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();






