FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Samples/Sample.AspNetCore/Sample.AspNetCore.csproj", "src/Samples/Sample.AspNetCore/"]
RUN dotnet restore "src/Samples/Sample.AspNetCore/Sample.AspNetCore.csproj"
COPY . .
WORKDIR "/src/src/Samples/Sample.AspNetCore"
RUN dotnet build "Sample.AspNetCore.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Sample.AspNetCore.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sample.AspNetCore.dll"]
