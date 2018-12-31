# to run: docker build . -t g2 && docker run --rm -it     -e TKEY=123 -e YKEY=321 g2
FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY convert_audio_message_to_text__bot/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY convert_audio_message_to_text__bot/. ./
RUN dotnet publish -c Release -o out



# runtime image
FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /appr
COPY --from=build-env /app/out .
RUN echo $CI_pub_test $CI_prv_test
# cat cfg.json \ &&
ENTRYPOINT  echo $CI_pub_test $CI_prv_test qqq $Test1 $Test2 \
&& sed -i 's/key2/'$YKEY'/g' cfg.json \
&& sed -i 's/key3/'$LKEY'/g' cfg.json \
&& sed -i 's/key1/'$TKEY'/g' cfg.json \
&& sed -i 's/key4/'$NIKEY'/g' cfg.json \
&& sed -i 's/key5/'$NAKEY'/g' cfg.json \
# && cat cfg.json \
&& dotnet  convert_audio_message_to_text__bot.dll
#ENTRYPOINT ["dotnet", "convert_audio_message_to_text__bot.dll"]
