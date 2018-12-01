using System;
using Microsoft.Extensions.DependencyInjection;
using convert_audio_message_to_text__bot.Services;
using Microsoft.Extensions.Logging;

namespace convert_audio_message_to_text__bot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initialization Main");

            var rootPath = System.IO.Path.GetPathRoot(AppContext.BaseDirectory);
            var logDir = System.IO.Path.Combine(rootPath, "logs", "convert_audio_message_to_text__bot");
            System.IO.Directory.CreateDirectory(logDir); 
            var logPath = System.IO.Path.Combine(logDir, "app.log");
            
            var serv = new ServiceCollection()
                .AddLogging(loggingBuilder => loggingBuilder
                    .AddFile(logPath, append: true)
                    .AddConsole())
                .AddSingleton<Settings>()
                .AddSingleton<TelegramProvider>()
                .AddSingleton<TgLog>()
                .AddSingleton<ConvBot>()
                .AddSingleton<YandexSpeech>()
                .BuildServiceProvider();
                        
            var InstanceOfBot = serv.GetService<ConvBot>();
            InstanceOfBot.Start();

        }
    }

}
