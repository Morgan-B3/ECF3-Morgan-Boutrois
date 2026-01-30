using BookHub.ApiGateway;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorClient", policy =>
    {
        policy
        .WithOrigins(
        "http://localhost:8080" // Docker (frontend)
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
builder.Services.AddAuthorization();

// Injection de GatewayHttpClient pour chaque service
var catalogUrl = builder.Configuration["Services:CatalogService"];
var userUrl = builder.Configuration["Services:UserService"];
var loanUrl = builder.Configuration["Services:LoanService"];


builder.Services.AddSingleton(new GatewayHttpClient(catalogUrl, builder.Services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>().CreateClient()));
builder.Services.AddSingleton(new GatewayHttpClient(userUrl, builder.Services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>().CreateClient()));
builder.Services.AddSingleton(new GatewayHttpClient(loanUrl, builder.Services.BuildServiceProvider().GetRequiredService<IHttpClientFactory>().CreateClient()));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("BlazorClient");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();