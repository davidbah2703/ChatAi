using System.Text.Json;
using ChatAI.Models;

namespace ChatAI.Services
{
    public interface IConversationManager
    {
        void AddMessage(string role, string content);
        List<Message> GetHistory();
        void Clear();
        void SaveToFile(string filePath = "chat_history.json");
        void LoadFromFile(string filePath = "chat_history.json");
    }

    public class ConversationManager : IConversationManager
    {
        private Conversation _activeConversation = new();
        private readonly ISentimentService _sentimentService;

        public ConversationManager(ISentimentService sentimentService)
        {
            _sentimentService = sentimentService;
        }
        public List<Message> SearchMessages(string keyword)
        {
            return _activeConversation.Messages
                .Where(m => m.Content.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        public void PrintStats()
        {
            int pos = _activeConversation.Messages.Count(m => m.Sentiment == "Positive");
            int neg = _activeConversation.Messages.Count(m => m.Sentiment == "Negative");
            Console.WriteLine($"-- Conversation Stats --");
            Console.WriteLine($"Total Messages: {_activeConversation.Messages.Count}");
            Console.WriteLine($"Positive: {pos}, Negative: {neg}, Neutral: {_activeConversation.Messages.Count - pos - neg}");
        }
        public void AddMessage(string role, string content)
        {
            var message = new Message(role, content);
            
           
            if (role.ToLower() == "user")
            {
                message.Sentiment = _sentimentService.Analyze(content);
            }
            else
            {
                message.Sentiment = "Neutral";
            }

            _activeConversation.Messages.Add(message);
        }

        public List<Message> GetHistory() => _activeConversation.Messages;

        public void Clear() => _activeConversation = new Conversation();

       
        public void SaveToFile(string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(_activeConversation, options);
            File.WriteAllText(filePath, json);
        }

        
        public void LoadFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var loaded = JsonSerializer.Deserialize<Conversation>(json);
                if (loaded != null)
                {
                    _activeConversation = loaded;
                }
            }
        }
    }
}