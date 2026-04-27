namespace ChatAI.UI
{
    public class ConsoleUI
    {
        private readonly dynamic _chatService;

        public ConsoleUI(dynamic chatService)
        {
            _chatService = chatService;
        }

        public void Run()
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

                var response = _chatService.SendMessage(input);
                Console.WriteLine($"AI: {response}");
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
                    _chatService.Clear();
                    Console.WriteLine("Conversation cleared.");
                    return true;

                case "/save":
                    _chatService.Save("chat.json");
                    Console.WriteLine("Saved.");
                    return true;

                case "/load":
                    _chatService.Load("chat.json");
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
