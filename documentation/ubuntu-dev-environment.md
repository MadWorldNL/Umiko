# Ubuntu Dev Environment

This guide covers the steps needed to set up a development environment for Umiko on Ubuntu. It assumes you already have .NET SDK and Docker installed. The steps below configure HTTPS certificates and Docker permissions so the application can run without issues.

## 1. Setup Dev HTTPS Certificates

ASP.NET Core uses HTTPS during development, but on Linux, trusting self-signed development certificates is more complex than on Windows or macOS. Linux distributions differ in how they mark certificates as trusted, and browsers use different certificate stores. The `linux-dev-certs` tool simplifies this process.

```bash
dotnet tool update -g linux-dev-certs
dotnet linux-dev-certs install
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

- `dotnet tool update -g linux-dev-certs` - Installs or updates the `linux-dev-certs` global tool, a community-supported tool that creates and trusts developer certificates on Linux.
- `dotnet linux-dev-certs install` - Generates a development certificate and registers it as trusted in the system and browser certificate stores.
- `dotnet dev-certs https --clean` - Removes any previously generated ASP.NET Core HTTPS development certificates.
- `dotnet dev-certs https --trust` - Trusts the ASP.NET Core HTTPS development certificate.

Source: [Trust the ASP.NET Core HTTPS development certificate - Microsoft](https://learn.microsoft.com/en-us/aspnet/core/security/enforcing-ssl?view=aspnetcore-10.0&tabs=visual-studio%2Clinux-sles#trust-the-aspnet-core-https-development-certificate)

## 2. Give Docker User Rights

By default, Docker requires `sudo` to run because the Docker daemon binds to a Unix socket owned by the `root` user. To avoid using `sudo` for every Docker command, you can add your user to the `docker` group.

```bash
sudo groupadd docker
sudo usermod -aG docker $USER
newgrp docker
sudo systemctl restart docker
```

- `sudo groupadd docker` - Creates the `docker` group if it does not already exist.
- `sudo usermod -aG docker $USER` - Adds your current user to the `docker` group. The `-aG` flag appends the group without removing existing group memberships.
- `newgrp docker` - Activates the new group membership in the current shell session without requiring a logout.
- `sudo systemctl restart docker` - Restarts the Docker daemon to apply the group changes.

Source: [How to fix Docker permission denied - StackOverflow](https://stackoverflow.com/questions/48957195/how-to-fix-docker-permission-denied)
