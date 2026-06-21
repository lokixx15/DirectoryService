FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 9001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG NUGET_USERNAME
ARG NUGET_PASSWORD
WORKDIR /src

COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]
	
RUN dotnet nuget add source https://nuget.pkg.github.com/lokixx15/index.json \
    -n github \
    --username $NUGET_USERNAME \
    --password $NUGET_PASSWORD \
    --store-password-in-clear-text

COPY ["src/DirectoryService.Presentation/DirectoryService.Presentation.csproj", "src/DirectoryService.Presentation/"]
COPY ["src/DirectoryService.Application/DirectoryService.Application.csproj", "src/DirectoryService.Application/"]
COPY ["src/DirectoryService.Contracts/DirectoryService.Contracts.csproj", "src/DirectoryService.Contracts/"]
COPY ["src/DirectoryService.Domain/DirectoryService.Domain.csproj", "src/DirectoryService.Domain/"]
COPY ["src/DirectoryService.Infrastructure/DirectoryService.Infrastructure.csproj", "src/DirectoryService.Infrastructure/"]
RUN dotnet restore "./src/DirectoryService.Presentation/DirectoryService.Presentation.csproj"

RUN dotnet tool install --global dotnet-ef --version 9.0.0
ENV PATH="/root/.dotnet/tools:${PATH}"

WORKDIR "/src"

COPY . .

RUN dotnet ef migrations bundle \
	--project "src/DirectoryService.Infrastructure/DirectoryService.Infrastructure.csproj" \
	--startup-project "src/DirectoryService.Presentation/DirectoryService.Presentation.csproj" \
	--output /app/efbundle \
	--configuration Release \
	--verbose \
	--self-contained

WORKDIR "/src/src/DirectoryService.Presentation"
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