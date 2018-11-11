using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;

namespace convert_audio_message_to_text__bot.Services
{
    public class TgLog
    {
        TelegramBotClient bot { get; set; }
        string logBackTgId = "";
        readonly ILogger<TgLog> logger;
        public TgLog(TelegramProvider bp, Settings s, ILoggerFactory loggerFactory)
        {
            logger=loggerFactory.CreateLogger<TgLog>(); // 
            bot = bp.bot;
            logBackTgId = s.cfg["logBack"];
            if (string.IsNullOrWhiteSpace(logBackTgId))
                Console.WriteLine("WARN: logs will not be sent to your telegram chat");
        }

        int lBackMessageId = 0;
        public void l(string s)
        {
            logger.LogInformation(s);
            if (!string.IsNullOrWhiteSpace(logBackTgId))
                try
                {
                    if (lBackMessageId == 0)
                        lBackMessageId = bot.SendTextMessageAsync(logBackTgId, s).Result.MessageId;

                    bot.EditMessageTextAsync(logBackTgId, lBackMessageId, s);
                }
                catch (Exception e)
                {
                    logger.LogError("tg send error: ",e);
                }
        }
    }
}
