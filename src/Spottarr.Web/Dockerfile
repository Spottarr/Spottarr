FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
RUN mkdir -p /data && chown -R $APP_UID /data && chmod -R 755 /data
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Spottarr.Web/Spottarr.Web.csproj", "src/Spottarr.Web/"]
COPY ["src/Spottarr.Data/Spottarr.Data.csproj", "src/Spottarr.Data/"]
COPY ["src/Spottarr.Services/Spottarr.Services.csproj", "src/Spottarr.Services/"]
COPY ./appsettings*.json ./
COPY ./Directory.Build.props ./
RUN dotnet restore "src/Spottarr.Web/Spottarr.Web.csproj"
COPY . .
WORKDIR "/src/src/Spottarr.Web"
RUN dotnet build "Spottarr.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Spottarr.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Spottarr.Web.dll"]
