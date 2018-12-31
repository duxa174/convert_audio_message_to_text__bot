using Concentus.Oggfile;
using Concentus.Structs;
using EricOulashin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace convert_audio_message_to_text__bot.Services
{
    /// <summary> ogg(opus) to wav(pcm) </summary>
    public class OpusToPcm
    {
        TgLog tgLog { get; set; }
        public OpusToPcm(Settings ss, TgLog tl) => tgLog = tl;

        /// <summary>
        /// only for telegram voice (its always 48000 and mono )
        /// to pcm 9 kHz
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public FileStream Do(Stream fs)
        {
            tgLog.l("OpusToPcm Do");
            OpusDecoder decoder = new OpusDecoder(48000, 1);
            OpusOggReadStream oggIn = new OpusOggReadStream(decoder, fs);
            var f = new WAVFile();
            var p = "speech.raw";
            f.Create(p, false, 8000, 16);

            while (oggIn.HasNextPacket)
            {
                short[] packet = oggIn.DecodeNextPacket();
                if (packet != null)
                    for (int c = 0; c < packet.Length; c += 6)
                    {
                        var g = (byte)(packet[c] & 0xFF);
                        var gg = (byte)((packet[c] >> 8) & 0xFF);// не нужно & 0xFF00 
                        f.AddSample_ByteArray(new[] { g, gg });
                    }
            }
            f.Close();
            var s=File.OpenRead(p);
            tgLog.l("OpusToPcm Do DONE");
            return s;
        }
    }
}
