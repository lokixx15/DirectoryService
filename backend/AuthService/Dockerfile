FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 9003

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY [".editorconfig", "AuthService/"]

RUN --mount=type=secret,id=nuget_username \
    --mount=type=secret,id=nuget_password \
    dotnet nuget add source https://nuget.pkg.github.com/lokixx15/index.json \
        -n github \
        --username $(cat /run/secrets/nuget_username) \
        --password $(cat /run/secrets/nuget_password) \
        --store-password-in-clear-text

COPY ["AuthService/src/AuthService.Web/AuthService.Web.csproj", "AuthService/src/AuthService.Web/"]
COPY ["AuthService/src/AuthService.Core/AuthService.Core.csproj", "AuthService/src/AuthService.Core/"]
COPY ["AuthService/src/AuthService.Contracts/AuthService.Contracts.csproj", "AuthService/src/AuthService.Contracts/"]
COPY ["AuthService/src/AuthService.Domain/AuthService.Domain.csproj", "AuthService/src/AuthService.Domain/"]
COPY ["AuthService/src/AuthService.Infrastructure.Postgres/AuthService.Infrastructure.Postgres.csproj", "AuthService/src/AuthService.Infrastructure.Postgres/"]
RUN dotnet restore "./AuthService/src/AuthService.Web/AuthService.Web.csproj"

RUN dotnet tool install --global dotnet-ef --version 9.0.0
ENV PATH="/root/.dotnet/tools:${PATH}"

WORKDIR "/src"

COPY ["AuthService", "AuthService/"]

RUN dotnet ef migrations bundle \
	--project "AuthService/src/AuthService.Infrastructure.Postgres/AuthService.Infrastructure.Postgres.csproj" \
	--startup-project "AuthService/src/AuthService.Web/AuthService.Web.csproj" \
	--output /app/efbundle \
	--configuration Release \
	--verbose \
	--self-contained

WORKDIR "/src/AuthService/src/AuthService.Web"
RUN dotnet build "./AuthService.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AuthService.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /app/efbundle .
RUN chmod +x efbundle

USER $APP_UID

ENTRYPOINT ["dotnet", "AuthService.Web.dll"]
