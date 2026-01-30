# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copier les fichiers projet
COPY ["src/Gateway/BookHub.ApiGateway/BookHub.ApiGateway.csproj", "src/Gateway/BookHub.ApiGateway/"]
COPY ["src/Shared/BookHub.Shared/BookHub.Shared.csproj", "src/Shared/BookHub.Shared/"]

# Restaurer les dépendances
RUN dotnet restore "src/Gateway/BookHub.ApiGateway/BookHub.ApiGateway.csproj"

# Copier le reste du code
COPY . .

# Build
WORKDIR "/src/src/Gateway/BookHub.ApiGateway"
RUN dotnet build "BookHub.ApiGateway.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "BookHub.ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Créer un utilisateur non-root pour la sécurité
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Copier les fichiers publiés
COPY --from=publish /app/publish .

# Exposer le port sur lequel le Gateway tourne
EXPOSE 5000

# Démarrer l'application
ENTRYPOINT ["dotnet", "BookHub.ApiGateway.dll"]