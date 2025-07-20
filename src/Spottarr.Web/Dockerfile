FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview-alpine AS base
RUN mkdir -p /data && chown -R $APP_UID /data && chmod -R 755 /data
RUN apk add --no-cache curl tzdata
USER $APP_UID
WORKDIR /app
ENV DOTNET_RUNNING_IN_CONTAINER_FAST_MODE=true
ENV ASPNETCORE_HTTP_PORTS=8383
EXPOSE 8383

FROM mcr.microsoft.com/dotnet/sdk:10.0-preview-alpine AS build
ARG BUILD_CONFIGURATION=Release
ARG BUILD_VERSION=v1.0.0
WORKDIR /src
COPY ["src/Spottarr.Web/Spottarr.Web.csproj", "src/Spottarr.Web/"]
COPY ["src/Spottarr.Data/Spottarr.Data.csproj", "src/Spottarr.Data/"]
COPY ["src/Spottarr.Services/Spottarr.Services.csproj", "src/Spottarr.Services/"]
COPY ./appsettings*.json ./
COPY ./Directory.Build.props ./
COPY ./Directory.Packages.props ./
RUN dotnet restore "src/Spottarr.Web/Spottarr.Web.csproj"
COPY . .
WORKDIR "/src/src/Spottarr.Web"
RUN dotnet build "Spottarr.Web.csproj" \
    --no-restore \
    -c $BUILD_CONFIGURATION \
    -p:UseAppHost=false \
    -p:Version=${BUILD_VERSION#v}

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Spottarr.Web.csproj" \
    --no-build \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    -p:UseAppHost=false \
    -p:Version=${BUILD_VERSION#v}

FROM base AS final
ENV DOTNET_ENVIRONMENT=Production
ENV DOTNET_RUNNING_IN_CONTAINER_FAST_MODE=false
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /src/appsettings*.json .
HEALTHCHECK CMD curl --fail http://localhost:8383/healthz || exit 1
ENTRYPOINT ["dotnet", "Spottarr.Web.dll"]
