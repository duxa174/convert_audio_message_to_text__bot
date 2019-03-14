using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;

namespace convert_audio_message_to_text__bot.Services
{
    /// <summary>
    ///  в телегу пишет поставив в очередь (в 2сек)
    /// </summary>
    public class TgLog
    {
        TelegramBotClient bot { get; set; }
        string logBackTgId = "";
        readonly ILogger<TgLog> logger;
        public TgLog(TelegramProvider bp, Settings s, ILoggerFactory loggerFactory)
        {
            logger = loggerFactory.CreateLogger<TgLog>(); // 
            bot = bp.bot;
            logBackTgId = s.cfg["logBack"];
            if (string.IsNullOrWhiteSpace(logBackTgId))
                Console.WriteLine("WARN: logs will not be sent to your telegram chat");

            if (!isRunningQueueLogHandler)
                Task.Run(async () =>
                {
                    isRunningQueueLogHandler = true;
                    while (true)
                    {
                        await Task.Delay(4000);
                        if (vs.TryDequeue(out string f))
                            _l(f);
                    }
                });
        }

        static bool isRunningQueueLogHandler; // если вдруг не синглтон - useless
        static Queue<string> vs = new Queue<string>();
        public void l(string s)
        {
            logger.LogInformation(s);
            vs.Enqueue(s);
        }

        static int lBackMessageId = 0;
        private void _l(string s)
        {
            if (!string.IsNullOrWhiteSpace(logBackTgId))
                try
                {
                    if (lBackMessageId == 0)
                        lBackMessageId = bot.SendTextMessageAsync(logBackTgId, s).Result.MessageId;

                    bot.EditMessageTextAsync(logBackTgId, lBackMessageId, s);
                }
                catch (Exception e)
                {
                    logger.LogError("tg send error: ", e);
                }
        }
    }
}
