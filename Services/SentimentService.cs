using ChatAI.Models;

namespace ChatAI.Services
{
    public interface ISentimentService
    {
        string Analyze(string text);
    }

    public class SentimentService : ISentimentService
    {
        public string Analyze(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "Neutral";

            var input = text.ToLower();

            // Simple keyword-based sentiment analysis (for demonstration purposes)
            var positiveWords = new[] { "happy", "great", "excellent", "thanks", "helpful", "love", "good" };
            var negativeWords = new[] { "bad", "error", "terrible", "angry", "broken", "fail", "hate" };

            int score = 0;
            foreach (var word in positiveWords) if (input.Contains(word)) score++;
            foreach (var word in negativeWords) if (input.Contains(word)) score--;

            return score > 0 ? "Positive" : score < 0 ? "Negative" : "Neutral";
        }
    }
}