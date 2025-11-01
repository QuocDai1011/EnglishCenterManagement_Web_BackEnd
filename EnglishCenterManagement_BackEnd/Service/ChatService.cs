
using EnglishCenterManagement_BackEnd.Models;
using System.Reflection.Metadata.Ecma335;

namespace EnglishCenterManagement_BackEnd.Service
{
    public class ChatService : IChatService
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly EnglishCenterManagementDevContext _context;

        public ChatService(IConfiguration configuration, EnglishCenterManagementDevContext contex, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
            _context = contex;
        }


        public Task<string> GenerateResponseAsync(string userMessage)
        {   
            
        }
    }
}
