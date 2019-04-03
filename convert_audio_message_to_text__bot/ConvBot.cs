using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using convert_audio_message_to_text__bot.Services;
using convert_audio_message_to_text__bot.Helpers;

namespace convert_audio_message_to_text__bot
{

    class ConvBot
    {
        //public static NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();
        //IConfigurationRoot cfg { get; set; }
        ISpeechRecognition YaSpeech { get; set; }
        TelegramBotClient bot { get; set; }
        TgLog tgLog { get; set; }

        public ConvBot(TelegramProvider b, TgLog l, ISpeechRecognition speech)
        {
            bot = b.bot;
            tgLog = l;
            YaSpeech = speech;
        }
        public void Start() //ConvBot
        {
            //try
            var tId = string.Format("{0:d3} init_", Thread.CurrentThread.ManagedThreadId);
            //l(tId + cfg["telegramKey"]);
            //l(tId + cfg["YandexSpeechKitKey"]);
            bot.OnMessage += onMsg;
            tgLog.l(tId + "подписали обработчик на событие");

            bot.StartReceiving();
            try { tgLog.l("Username= " + bot.GetMeAsync().Result.Username); } catch { }
            tgLog.l(tId + "StartReceiving DONE");
            Console.ReadLine();
            //catch (Exception e) { tgLog.l(e.StackTrace); }
            while (true) { tgLog.l("while " + DateTime.Now.ToString()); System.Threading.Thread.Sleep(60 * 1000); }//Console.ReadLine(); //instead this
        }

        void onMsg(object obj, MessageEventArgs messageEventArgs)
        {
            var tId = string.Format("{0:d3}_newMsg_", Thread.CurrentThread.ManagedThreadId);
            tgLog.l(tId + DateTime.Now.ToString());
            var message = messageEventArgs.Message;
            //log.Info(message.Chat.Id.ToString());
            if (message == null) return;

            tgLog.l(tId + "Type " + message.Type);
            tgLog.l(tId + "From " + message.Chat.Id);
            FileStream s = null;
            string textFromVoice = "";
            if (message.Type == MessageType.VoiceMessage)
            {
                var remoteFile = message.Voice;
                var voice = bot.GetFileAsync(remoteFile.FileId, s).Result;
                var YandexMime = MimeConverter.Convert(remoteFile.MimeType);

                textFromVoice = YaSpeech.PostMethod(voice.FileStream, YandexMime);
                textFromVoice = Process(textFromVoice);
                if (String.IsNullOrWhiteSpace(textFromVoice)) textFromVoice = "#itsempty";
                bot.SendTextMessageAsync(message.Chat.Id, textFromVoice);
            }
            else if (message.Type == MessageType.DocumentMessage)
            {
                var remoteFile = message.Document;
                var docVoice = bot.GetFileAsync(remoteFile.FileId, s).Result;
                var YandexMime = MimeConverter.Convert(remoteFile.MimeType);

                textFromVoice = YaSpeech.PostMethod(docVoice.FileStream, YandexMime);
                textFromVoice = Process(textFromVoice);
                bot.SendTextMessageAsync(message.Chat.Id, textFromVoice);
            }
            tgLog.l(tId + textFromVoice);
        }


        static string Process(string str)
        {
            str = IsHashTag(str);
            str = IsVopros(str);
            return str;
        }
        
        static string IsHashTag(string str)
        {
            Console.WriteLine(str);
            str = str.Replace("хэштег ", "#");
            str = str.Replace("хэштэг ", "#");
            str = str.Replace("хештег ", "#");
            str = str.Replace("хештэг ", "#");
            str = str.Replace("хэш-тег ", "#");
            str = str.Replace("хеш-тег ", "#");
            str = str.Replace("хэш-тэг ", "#");
            str = str.Replace("хеш-тэг ", "#");
            str = str.Replace("hashtag ", "#");
            Console.WriteLine(str);
            return str;
        }

        static string IsVopros(string str)
        {
            Console.WriteLine(str);
            str = str.Replace(" вопрос", " ?");
            Console.WriteLine(str);
            return str;
        }
    }
}
