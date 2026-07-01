FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 9002

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY [".editorconfig", "FileService/"]

RUN --mount=type=secret,id=nuget_username \
    --mount=type=secret,id=nuget_password \
    dotnet nuget add source https://nuget.pkg.github.com/lokixx15/index.json \
        -n github \
        --username $(cat /run/secrets/nuget_username) \
        --password $(cat /run/secrets/nuget_password) \
        --store-password-in-clear-text

COPY ["FileService/src/FileService.Web/FileService.Web.csproj", "FileService/src/FileService.Web/"]
COPY ["FileService/src/FileService.Contracts/FileService.Contracts.csproj", "FileService/src/FileService.Contracts/"]
COPY ["FileService/src/FileService.Core/FileService.Core.csproj", "FileService/src/FileService.Core/"]
COPY ["FileService/src/FileService.Domain/FileService.Domain.csproj", "FileService/src/FileService.Domain/"]
COPY ["FileService/src/FileService.Infrastructure.Postgres/FileService.Infrastructure.Postgres.csproj", "FileService/src/FileService.Infrastructure.Postgres/"]
COPY ["FileService/src/FileService.Infrastructure.S3/FileService.Infrastructure.S3.csproj", "FileService/src/FileService.Infrastructure.S3/"]
RUN dotnet restore "./FileService/src/FileService.Web/FileService.Web.csproj"
COPY ["FileService", "FileService/"]

RUN dotnet tool install --global dotnet-ef --version 9.0.0
ENV PATH="/root/.dotnet/tools:${PATH}"

RUN dotnet ef migrations bundle \
    --project "FileService/src/FileService.Infrastructure.Postgres/FileService.Infrastructure.Postgres.csproj" \
    --startup-project "FileService/src/FileService.Web/FileService.Web.csproj" \
    --output /app/efbundle \
    --configuration Release \
    --verbose \
    --self-contained

WORKDIR "/src/FileService/src/FileService.Web"
RUN dotnet build "./FileService.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FileService.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /app/efbundle .
RUN chmod +x efbundle

USER $APP_UID

ENTRYPOINT ["dotnet", "FileService.Web.dll"]