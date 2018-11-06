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
        public TgLog(TelegramProvider bp, Settings s) { bot = bp.bot; logBackTgId = s.cfg["logBack"]; }

        int lBackMessageId = 0;
        public void l(string s)
        {
            try
            {
                Console.WriteLine(s);
                if (lBackMessageId == 0)
                    lBackMessageId = bot.SendTextMessageAsync(logBackTgId, s).Result.MessageId;

                bot.EditMessageTextAsync(logBackTgId, lBackMessageId, s);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
