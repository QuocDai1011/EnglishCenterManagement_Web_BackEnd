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
            // B1: Lấy dữ liệu khóa học
            var queryData = await _context.Courses.ToListAsync();
            string dataAsText = string.Join("\n", queryData.Select(c => $"{c.CourseName} - {c.Description} - {c.TutitionFee}"));

            // B2: Tạo prompt
            string systemPrompt = @"Bạn là trợ lý tư vấn tuyển sinh thân thiện và gần gũi của trung tâm ngoại ngữ.
Nhiệm vụ của bạn là dựa vào dữ liệu được cung cấp trong phần [BỐI CẢNH DỮ LIỆU] để trả lời câu hỏi của học viên.
Luôn trả lời dựa trên dữ liệu này. Nếu không tìm thấy thông tin, hãy nói bạn không tìm thấy.
Hãy tư vấn về khóa học, so sánh, và tư vấn lộ trình.
(Nếu học viên muốn đăng ký, hãy tạm thời hướng dẫn họ liên hệ hotline 0965399554).";

            string context = $"[BỐI CẢNH DỮ LIỆU]\n{dataAsText}\n[HẾT BỐI CẢNH DỮ LIỆU]";
            string userQuestion = $"Học viên hỏi: {userMessage}";
            string fullPrompt = $"{systemPrompt}\n\n{context}\n\n{userQuestion}";

            // B3: Gọi Gemini API với retry
            string apiKey = _configuration["GoogleAiApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                return "Lỗi: API Key chưa được cấu hình trong appsettings.json.";

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

            const int MAX_RETRIES = 3;
            const int BASE_DELAY_MS = 2000; // 2 giây

            for (int attempt = 1; attempt <= MAX_RETRIES; attempt++)
            {
                try
                {
                    var response = await _httpClient.PostAsJsonAsync(
                        $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key={apiKey}",
                        requestBody);

                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        int delay = BASE_DELAY_MS * attempt;
                        _logger.LogWarning($"Gemini API trả về 429. Thử lại lần {attempt}/{MAX_RETRIES} sau {delay}ms...");
                        await Task.Delay(delay);
                        continue;
                    }

                    // Nếu lỗi khác ngoài 429, ném ra luôn
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
                catch (HttpRequestException ex)
                {
                    _logger.LogError(ex, $"Lỗi HTTP khi gọi Gemini API (lần {attempt}/{MAX_RETRIES})");
                    if (attempt == MAX_RETRIES) throw;
                    await Task.Delay(BASE_DELAY_MS * attempt);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi không xác định khi gọi Gemini API.");
                    return "Lỗi hệ thống khi xử lý yêu cầu. Vui lòng thử lại sau.";
                }
            }

            return "Hệ thống đang quá tải. Vui lòng thử lại sau ít phút.";
        }

    }
}
