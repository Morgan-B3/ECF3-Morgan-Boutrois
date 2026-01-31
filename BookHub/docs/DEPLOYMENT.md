# Guide de déploiement – BookHub

Ce document décrit les différentes façons de démarrer, configurer et exploiter l’application BookHub, en environnement de développement local ainsi qu’en environnement conteneurisé via Docker Compose.

---

## 1. Prérequis

Avant de démarrer l’application, assurez-vous d’avoir installé :

- Docker Desktop (incluant Docker Compose)
- .NET 8 SDK (si exécution en mode développement local)
- (Optionnel) Git pour cloner le projet

Vérifications rapides :

```
docker --version
docker compose version
dotnet --version
```

---

## 2. Modes de démarrage

BookHub peut être exécuté selon deux modes principaux :

| Mode | Description |
|------|-------------|
| dev | Démarre uniquement l’infrastructure (PostgreSQL + RabbitMQ). Les services .NET sont lancés manuellement. |
| prod | Démarre l’ensemble des services via Docker Compose (frontend + backend + infrastructure). |

---

## 3. Démarrage via Docker Compose (recommandé)

### 3.1 Démarrer tout le projet (mode production)

À la racine du projet, exécutez :

```
docker compose up -d --build
```

Cela démarre automatiquement :

- PostgreSQL  
- RabbitMQ  
- CatalogService  
- UserService  
- LoanService  
- Blazor WebAssembly Client  

### 3.2 Arrêter l’application

```
docker compose down
```

### 3.3 Voir les logs en temps réel

```
docker compose logs -f
```

---

## 4. Démarrage via le script `start.sh`

Un script de démarrage est fourni à la racine du projet : `start.sh`.

### 4.1 Mode développement (infrastructure seule)

```
./start.sh dev
```

Cela :

- Démarre PostgreSQL et RabbitMQ via Docker
- N’exécute PAS les services .NET ni le frontend

Une fois l’infrastructure prête, vous devez lancer manuellement chaque service dans un terminal séparé :

```
cd src/Services/BookHub.CatalogService && dotnet run
cd src/Services/BookHub.UserService && dotnet run
cd src/Services/BookHub.LoanService && dotnet run
cd src/Gateway/BookHub.ApiGateway && dotnet run
cd src/Web/BookHub.BlazorClient && dotnet run
```

### 4.2 Mode production (tout en Docker)

```
./start.sh prod
```

Cela équivaut à :

```
docker compose up -d --build
```

---

## 5. Adresses des services

Une fois le projet lancé (via Docker Compose) :

| Service | URL |
|--------|-----|
| BookHub (Blazor) | http://localhost:8080 |
| CatalogService | http://localhost:5001 |
| UserService | http://localhost:5002 |
| LoanService | http://localhost:5003 |
| API Gateway (si activée) | http://localhost:5000 |
| RabbitMQ | http://localhost:15672 |

La liste des endpoints est disponible dans le fichier [API_REFERENCE.md](API_REFERENCE.md)

### Swagger

Chaque service expose Swagger en mode développement ( /swagger ) :

- CatalogService : http://localhost:5001/swagger  
- UserService : http://localhost:5002/swagger  
- LoanService : http://localhost:5003/swagger  

### Health Checks
Chaque service expose un endpoint de santé ( /health ) :

- CatalogService : http://localhost:5001/health  
- UserService : http://localhost:5002/health  
- LoanService : http://localhost:5003/health  

---

## 6. Configuration via `.env` (gestion des secrets)

Sur la branche "env-variables", l’application est conçue pour s’appuyer sur un fichier `.env` à la racine du projet afin de stocker :

- Identifiants PostgreSQL  
- Paramètres des bases de données  
- Clé secrète JWT  
- URLs internes des services  
- Configuration RabbitMQ  

### Exemple de `.env` recommandé

```
# ==========================
# PostgreSQL
# ==========================
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
POSTGRES_HOST=postgres
POSTGRES_PORT=5432

CATALOG_DB=bookhub_catalog
USER_DB=bookhub_users
LOAN_DB=bookhub_loans

# ==========================
# JWT (Sécurité)
# ==========================
JWT_SECRET_KEY=BookHubSuperSecretKeyThatIsAtLeast32CharactersLong!
JWT_ISSUER=BookHub
JWT_AUDIENCE=BookHubUsers
JWT_EXPIRATION_HOURS=24

# ==========================
# Services internes (Docker)
# ==========================
CATALOG_SERVICE_URL=http://catalog-service:8080
USER_SERVICE_URL=http://user-service:8080

# ==========================
# RabbitMQ
# ==========================
RABBITMQ_HOST=rabbitmq
RABBITMQ_PORT=5672
RABBITMQ_USER=guest
RABBITMQ_PASSWORD=guest
```

### Sécurité

- Aucune clé secrète ne doit être commitée dans Git en production.
- Le `.env` doit idéalement être généré par l’utilisateur ou fourni par un gestionnaire de secrets (Docker secrets, Azure Key Vault, etc.).

---

## 7. Persistance des données

Docker Compose utilise des volumes persistants pour PostgreSQL afin d’éviter la perte de données entre redémarrages :

```
volumes:
  postgres_data:
```

Pour supprimer complètement les données locales :

```
docker compose down -v
```

---

## 8. Dépannage (problèmes courants)

### 8.1 PostgreSQL ne démarre pas

Vérifiez que le port 5432 n’est pas déjà utilisé :

```
netstat -ano | findstr :5432
```

Ou redémarrez proprement :

```
docker compose down
docker compose up -d
```

### 8.2 Erreur `JWT_SECRET_KEY` manquante

Assurez-vous que :

- Le fichier `.env` est bien présent à la racine du projet  
- Docker Compose charge bien le `.env`  
- La variable est bien définie :

```
echo $JWT_SECRET_KEY
```

### 8.3 Blazor ne charge pas les données

Vérifiez :

- Que les services backend tournent bien sur 5001, 5002 et 5003  
- Que CORS autorise `http://localhost:8080` dans `Program.cs`

---

## 9. Résumé rapide

| Action | Commande |
|--------|---------|
| Démarrer tout | `docker compose up -d --build` |
| Arrêter tout | `docker compose down` |
| Voir logs | `docker compose logs -f` |
| Mode dev (infra seule) | `./start.sh dev` |
| Mode prod (tout Docker) | `./start.sh prod` |

---