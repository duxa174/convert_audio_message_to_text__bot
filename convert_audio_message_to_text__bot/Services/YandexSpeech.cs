using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace convert_audio_message_to_text__bot.Services
{
    public class YandexSpeech
    {
        TgLog tgLog { get; set; }
        string KEY = "";

        public YandexSpeech(Settings ss, TgLog tl)
        {
            KEY = ss.cfg["YandexSpeechKitKey"];
            tgLog = tl;
        }

        /// <summary>
        ///код взял отсюда http://www.cyberforum.ru/web-services-wcf/thread1310503.html
        /// </summary>
        public string PostMethod(Stream fs, string mimeType)
        {
            string postUrl = "https://asr.yandex.net/asr_xml?" +
            "uuid=" + System.Guid.NewGuid().ToString() + "&" +
            "key=" + KEY + "&" +
            "topic=queries";
            postUrl = "https://asr.yandex.net/asr_xml?uuid=c3d106c898d0433c80e7a791b790357d&key=41060ed1-38cf-4953-ba62-b36c1b9cbf52&topic=queries";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(postUrl);
            request.Method = "POST";
            request.Host = "asr.yandex.net";
            request.SendChunked = true;
            request.ContentType = mimeType;//"audio/x-wav";//"audio/x-pcm;bit=16;rate=16000";
            request.ContentLength = fs.Length;

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

            int index = responseToString.IndexOf("<variant confidence=\"");
            if (index == -1)
            {
                var badUser = responseToString.IndexOf("success=\"0\"")>1;
                if(!badUser) // it means smth wrong
                    tgLog.l(responseToString);
                return "";
            }
            index = responseToString.IndexOf("\">", index);

            responseToString = responseToString.Substring(index + 2, responseToString.Length - index - 2);

            int index2 = responseToString.IndexOf("</variant>");

            responseToString = responseToString.Substring(0, index2);

            if (index == -1)//если несколько вариантов текста
            {
                tgLog.l(responseToString);
                return "";
            }
            return responseToString;
        }
    }
}
