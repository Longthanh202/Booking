using Container_App.Common.Config;
using Container_App.Core.Model.Email;
using Container_App.Data;
using Container_App.Middleware;
using Container_App.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Container_App.Service.Services.Hubs;

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

builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Tự động bỏ qua các trường có giá trị NULL khi Serialize ra JSON
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

// Đọc 1 lần, "đóng băng" giá trị ngay tại startup — không bị ảnh hưởng bởi reload sau này
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtIssuer = jwtSection["Issuer"];
var jwtAudience = jwtSection["Audience"];
var jwtKey = jwtSection["Key"];

if (string.IsNullOrWhiteSpace(jwtIssuer) || string.IsNullOrWhiteSpace(jwtKey))
{
    throw new Exception("LỖI: Thiếu Jwt:Issuer hoặc Jwt:Key trong appsettings.json lúc khởi động.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,           // Giá trị cố định, không đổi theo reload
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"[JWT Fail]: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();
                Console.WriteLine($"[Debug] Authorization: '{authHeader}'");
                return Task.CompletedTask;
            }
        };
    });



// Program.cs
// builder.Services.AddStackExchangeRedisCache(options =>
// {
//     options.Configuration = builder.Configuration["ConnectionRedis:Redis"];
// });


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});


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
builder.Services.AddSignalR();


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
app.MapHub<BookingHub>("/bookingHub");

//Thêm đoạn code này nếu muốn chạy code first
//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
//    db.Database.EnsureCreated();   // hoặc db.Database.Migrate();
//}

app.Run();
