using ChatAI.Services;

namespace ChatAI.UI
{
    public class ConsoleUI
    {
        private readonly IOpenAIService _chatService;
        private readonly IConversationManager _manager;

        public ConsoleUI(IOpenAIService chatService, IConversationManager manager)
        {
            _chatService = chatService;
            _manager = manager;
        }

        public async Task Run()
        {
            Console.WriteLine("Chat AI started. Type /help for commands.");

            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(input))
                    continue;

                if (HandleCommand(input))
                    continue;

                _manager.AddMessage("user", input);
                try
                {
                    var response = await _chatService.SendMessageAsync(_manager.GetHistory());
                    _manager.AddMessage("assistant", response);
                    Console.WriteLine($"AI: {response}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }

        private bool HandleCommand(string input)
        {
            switch (input.ToLower())
            {
                case "/exit":
                    Environment.Exit(0);
                    return true;

                case "/help":
                    ShowHelp();
                    return true;

                case "/clear":
                    _manager.Clear();
                    Console.WriteLine("Conversation cleared.");
                    return true;

                case "/save":
                    _manager.SaveToFile("chat.json");
                    Console.WriteLine("Saved.");
                    return true;

                case "/load":
                    _manager.LoadFromFile("chat.json");
                    Console.WriteLine("Loaded.");
                    return true;

                default:
                    return false;
            }
        }

        private void ShowHelp()
        {
            Console.WriteLine(@"
Commands:
/help   - Show commands
/exit   - Quit app
/clear  - Reset conversation
/save   - Save chat
/load   - Load chat
");
        }
    }
}
