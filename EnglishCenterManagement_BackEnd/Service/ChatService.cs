using EnglishCenterManagement_BackEnd.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace EnglishCenterManagement_BackEnd.Service
{
    public class ChatService : IChatService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ChatService> _logger;
        private readonly EnglishCenterManagementDevContext _context;
        private readonly HttpClient _httpClient;

        public ChatService(IConfiguration configuration, EnglishCenterManagementDevContext context, ILogger<ChatService> logger)
        {
            _configuration = configuration;
            _context = context;
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public async Task<string> GenerateResponseAsync(string userMessage)
        {
            // B1: Truy vấn dữ liệu
            var queryData = await _context.Courses.ToListAsync();
            string dataAsText = string.Join("\n", queryData.Select(c => $"{c.CourseName} - {c.Description} - {c.TutitionFee}"));

            // B2: Xây dựng prompt
            string systemPrompt = @"Bạn là trợ lý tư vấn tuyển sinh thân thiện và gần gũi của trung tâm ngoại ngữ.
Nhiệm vụ của bạn là dựa vào dữ liệu được cung cấp trong phần [BỐI CẢNH DỮ LIỆU] để trả lời câu hỏi của học viên.
Luôn trả lời dựa trên dữ liệu này. Nếu không tìm thấy thông tin, hãy nói bạn không tìm thấy.
Hãy tư vấn về khóa học, so sánh, và tư vấn lộ trình.
(Nếu học viên muốn đăng ký, hãy tạm thời hướng dẫn họ liên hệ hotline 0965399554).";

            string context = $"[BỐI CẢNH DỮ LIỆU]\n{dataAsText}\n[HẾT BỐI CẢNH DỮ LIỆU]";
            string userQuestion = $"Học viên hỏi: {userMessage}";
            string fullPrompt = $"{systemPrompt}\n\n{context}\n\n{userQuestion}";

            // B3: Gọi Gemini API
            string apiKey = _configuration["GoogleAiApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                return "Lỗi: API Key chưa được cấu hình trong appsettings.json.";

            try
            {
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = fullPrompt }
                            }
                        }
                    }
                };

                var response = await _httpClient.PostAsJsonAsync(
                    $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}",
                    requestBody);

                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(json);

                string? resultText = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return resultText ?? "Không có phản hồi từ Gemini API.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi Gemini API.");
                return $"Lỗi khi gọi Gemini API: {ex.Message}";
            }
        }
    }
}
