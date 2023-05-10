FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
LABEL maintainer="Kamil Janek"
WORKDIR /source

COPY . .

RUN dotnet restore
#RUN dotnet publish -c Release -o /app --no-restore
#RUN dotnet restore API/src/TibiaEnemyOtherCharactersFinder.Api/TibiaEnemyOtherCharactersFinder.Api.csproj
RUN dotnet publish API/src/TibiaEnemyOtherCharactersFinder.Api/TibiaEnemyOtherCharactersFinder.Api.csproj -c Release -o /app --no-restore
#RUN dotnet restore Seeders/src/CharacterAnalyser/CharacterAnalyser.csproj
RUN dotnet publish Seeders/src/CharacterAnalyser/CharacterAnalyser.csproj -c Release -o /app --no-restore
#RUN dotnet restore Seeders/src/DbTableCleaner/DbCleaner.csproj
RUN dotnet publish Seeders/src/DbTableCleaner/DbCleaner.csproj -c Release -o /app --no-restore
#RUN dotnet restore Seeders/src/WorldScanSeeder/WorldScanSeeder.csproj
RUN dotnet publish Seeders/src/WorldScanSeeder/WorldScanSeeder.csproj -c Release -o /app --no-restore
#RUN dotnet restore Seeders/src/WorldSeeder/WorldSeeder.csproj
RUN dotnet publish Seeders/src/WorldSeeder/WorldSeeder.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

COPY --from=build /app ./

EXPOSE 80