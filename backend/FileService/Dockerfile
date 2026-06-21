FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 9002

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]

RUN --mount=type=secret,id=nuget_username \
    --mount=type=secret,id=nuget_password \
    dotnet nuget add source https://nuget.pkg.github.com/lokixx15/index.json \
        -n github \
        --username $(cat /run/secrets/nuget_username) \
        --password $(cat /run/secrets/nuget_password) \
        --store-password-in-clear-text

COPY ["src/FileService.Web/FileService.Web.csproj", "src/FileService.Web/"]
COPY ["src/FileService.Contracts/FileService.Contracts.csproj", "src/FileService.Contracts/"]
COPY ["src/FileService.Core/FileService.Core.csproj", "src/FileService.Core/"]
COPY ["src/FileService.Domain/FileService.Domain.csproj", "src/FileService.Domain/"]
COPY ["src/FileService.Infrastructure.Postgres/FileService.Infrastructure.Postgres.csproj", "src/FileService.Infrastructure.Postgres/"]
COPY ["src/FileService.Infrastructure.S3/FileService.Infrastructure.S3.csproj", "src/FileService.Infrastructure.S3/"]
RUN dotnet restore "./src/FileService.Web/FileService.Web.csproj"
COPY . .

RUN dotnet tool install --global dotnet-ef --version 9.0.0
ENV PATH="/root/.dotnet/tools:${PATH}"

RUN dotnet ef migrations bundle \
    --project "src/FileService.Infrastructure.Postgres/FileService.Infrastructure.Postgres.csproj" \
    --startup-project "src/FileService.Web/FileService.Web.csproj" \
    --output /app/efbundle \
    --configuration Release \
    --verbose \
    --self-contained

WORKDIR "/src/src/FileService.Web"
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