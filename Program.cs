using BBMS_WebAPI.Data;
using BBMS_WebAPI.Services;
using BBMS_WebAPI.Utilities;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

/*builder.Services.AddControllers()
    .AddFluentValidation(c => c.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()));*/

builder.Services.AddScoped<DonorRepository>();

// Add CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Your frontend URL
              .AllowAnyHeader()                     // Allow any header
              .AllowAnyMethod()                     // Allow any HTTP method (GET, POST, etc.)
              .AllowCredentials();                  // Allow credentials (cookies, etc.)
    });
});

// Register the database context for MailKit
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionString")));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services and utilities
builder.Services.AddScoped<OtpService>();
builder.Services.AddScoped<EmailHelper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Use CORS middleware
app.UseCors("AllowSpecificOrigin");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
