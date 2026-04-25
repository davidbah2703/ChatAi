using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatAI.Models;
using ChatAI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ChatAI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
           
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.File("logs/chatai-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                using var host = Host.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((ctx, cfg) =>
                    {
                        cfg.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                        cfg.AddEnvironmentVariables();
                    })
                    .ConfigureServices((ctx, services) =>
                    {
                        
                        services.Configure<OpenAIOptions>(ctx.Configuration.GetSection("OpenAI"));
                        services.PostConfigure<OpenAIOptions>(opts =>
                        {
                            var envKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
                            if (!string.IsNullOrWhiteSpace(envKey)) opts.ApiKey = envKey;
                        });

                      
                        services.AddSingleton<ISentimentService, SentimentService>();
                        services.AddSingleton<IConversationManager, ConversationManager>();
                        
                        
                        services.AddHttpClient<IOpenAIService, OpenAIService>();
                    })
                    .UseSerilog()
                    .Build();

         
                var chatService = host.Services.GetRequiredService<IOpenAIService>();
                var manager = host.Services.GetRequiredService<IConversationManager>();

                
                manager.LoadFromFile();
// holder for Member 3
                string userPrompt = "I’m stressed with school, give me advice";
                
                
                manager.AddMessage("user", userPrompt);

                Console.WriteLine("Calling OpenAI with history and sentiment tracking...");

               
                var reply = await chatService.SendMessageAsync(manager.GetHistory());

                
                manager.AddMessage("assistant", reply);

               
                manager.SaveToFile();

                
                Console.WriteLine($"\nAssistant: {reply}");
                
                
                var lastUserMsg = manager.GetHistory().LastOrDefault(m => m.Role == "user");
                Console.WriteLine($"\n[MEMBER 2 DEBUG] Detected Sentiment: {lastUserMsg?.Sentiment}");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}