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

# cat cfg.json \ &&
ENTRYPOINT  sed -i 's/key2/'$YKEY'/g' cfg.json \
&& sed -i 's/key3/'$LKEY'/g' cfg.json \
&& sed -i 's/key1/'$TKEY'/g' cfg.json \
# && cat cfg.json \
&& dotnet  convert_audio_message_to_text__bot.dll
#ENTRYPOINT ["dotnet", "convert_audio_message_to_text__bot.dll"]

