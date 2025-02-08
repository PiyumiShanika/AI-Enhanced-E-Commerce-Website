using EcomApi.Application.Custom_Midleware;
using EcomApi.Application.Interfaces;
using EcomApi.Application.Services;
using EcomApi.Infrastructure._3P_Services;
using EcomApi.Infrastructure.Config;
using EcomApi.Infrastructure.Database;
using EcomApi.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
.AddJsonOptions(options =>
 {
     options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles; //ignore cycle

     //remove null
     options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;

 });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });



    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

builder.Services.Configure<KeyConfig>(options =>
{
    options.clientId = builder.Configuration["Auth0:clientId"];
    options.auth0Domain = builder.Configuration["Auth0:Domain"];
    options.clientSecret = builder.Configuration["Auth0:clientSecret"];
    options.audience = builder.Configuration["Auth0:Audience"];
    options.StripeSecretKey = builder.Configuration["Stripe:SecretKey"];
    options.SendGridapiKey = builder.Configuration["SendGrid:apiKey"];
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
      options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}/";
      options.Audience = builder.Configuration["Auth0:Audience"];
      options.TokenValidationParameters = new TokenValidationParameters
      {
          NameClaimType = ClaimTypes.NameIdentifier
      };
  });

//builder.Services.AddSwaggerGen(option =>
//{
//    option.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
//});

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("ApiLogs/logs-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

//dependency injection
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAddressService,AddressService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthServices, AuthServices>();
builder.Services.AddScoped<IStripeServices, StripeServices>();
builder.Services.AddScoped<IEmailServices, EmailServices>();
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<ITransaction, Transaction>();

//stripe config
Stripe.StripeConfiguration.ApiKey = "sk_test_51PBCroRuyftYBncd43fIFIZVBg19RbEDndRpRoU24eUpUjOTJSbD4oS79hD6Ouqls26gdGxB31n2Z8OibHph52Yz00HK9gt1Lv";

//db
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnStr"));
});

builder.Services.AddHttpClient();

var app = builder.Build();


//cores blazor
app.UseCors(c => c.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

//allow headers
app.UseCors(c =>
c.AllowAnyHeader()
.AllowAnyMethod()
.SetIsOriginAllowed(origin => true)
.AllowCredentials());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<GlobalCustomMiddleware>();

app.MapControllers();

app.Run();
