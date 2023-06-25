FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/NetTsMapper.Cli/NetTsMapper.Cli.csproj", "src/NetTsMapper.Cli/"]
RUN dotnet restore "src/NetTsMapper.Cli/NetTsMapper.Cli.csproj"
COPY . .
WORKDIR "/src/src/NetTsMapper.Cli"
RUN dotnet build "NetTsMapper.Cli.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NetTsMapper.Cli.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NetTsMapper.Cli.dll"]
