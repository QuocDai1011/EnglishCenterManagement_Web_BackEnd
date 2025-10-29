using EnglishCenterManagement_BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace EnglishCenterManagement_BackEnd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Bật CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy
                        .AllowAnyOrigin()   // Cho phép mọi domain (như http://127.0.0.1:5500)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            // Thêm các dịch vụ
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // ✅ Thêm DbContext vào Dependency Injection container
            builder.Services.AddDbContext<EnglishCenterManagementDevContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");
            Console.WriteLine($"DefaultConnection: {builder.Configuration.GetConnectionString("DefaultConnection")}");

            // Bật CORS (nếu cần dùng frontend)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });


            var app = builder.Build();

            // Chỉ bật Swagger khi đang ở môi trường Development
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Cấu hình pipeline
            app.UseHttpsRedirection();

            // ✅ Phải gọi UseCors trước UseAuthorization
            app.UseCors("AllowAll");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
