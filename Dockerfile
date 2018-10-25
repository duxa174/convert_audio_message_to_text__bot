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
# n0
WORKDIR /appr
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "convert_audio_message_to_text__bot.dll"]
# useless EXPOSE 80    
