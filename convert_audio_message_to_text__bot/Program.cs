using System;
using Microsoft.Extensions.DependencyInjection;
using convert_audio_message_to_text__bot.Services;

namespace convert_audio_message_to_text__bot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initialization Main");
            // new ConvBot();
            var serv = new ServiceCollection()
            .AddSingleton<Settings>()
            .AddSingleton<TelegramProvider>()
            .AddSingleton<TgLog>()
            .AddSingleton<ConvBot>()
            //.AddSingleton<YandexSpeech>()
            .AddSingleton<IStt,WitAiSpeech>()

            .BuildServiceProvider();

            var InstanceOfBot = serv.GetService<ConvBot>();
            InstanceOfBot.Start();
            
        }
    }

}
