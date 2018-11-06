using System;
using System.Collections.Generic;
using System.Text;

namespace convert_audio_message_to_text__bot.Helpers
{
    /// <summary>
    /// telegram to YandexSpeech
    /// </summary>
    public static class MimeConverter
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

}
