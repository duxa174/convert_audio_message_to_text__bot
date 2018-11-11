using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace convert_audio_message_to_text__bot.Services
{
    static class tgLog
    {
        public static void l(object l)
        {
            Console.WriteLine(l);
        }
    }
    public class WitAiSpeech
    {
        //TgLog tgLog { get; set; }
        string KEY = "6VNPTXNSZ3YSIXXSRKBP7FD3TQWSGWTD";

        //public WitAiSpeech(Settings ss, TgLog tl)
        //{
        //    KEY = ss.cfg["WitAiKey"];
        //    tgLog = tl;
        //}

        /// <summary>
        ///код взял отсюда http://www.cyberforum.ru/web-services-wcf/thread1310503.html
        /// </summary>
        public string PostMethod(Stream fs, string mimeType)
        {
            string postUrl = "https://api.wit.ai/speech?v=20170307";
            //"uuid=" + System.Guid.NewGuid().ToString().Replace("-", "") + "&" +
            //"key=" + KEY + "&" +
            //"topic=queries";
            //postUrl = "https://asr.yandex.net/asr_xml?uuid=c3d106c898d0433c80e7a791b790357d&key=41060ed1-38cf-4953-ba62-b36c1b9cbf52&topic=queries";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postUrl);
            request.Method = "POST";
            //request.Host = "asr.yandex.net";
            //request.SendChunked = true;
            request.ContentType = mimeType;//"audio/x-wav";//"audio/x-pcm;bit=16;rate=16000";
            //request.ContentLength = fs.Length;
            request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + KEY);

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
            tgLog.l(responseToString);
            
            return responseToString;
        }
    }
}
