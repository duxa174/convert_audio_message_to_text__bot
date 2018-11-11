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
            // new ConvBot();
            var serv = new ServiceCollection()
                .AddLogging()
                .AddSingleton<Settings>()
                .AddSingleton<TelegramProvider>()
                .AddSingleton<TgLog>()
                .AddSingleton<ConvBot>()
                .AddSingleton<YandexSpeech>()
                .BuildServiceProvider();

            serv.GetService<ILoggerFactory>()
                .AddConsole();

            var InstanceOfBot = serv.GetService<ConvBot>();
            InstanceOfBot.Start();

        }
    }

}
