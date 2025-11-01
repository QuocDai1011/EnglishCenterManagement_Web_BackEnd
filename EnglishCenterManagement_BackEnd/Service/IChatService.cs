namespace EnglishCenterManagement_BackEnd.Service
{
    public interface IChatService
    {
        Task<string> GenerateResponseAsync(string userMessage);
    }
}
