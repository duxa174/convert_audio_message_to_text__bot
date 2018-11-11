using System;
using System.IO;

namespace WitAiSttExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("give path to .mp3");
            var path = Console.ReadLine();
            var s = new StreamReader(path);
            var res=new convert_audio_message_to_text__bot.Services.WitAiSpeech().PostMethod(s.BaseStream, "audio/mpeg3");
            Console.WriteLine(res);
            Console.ReadLine();

        }
    }
}
