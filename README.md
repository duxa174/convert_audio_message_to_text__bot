# convert_audio_message_to_text__bot
телеграм-бот для перевода голосовых сообщений в текст (TTS)

##### you can do the same by yourself 
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
