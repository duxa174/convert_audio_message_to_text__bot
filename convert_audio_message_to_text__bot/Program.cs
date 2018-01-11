using System;
using System.IO;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

using Telegram.Bot;
using Telegram;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace convert_audio_message_to_text__bot
{
    class Program
    {
        //public static NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        static IConfigurationRoot cfg { get; set; }
        static YandexSpeech YaSpeech { get; set; }
        static TelegramBotClient bot { get; set; }

        static void Main(string[] args)
        {
            Console.WriteLine("Initialization");

            var builder = new ConfigurationBuilder().AddJsonFile("cfg.json");
            cfg = builder.Build();
            bot = new TelegramBotClient(cfg["telegramKey"]);
            YaSpeech = new YandexSpeech(cfg["YandexSpeechKitKey"]);

            bot.OnMessage += onMsg;
            bot.StartReceiving();
            while (true) { System.Threading.Thread.Sleep(999999999); }//Console.ReadLine(); //instead this
        }

        static void onMsg(object obj, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            //log.Info(message.Chat.Id.ToString());
            if (message == null) return;

            FileStream s = null;
            if (message.Type == MessageType.VoiceMessage)
            {
                var remoteFile = message.Voice;
                var voice = bot.GetFileAsync(remoteFile.FileId, s).Result;
                var YandexMime = MimeConverter.Convert(remoteFile.MimeType);

                var textFromVoice = YaSpeech.PostMethod(voice.FileStream, YandexMime);
                bot.SendTextMessageAsync(message.Chat.Id, textFromVoice);
            }
            else if (message.Type == MessageType.DocumentMessage)
            {
                var remoteFile = message.Document;
                var docVoice = bot.GetFileAsync(remoteFile.FileId, s).Result;
                var YandexMime = MimeConverter.Convert(remoteFile.MimeType);

                var textFromVoice = YaSpeech.PostMethod(docVoice.FileStream, YandexMime);
                bot.SendTextMessageAsync(message.Chat.Id, textFromVoice);
            }

        }
    }

    /// <summary>
    /// telegram to YandexSpeech
    /// </summary>
    static class MimeConverter
    {
        static public string Convert(string TelegramMime)
        {
            switch (TelegramMime)
            {
                case "audio/ogg": return "audio/ogg;codecs=opus";
                default: return TelegramMime;//works for wav
            }

        }
    }

    public class YandexSpeech
    {
        static string KEY = "";

        public YandexSpeech(string key)
        {
            KEY = key;
        }

        /// <summary>
        ///код взял отсюда http://www.cyberforum.ru/web-services-wcf/thread1310503.html
        /// </summary>
        public string PostMethod(Stream fs, string mimeType)
        {
            string postUrl = "https://asr.yandex.net/asr_xml?" +
            "uuid=" + System.Guid.NewGuid().ToString() + "&" +
            "key=" + KEY + "&" +
            "topic=queries";
            postUrl = "https://asr.yandex.net/asr_xml?uuid=c3d106c898d0433c80e7a791b790357d&key=41060ed1-38cf-4953-ba62-b36c1b9cbf52&topic=queries";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postUrl);
            request.Method = "POST";
            request.Host = "asr.yandex.net";
            request.SendChunked = true;
            request.ContentType = mimeType;//"audio/x-wav";//"audio/x-pcm;bit=16;rate=16000";
            request.ContentLength = fs.Length;

            using (var newStream = request.GetRequestStream())
            {
                fs.CopyTo(newStream);
                //newStream.Write(bytes, 0, bytes.Length); if by bytes
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string responseToString = "";
            if (response != null)
            {
                var strreader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);
                responseToString = strreader.ReadToEnd();
            }

            int index = responseToString.IndexOf("<variant confidence=\"");
            index = responseToString.IndexOf("\">", index);

            responseToString = responseToString.Substring(index + 2, responseToString.Length - index - 2);

            int index2 = responseToString.IndexOf("</variant>");

            responseToString = responseToString.Substring(0, index2);

            if (index == -1)//если несколько вариантов текста
            {
                Console.WriteLine(responseToString);
                return "";
            }
            return responseToString;
        }
    }
}
