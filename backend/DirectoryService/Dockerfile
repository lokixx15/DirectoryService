FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 9001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
COPY [".editorconfig", "DirectoryService/"]

RUN --mount=type=secret,id=nuget_username \
    --mount=type=secret,id=nuget_password \
    dotnet nuget add source https://nuget.pkg.github.com/lokixx15/index.json \
        -n github \
        --username $(cat /run/secrets/nuget_username) \
        --password $(cat /run/secrets/nuget_password) \
        --store-password-in-clear-text

COPY ["DirectoryService/src/DirectoryService.Presentation/DirectoryService.Presentation.csproj", "DirectoryService/src/DirectoryService.Presentation/"]
COPY ["DirectoryService/src/DirectoryService.Application/DirectoryService.Application.csproj", "DirectoryService/src/DirectoryService.Application/"]
COPY ["DirectoryService/src/DirectoryService.Contracts/DirectoryService.Contracts.csproj", "DirectoryService/src/DirectoryService.Contracts/"]
COPY ["DirectoryService/src/DirectoryService.Domain/DirectoryService.Domain.csproj", "DirectoryService/src/DirectoryService.Domain/"]
COPY ["DirectoryService/src/DirectoryService.Infrastructure/DirectoryService.Infrastructure.csproj", "DirectoryService/src/DirectoryService.Infrastructure/"]
RUN dotnet restore "./DirectoryService/src/DirectoryService.Presentation/DirectoryService.Presentation.csproj"

RUN dotnet tool install --global dotnet-ef --version 9.0.0
ENV PATH="/root/.dotnet/tools:${PATH}"

WORKDIR "/src"

COPY ["DirectoryService", "DirectoryService/"]

RUN dotnet ef migrations bundle \
	--project "DirectoryService/src/DirectoryService.Infrastructure/DirectoryService.Infrastructure.csproj" \
	--startup-project "DirectoryService/src/DirectoryService.Presentation/DirectoryService.Presentation.csproj" \
	--output /app/efbundle \
	--configuration Release \
	--verbose \
	--self-contained

WORKDIR "/src/DirectoryService/src/DirectoryService.Presentation"
RUN dotnet build "./DirectoryService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DirectoryService.Presentation.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build /app/efbundle .
RUN chmod +x efbundle

USER $APP_UID

ENTRYPOINT ["dotnet", "DirectoryService.Presentation.dll"]