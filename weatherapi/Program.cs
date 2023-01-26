global using System.Text;
global using Microsoft.AspNetCore.WebUtilities;
global using System.Text.Encodings.Web;

using weatherapi.Services.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using weatherapi.Core;
using weatherapi.Entities;
using weatherapi.Core.Serrvices.Generic;
using weatherapi.Entities.Declarations.Generic;

var builder = WebApplication.CreateBuilder(args);

//----------------------------------------------------------------------------------------------------
// Add services to the container.
//----------------------------------------------------------------------------------------------------


//Set tup Database
builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    .EnableSensitiveDataLogging();
});



//Set up Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireUppercase = false;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 6;

    options.User.RequireUniqueEmail = true;

    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
    options.SignIn.RequireConfirmedAccount = false;

})
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();


//Register JWT Authentication
builder.Services.AddSingleton<IJwtAuthentication, JwtAuthentication>();


//Set up Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})


//Set up Jwt Bearer
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ClockSkew = TimeSpan.Zero,

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:SecretKey"])),
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"]
    };
});



//Add Cotrollers
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
                     options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);


//Set up swagger
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Weather API",
        Description = "An ASP.NET Core 6 Weather Web API with authentication and authorization",
        TermsOfService = new Uri("https://techiestephen.github.io/"),
        Contact = new OpenApiContact
        {
            Name = "Ohien Stephen",
            Url = new Uri("https://techiestephen.github.io/t")
        },
        License = new OpenApiLicense
        {
            Name = "License",
            Url = new Uri("https://techiestephen.github.io/")
        }
        
        
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorizaion",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

//Register AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

//Register RepositoryWrapper
builder.Services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();


//----------------------------------------------------------------------------------------------------
// Configure the HTTP request pipeline.
//----------------------------------------------------------------------------------------------------

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

