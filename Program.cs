using BBMS_WebAPI.Data;
using BBMS_WebAPI.Middleware;
using BBMS_WebAPI.Services;
using BBMS_WebAPI.Utilities;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ------------------------- Configure Services -------------------------

// Add controllers with FluentValidation
builder.Services.AddControllers()
    .AddFluentValidation(c => c.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()));

// Register repositories
builder.Services.AddScoped<DonorRepository>();
builder.Services.AddScoped<RecipientRepository>();
builder.Services.AddScoped<AdminRepository>();
builder.Services.AddScoped<DonationRepository>();

// Register utilities and services
builder.Services.AddScoped<OtpService>();
builder.Services.AddScoped<EmailHelper>();
builder.Services.AddScoped<TokenService>();

// Load DonorMapper mappings
DonorMapper.LoadMapping(builder.Configuration);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Your frontend URL
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.Length < 32)
    throw new Exception("Invalid JWT key. It must be at least 256 bits (32 characters).");

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Configure the database context for EF Core
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ------------------------- Build Application -------------------------

var app = builder.Build();

// ------------------------- Configure Middleware -------------------------

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS middleware
app.UseCors("AllowSpecificOrigin");

// Validate JWT token on every request
app.UseMiddleware<JwtMiddleware>();

app.UseHttpsRedirection();

app.UseDeveloperExceptionPage();

// Add Authentication and Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
