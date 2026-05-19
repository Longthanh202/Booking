using Container_App.Common.Config;
using Container_App.Core.Interface.Banners;
using Container_App.Core.Interface.KhachSans;
using Container_App.Core.Interface.LoaiPhongs;
using Container_App.Core.Interface.Permissions;
using Container_App.Core.Interface.Phongs;
using Container_App.Core.Interface.RefreshTokens;
using Container_App.Core.Interface.RolePermissions;
using Container_App.Core.Interface.TienIchs;
using Container_App.Core.Interface.Users;
using Container_App.Data.Connection;
using Container_App.Middleware;
using Container_App.Service.Services.Banners;
using Container_App.Service.Services.Cloudinarys;
using Container_App.Service.Services.KhachSans;
using Container_App.Service.Services.LoaiPhongs;
using Container_App.Service.Services.Permissions;
using Container_App.Service.Services.Phongs;
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

#region Connection String
#endregion

builder.Services.AddScoped<IStoredProcedureExecutor, StoredProcedureExecutor>();

#region Add Service
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IRolePermissionService, RolePermissionService>();
builder.Services.AddScoped<IKhachSanService, KhachSanService>();
builder.Services.AddScoped<ITienIchService, TienIchService>();
builder.Services.AddScoped<ILoaiPhongService, LoaiPhongService>();
builder.Services.AddScoped<IPhongService, PhongService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
builder.Services.AddScoped<CloudinaryService>();
builder.Services.AddScoped<IBannerService, BannerService>();
builder.Services.AddScoped<IKhachSanImageService, KhachSanImageService>();
#endregion

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
builder.WebHost.UseUrls("http://0.0.0.0:5925");

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

//app.UseHttpsRedirection(); // Đặt trước UseRouting // bỏ chạy http thâu
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
