using Xunit;

public class ConversationTests
{
    [Fact]
    public void AddMessage_ShouldNotFail()
    {
        var service = new ChatService();

        var response = service.SendMessage("Hello");

        Assert.NotNull(response);
    }

    [Fact]
    public void SaveLoad_ShouldWork()
    {
        var service = new ChatService();

        service.SendMessage("Test message");
        service.Save("test.json");

        var newService = new ChatService();
        newService.Load("test.json");

        var response = newService.SendMessage("Hi again");

        Assert.NotNull(response);
    }
}
