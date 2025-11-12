using EnglishCenterManagement_BackEnd.Models;
using EnglishCenterManagement_BackEnd.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace EnglishCenterManagement_BackEnd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Đăng ký Cookie Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "auth_cookie";
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.None;

                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                options.SlidingExpiration = true;

                options.LoginPath = "/api/Auth/login";
                options.AccessDeniedPath = "/api/Auth/accessdenied";

                // ✅ Thêm đoạn này ngay trong AddCookie
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.CompletedTask;
                };
            });


            // 2. Bật CORS và cho phép gửi Cookie (Credentials)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOriginsWithCredentials", policy =>
                {
                    policy
                        // Cần liệt kê rõ các domain frontend của bạn
                        .WithOrigins(
                            "http://localhost:3000",
                            "http://localhost:5173"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials(); // BẮT BUỘC để gửi Cookie
                });
            });

            // Thêm các dịch vụ cơ bản
            builder.Services.AddAuthorization();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Thêm DbContext
            builder.Services.AddDbContext<EnglishCenterManagementDevContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllers()
            .AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });

            // Đăng ký các dịch vụ đã có
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<IChatService, ChatService>();



            var app = builder.Build();

            // Chỉ bật Swagger khi đang ở môi trường Development
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Cấu hình pipeline
            app.UseHttpsRedirection();
            app.UseRouting();
            // ✅ Sử dụng chính sách CORS cho phép credentials (Phải gọi trước UseAuthentication)
            app.UseCors("AllowSpecificOriginsWithCredentials");

            // 3. THÊM UseAuthentication
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}