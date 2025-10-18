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
