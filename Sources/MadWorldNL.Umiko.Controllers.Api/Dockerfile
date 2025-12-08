FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY [ "Directory.Build.props", "Directory.Build.props" ]
COPY [ "Directory.Packages.props", "Directory.Packages.props" ]
COPY ["Sources/MadWorldNL.Umiko.Controllers.Api/Api.csproj", "Sources/MadWorldNL.Umiko.Controllers.Api/"]
COPY ["Sources/MadWorldNL.Umiko.Core.Application/Application.csproj", "Sources/MadWorldNL.Umiko.Core.Application/"]
COPY ["Sources/MadWorldNL.Umiko.Core.Domain/Domain.csproj", "Sources/MadWorldNL.Umiko.Core.Domain/"]
COPY ["Sources/MadWorldNL.Umiko.Controllers.Contracts/Contracts.csproj", "Sources/MadWorldNL.Umiko.Controllers.Contracts/"]
COPY ["Sources/MadWorldNL.Umiko.Infrastructure.Database/Database.csproj", "Sources/MadWorldNL.Umiko.Infrastructure.Database/"]
RUN dotnet restore "Sources/MadWorldNL.Umiko.Controllers.Api/Api.csproj"
COPY . .
WORKDIR "/src/Sources/MadWorldNL.Umiko.Controllers.Api"
RUN dotnet build "./Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]
