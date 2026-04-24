using System;
using System.Collections.Generic;

namespace ChatAI.Models
{
    public class Message
    {
        public string Role { get; set; } = "user";
        public string Content { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? Sentiment { get; set; }

        public Message() { }
        public Message(string role, string content)
        {
            Role = role;
            Content = content;
            Timestamp = DateTime.UtcNow;
        }
    }

    public class Conversation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = "New Conversation";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<Message> Messages { get; set; } = new List<Message>();

        public void AddMessage(string role, string content)
        {
            Messages.Add(new Message(role, content));
        }
    }

    public class OpenAIOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gpt-4o-mini";
        public string BaseUrl { get; set; } = "https://api.openai.com/v1";
        public int MaxRetries { get; set; } = 4;
        public double Temperature { get; set; } = 0.7;
    }
}
