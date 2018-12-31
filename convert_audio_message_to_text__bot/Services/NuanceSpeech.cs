using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace convert_audio_message_to_text__bot.Services
{
    public class NuanceSpeech : ISpeechRecognition
    {
        TgLog tgLog { get; set; }
        string appId = "";
        string appKey = "";
        OpusToPcm opusToPcm;

        public NuanceSpeech(Settings ss, TgLog tl, OpusToPcm opusToPcm)
        {
            appId = ss.cfg["NuanceSpeechAppId"];
            appKey = ss.cfg["NuanceSpeechAppKey"];
            tgLog = tl;
            this.opusToPcm = opusToPcm;
        }

        /// <summary>
        ///код взял отсюда http://www.cyberforum.ru/web-services-wcf/thread1310503.html
        /// </summary>
        public string PostMethod(Stream fs, string mimeType)
        {
            var wav = opusToPcm.Do(fs /*"audio/ogg;codecs=opus"*/);

            string postUrl = "https://dictation.nuancemobility.net:443/NMDPAsrCmdServlet/dictation?"
            + $"appId={appId}&appKey={appKey}";// "&ttsLang=en_US";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postUrl);
            request.Method = "POST";
            request.SendChunked = true; // optionally 
            request.ContentType = "audio/x-wav;codec=pcm;bit=16;rate=8000";
            request.Headers.Add("Accept-Language: rus-RUS");
            // request.Headers.Add("Accept: application/xml"); // optionally 
            request.ContentLength = wav.Length;
            using (var newStream = request.GetRequestStream())
                wav.CopyTo(newStream);
            wav.Close();//!!!
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string responseToString = "";
            if (response != null)
            {
                var strreader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8);
                responseToString = strreader.ReadLine(); //responseToString = strreader.ReadToEnd();
            }
            tgLog.l(responseToString);
            return responseToString;
        }
    }
}
