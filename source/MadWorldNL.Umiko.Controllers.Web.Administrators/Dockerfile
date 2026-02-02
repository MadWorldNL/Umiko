FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files for restore
COPY Directory.Build.props Directory.Packages.props ./
COPY MadWorldNL.Umiko.Controllers.Web.Administrators/Web.Administrators.csproj MadWorldNL.Umiko.Controllers.Web.Administrators/
COPY MadWorldNL.Umiko.Controllers.Api.Contracts/Api.Contracts.csproj MadWorldNL.Umiko.Controllers.Api.Contracts/

# Restore dependencies
RUN dotnet restore MadWorldNL.Umiko.Controllers.Web.Administrators/Web.Administrators.csproj

# Copy source code
COPY MadWorldNL.Umiko.Controllers.Web.Administrators/ MadWorldNL.Umiko.Controllers.Web.Administrators/
COPY MadWorldNL.Umiko.Controllers.Api.Contracts/ MadWorldNL.Umiko.Controllers.Api.Contracts/

# Publish the Blazor WASM app
RUN dotnet publish MadWorldNL.Umiko.Controllers.Web.Administrators/Web.Administrators.csproj \
    -c Release \
    -o /app/publish

FROM nginx:alpine AS final

# Remove default nginx config
RUN rm /etc/nginx/conf.d/default.conf

# Copy custom nginx config
COPY MadWorldNL.Umiko.Controllers.Web.Administrators/DockerConfigs/nginx.conf /etc/nginx/conf.d/default.conf

# Copy published Blazor WASM output to nginx
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html

EXPOSE 80