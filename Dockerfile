FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY *.csproj ./
RUN dotnet restore
COPY . ./
RUN dotnet publish ConsumersVoiceSystemPrototype.csproj -c Release -o /src/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /src/publish
COPY --from=build /src/publish .
ENV USE_SQLITE=1
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 10000
ENTRYPOINT ["dotnet", "ConsumersVoiceSystemPrototype.dll"]