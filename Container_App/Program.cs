using Container_App.Common.Config;
using Container_App.Consumer;
using Container_App.Core.Model.Email;
using Container_App.Data;
using Container_App.Data.Connection;
using Container_App.Data.Repository.Banners;
using Container_App.Data.Repository.DatPhongs;
using Container_App.Data.Repository.Emails;
using Container_App.Data.Repository.KhachSans;
using Container_App.Data.Repository.LoaiPhongs;
using Container_App.Data.Repository.Permissions;
using Container_App.Data.Repository.Phongs;
using Container_App.Data.Repository.Provinces;
using Container_App.Data.Repository.RabbitMQ;
using Container_App.Data.Repository.Redis;
using Container_App.Data.Repository.RefreshTokens;
using Container_App.Data.Repository.RolePermissions;
using Container_App.Data.Repository.TienIchs;
using Container_App.Data.Repository.Users;
using Container_App.Middleware;
using Container_App.Service;
using Container_App.Service.Services.Banners;
using Container_App.Service.Services.Cloudinarys;
using Container_App.Service.Services.DatPhongs;
using Container_App.Service.Services.Emails;
using Container_App.Service.Services.KhachSans;
using Container_App.Service.Services.LoaiPhongs;
using Container_App.Service.Services.Permissions;
using Container_App.Service.Services.Phongs;
using Container_App.Service.Services.Provinces;
using Container_App.Service.Services.RabbitMQ;
using Container_App.Service.Services.Redis;
using Container_App.Service.Services.RefreshTokens;
using Container_App.Service.Services.RolePermissions;
using Container_App.Service.Services.TienIchs;
using Container_App.Service.Services.Users;
using dotenv.net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System;
using System.ComponentModel.Design;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.Configure<MailSettings>(
    builder.Configuration.GetSection("MailSettings"));

builder.Services.AddDIData(builder.Configuration);//DI Data
builder.Services.AddDIService(builder.Configuration);//DI Service
builder.Services.AddHttpContextAccessor();//DI HttpContextAccessor


builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("Redis");

    return ConnectionMultiplexer.Connect(connectionString);
});



//Add consumer
builder.Services.AddHostedService<EmailConsumer>();

var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("JWT Key is missing.");
}

var keyBytes = Convert.FromBase64String(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // true khi production
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),

            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],

            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// Program.cs
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["ConnectionRedis:Redis"];
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Cấu hình ứng dụng lắng nghe HTTP
//builder.WebHost.UseUrls("http://0.0.0.0:5925");

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "My API",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập token theo dạng: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddMemoryCache();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
    });
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers(); // Không cần gọi MapControllers ở đây
});


//Thêm đoạn code này nếu muốn chạy code first
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
//    db.Database.EnsureCreated();   // hoặc db.Database.Migrate();
//}

app.Run();
