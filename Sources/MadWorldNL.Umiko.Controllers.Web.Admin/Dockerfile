FROM nginx:1.29.1 AS base
USER $APP_UID
EXPOSE 80
EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILDPLATFORM
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# COPY ["NuGet.Config", "./"]
COPY ["Directory.Build.props", "./"]
COPY ["Directory.Packages.props", "./"]
COPY ["Sources/MadWorldNL.Umiko.Controllers.Web.Admin/Web.Admin.csproj", "Sources/MadWorldNL.Umiko.Controllers.Web.Admin/"]
RUN dotnet restore "Sources/MadWorldNL.Umiko.Controllers.Web.Admin/Web.Admin.csproj"
COPY ["Sources/MadWorldNL.Umiko.Controllers.Web.Admin/", "Sources/MadWorldNL.Umiko.Controllers.Web.Admin/"]
COPY ["Sources/MadWorldNL.Umiko.Controllers.Contracts/", "Sources/MadWorldNL.Umiko.Controllers.Contracts/"]
# COPY ["src/MadWorldNL.MadTransfer.Controllers.Web/Configs/web/appsettings.json", "MadWorldNL.MadTransfer.Controllers.Web/wwwroot"]
WORKDIR "/src/Sources/MadWorldNL.Umiko.Controllers.Web.Admin"
RUN dotnet build "./Web.Admin.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Web.Admin.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
LABEL org.opencontainers.image.source="https://github.com/MadWorldNL/Umiko"

COPY --from=publish /app/publish/wwwroot /usr/share/nginx/html
# COPY ["src/MadWorldNL.MadTransfer.Controllers.Web/Configs/nginx/default.conf", "/etc/nginx/conf.d/"]
# RUN rm /usr/share/nginx/html/appsettings.json.br \
# && rm /usr/share/nginx/html/appsettings.json.gz