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
using System.Threading;

namespace convert_audio_message_to_text__bot
{
    class Program
    {
        //public static NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        static IConfigurationRoot cfg { get; set; }
        static YandexSpeech YaSpeech { get; set; }
        static TelegramBotClient bot { get; set; }
        static string lback = "";
        static int lBackMessageId = 0;
        static void l(string s)
        {
            try
            {
                Console.WriteLine(s);
                if (lBackMessageId == 0)
                    lBackMessageId = bot.SendTextMessageAsync(lback, s).Result.MessageId;

                bot.EditMessageTextAsync(lback, lBackMessageId, s);
            }
            catch
            { }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Initialization");

            var builder = new ConfigurationBuilder().AddJsonFile("cfg.json");
            cfg = builder.Build();
            try
            {
                var tId = string.Format("{0:d3} init_", Thread.CurrentThread.ManagedThreadId);
                l(tId + cfg["telegramKey"]);
                l(tId + cfg["YandexSpeechKitKey"]);
                bot = new TelegramBotClient(cfg["telegramKey"]);
                YaSpeech = new YandexSpeech(cfg["YandexSpeechKitKey"]);
                lback = cfg["logBack"];
                bot.OnMessage += onMsg;
                l(tId + "подписали обработчик на событие");

                bot.StartReceiving();
                l(tId + "StartReceiving DONE");
                Console.ReadLine();

            }
            catch (Exception e) { l(e.StackTrace); }
            while (true) { l("while " + DateTime.Now.ToString()); System.Threading.Thread.Sleep(999999999); }//Console.ReadLine(); //instead this
        }

        static void onMsg(object obj, MessageEventArgs messageEventArgs)
        {
            var tId = string.Format("{0:d3}_newMsg_", Thread.CurrentThread.ManagedThreadId);
            l(tId + DateTime.Now.ToString());
            var message = messageEventArgs.Message;
            //log.Info(message.Chat.Id.ToString());
            if (message == null) return;

            l(tId + "Type " + message.Type);
            l(tId + "From " + message.Chat.Id);
            FileStream s = null;
            string textFromVoice = "";
            if (message.Type == MessageType.VoiceMessage)
            {
                var remoteFile = message.Voice;
                var voice = bot.GetFileAsync(remoteFile.FileId, s).Result;
                var YandexMime = MimeConverter.Convert(remoteFile.MimeType);

                textFromVoice = YaSpeech.PostMethod(voice.FileStream, YandexMime);
                bot.SendTextMessageAsync(message.Chat.Id, textFromVoice);
            }
            else if (message.Type == MessageType.DocumentMessage)
            {
                var remoteFile = message.Document;
                var docVoice = bot.GetFileAsync(remoteFile.FileId, s).Result;
                var YandexMime = MimeConverter.Convert(remoteFile.MimeType);

                textFromVoice = YaSpeech.PostMethod(docVoice.FileStream, YandexMime);
                bot.SendTextMessageAsync(message.Chat.Id, textFromVoice);
            }
            l(tId + textFromVoice);

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
