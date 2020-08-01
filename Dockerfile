
# base dotnet core 3 image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 5001

# Copy projects and build
# Blazor server
FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS dotnetbuilder
WORKDIR /src
COPY ./BlazorApp ./BlazorApp
COPY ./Core ./Core
COPY ./AI/AI-lib ./AI/AI-lib
WORKDIR /src/BlazorApp
RUN dotnet restore ./Server/BlazorApp.Server.csproj
RUN dotnet build ./Server/BlazorApp.Server.csproj -c Release -o /app

# Publish
FROM dotnetbuilder AS publish
RUN dotnet publish ./Server/BlazorApp.Server.csproj -c Release -o /app

# Final
FROM base AS final
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_HTTPS_PORT=https://+5001
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BlazorApp.Server.dll"]