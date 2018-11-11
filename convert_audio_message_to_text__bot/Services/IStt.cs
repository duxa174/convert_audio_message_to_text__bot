using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace convert_audio_message_to_text__bot.Services
{
    /// <summary>
    /// speech to text
    /// </summary>
    public interface IStt
    {
        string PostMethod(Stream fs, string mimeType);
    }
}
