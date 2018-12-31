# convert_audio_message_to_text__bot
телеграм-бот для перевода голосовых сообщений в текст (TTS)

##### you can do the same by yourself (or see gitlab-ci.yml)to 
    - install docker https://losst.ru/ustanovka-docker-na-ubuntu-16-04
    - docker build https://github.com/ayazzali/convert_audio_message_to_text__bot.git -t g3 
    - docker run --rm -it -e TKEY=123 -e YKEY=321 g3
            - where TKEY=TelegramKey from BotFather
            - and YKEY=SpeechKitCloudKey from https://tech.yandex.ru/speechkit/cloud or https://developer.tech.yandex.ru/keys
            - and LKEY=your "telegram chat id" for easy logs
### both keys are free for noncommercial purposes(SpeechKitCloudKey).

###### i use
```
sudo docker build --no-cache https://github.com/ayazzali/convert_audio_message_to_text__bot.git -t g3
 
sudo docker run  --name conv --restart unless-stopped  -d   -e TKEY=123123:2131jh2jk131hk2_13123 -e YKEY=123123-1233-1233-1231-1k2j31k2  g3

sudo docker logs conv
```

###### about nuance asr (create app and get 'appId' and 'appKey')
```
 curl "https://dictation.nuancemobility.net:443/NMDPAsrCmdServlet/dictation?appId=NMDPTRIAL_ayasdfli_masd_ru20160209233603&appKey=6sdf3676ba60b3sdfsdfsdf9cacb0e25e9315355a8fsdfsdfd0b782d26c78sdf8fc2094923bc985fbdd3esdfsdf6ac882700ddcff2sdfsdff6547c1csdfsdfaa&id=C4461956B60B"
 -H "Content-Type: audio/x-wav;codec=pcm;bit=16;rate=8000"
 -H "Accept-Language: rus-RUS"
 -H "Accept: application/xml"
 -k
 --data-binary @speech3.raw
 -v
```
see more at https://developer.nuance.com/public/index.php?task=prodDev


##### instead of roadmap (my thoughts)

i wanna do remainder
- so i need db
	- but my server is bad - it brokes every week often
	- i do have two server like this one
	- i wanna docker
	- i could do backup like every day or hour 
		- but where?
		- to google tables
