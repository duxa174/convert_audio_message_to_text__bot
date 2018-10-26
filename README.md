# convert_audio_message_to_text__bot
телеграм-бот для перевода голосовых сообщений в текст (TTS)

you can do the same by yourself 
    - docker build https://github.com/ayazzali/convert_audio_message_to_text__bot.git -t
g3 
    - docker run --rm -it -e TKEY=123 -e YKEY=321 g3
            - where TKEY=TelegramKey from BotFather
            - and YKEY=SpeechKitCloudKey from https://tech.yandex.ru/speechkit/cloud or https://developer.tech.yandex.ru/keys
### both keys are free for noncommercial purposes(SpeechKitCloudKey).
