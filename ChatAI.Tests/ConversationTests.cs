using ChatAI.Services;
using Xunit;

public class ConversationTests
{
    [Fact]
    public void AddMessage_ShouldAppendToHistory()
    {
        var sentiment = new SentimentService();
        var manager = new ConversationManager(sentiment);

        manager.AddMessage("user", "Hello");

        Assert.Single(manager.GetHistory());
        Assert.Equal("Hello", manager.GetHistory()[0].Content);
    }

    [Fact]
    public void Clear_ShouldResetHistory()
    {
        var sentiment = new SentimentService();
        var manager = new ConversationManager(sentiment);

        manager.AddMessage("user", "Hello");
        manager.Clear();

        Assert.Empty(manager.GetHistory());
    }

    [Fact]
    public void SaveLoad_ShouldPreserveMessages()
    {
        var sentiment = new SentimentService();
        var manager = new ConversationManager(sentiment);
        var path = Path.GetTempFileName();

        manager.AddMessage("user", "Test message");
        manager.SaveToFile(path);

        var manager2 = new ConversationManager(sentiment);
        manager2.LoadFromFile(path);

        Assert.Single(manager2.GetHistory());
        Assert.Equal("Test message", manager2.GetHistory()[0].Content);

        File.Delete(path);
    }

    [Fact]
    public void SentimentService_DetectsPositive()
    {
        var service = new SentimentService();
        Assert.Equal("Positive", service.Analyze("This is great and helpful!"));
    }

    [Fact]
    public void SentimentService_DetectsNegative()
    {
        var service = new SentimentService();
        Assert.Equal("Negative", service.Analyze("This is terrible and broken"));
    }
}
