using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace convert_audio_message_to_text__bot.Services
{
    interface ISpeechRecognition
    {
        string PostMethod(Stream fs, string mimeType);
    }
}
