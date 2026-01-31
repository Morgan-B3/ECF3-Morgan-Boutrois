# Architecture de BookHub

## 1. Vue d’ensemble

BookHub est une plateforme de gestion de bibliothèque numérique construite selon une **architecture microservices** et reposant sur les principes de l’**architecture hexagonale (Ports & Adapters)**.

L’application est développée avec :
- **.NET 8** pour les services backend,
- **Blazor WebAssembly** pour le frontend,
- **PostgreSQL** pour la persistance des données,
- **Docker et Docker Compose** pour la conteneurisation et l’orchestration.

### Principes architecturaux clés

L’architecture de BookHub repose sur les principes suivants :

- **Microservices indépendants** : chaque domaine métier est géré par un service dédié.
- **Architecture hexagonale (Ports & Adapters)** pour une meilleure maintenabilité et testabilité.
- **Base de données dédiée par service** afin d’éviter le couplage.
- **Communication inter-services via HTTP REST**.
- **Déploiement conteneurisé avec Docker Compose**.

---

## 2. Architecture globale

Vue simplifiée des composants et de leurs interactions :
```
┌───────────────────────────────────────────┐
│            Blazor WebAssembly             │
│            (Frontend Client)              │
└─────────────────────┬─────────────────────┘
                      │
                      ▼
┌─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─ ─┐
│             ( API Gateway )               │
│             ( Port: 5000  )               │
└─ ─ ┬ ─ ─ ─ ─ ─ ─ ─ ─┬─ ─ ─ ─ ─ ─ ─ ┬ ─ ─ ─┘
     │                │              │
     ▼                ▼              ▼
┌──────────┐    ┌──────────┐    ┌──────────┐   
│ Catalog  │    │  User    │    │  Loan    │    
│ Service  │    │ Service  │    │ Service  │   
│ :5001    │    │ :5002    │    │ :5003    │    
└────┬─────┘    └────┬─────┘    └────┬─────┘    
     │               │               │ 
     └───────────────┴───────────────┘
                     │
                     ▼
        ┌────────────────────────┐
        │  PostgreSQL / RabbitMQ │
        └────────────────────────┘
```
*Note: L'API Gateway, censée être présente dans le projet, n'a pu être mise en place. Une version du projet utilisant une API Gateway est disponible sur la branche "gateway", mais celle-ci n'est pas fonctionnelle*

Arborescence du projet :

```
BookHub/
├── src/
│   ├── Services/
│   │   ├── BookHub.CatalogService/ # Service de gestion du catalogue
│   │   │   ├── Api/                # Controllers
│   │   │   ├── Application/        # Services applicatifs
│   │   │   ├── Domain/             # Entites et Ports
│   │   │   └── Infrastructure/     # Implementations (Persistence)
│   │   ├── BookHub.UserService/    # Service de gestion des utilisateurs
│   │   │   ├── Api/                # Controllers
│   │   │   ├── Application/        # Services applicatifs
│   │   │   ├── Domain/             # Entites et Ports
│   │   │   └── Infrastructure/     # Implementations (Persistence, Security)
│   │   └── BookHub.LoanService/    # Service de gestion des emprunts
│   │       ├── Api/                # Controllers
│   │       ├── Application/        # Services applicatifs
│   │       ├── Domain/             # Entites et Ports
│   │       └── Infrastructure/     # Implementations (Persistence, HttpClients)
│   ├── Web/
│   │   └── BookHub.BlazorClient/   # Application Blazor WASM
│   └── Shared/
│       └── BookHub.Shared/         # DTOs et contrats partages
├── docs/                           # Documentation
├── docker-compose.yml              # Orchestration
└── README.md
```


---

## 3. Composants principaux

### 3.1 Frontend – Blazor WebAssembly

L’application frontend :

- S’exécute directement dans le navigateur de l’utilisateur.
- Communique avec les microservices via des requêtes HTTP.
- Consomme des APIs REST exposées par les services backend.
- Gère l’interface utilisateur, l’authentification et l’expérience utilisateur.

*Note: sur la branche "gateway", le frontend communique exclusivement avec le gateway*

---

### 3.2 Microservices Backend

BookHub est structuré autour de **trois microservices principaux** :

#### 📚 CatalogService (Service du catalogue)

**Responsabilités :**
- Gestion du catalogue de livres (ajout, modification, consultation).
- Gestion du stock et de la disponibilité des livres.
- Exposition d’une API REST pour accéder aux données du catalogue.

**Persistance :**
- Base de données : **PostgreSQL – bookhub_catalog**

---

#### 👤 UserService (Service utilisateurs)

**Responsabilités :**
- Gestion des utilisateurs (création, mise à jour, suppression).
- Gestion de l’authentification et de l’autorisation via **JWT**.
- Délivrance de tokens JWT après connexion.

**Persistance :**
- Base de données : **PostgreSQL – bookhub_users**

---

#### 📖 LoanService (Service des emprunts)

**Responsabilités :**
- Gestion des emprunts de livres.
- Application des règles métier (durée, limite d’emprunts, pénalités).
- Communication avec :
  - **CatalogService** pour vérifier la disponibilité des livres,
  - **UserService** pour vérifier l’existence des utilisateurs.

**Persistance :**
- Base de données : **PostgreSQL – bookhub_loans**

---

## 4. Architecture Hexagonale (Ports & Adapters)

Chaque microservice suit strictement l’architecture hexagonale, structurée en quatre couches principales :

```
Service/
├── Domain/              # Cœur métier (aucune dépendance externe)
│   ├── Entities/        # Entités du domaine
│   └── Ports/           # Interfaces (contrats)
├── Application/         # Cas d'utilisation
│   └── Services/        # Services applicatifs
├── Infrastructure/      # Implementations des ports
│   ├── Persistence/     # DbContext, Repositories
│   ├── HttpClients/     # Clients HTTP pour communication inter-services
│   └── Security/        # Implémentations sécurité (JWT, hashing)
└── Api/                 # Point d'entrée
    └── Controllers/     # Controllers REST
```

### Rôle de chaque couche

| Couche | Responsabilité |
|--------|----------------|
| **Domain** | Contient les règles métier et entités, sans dépendance technique. |
| **Application** | Implémente les cas d’usage et orchestre le domaine. |
| **Infrastructure** | Contient les implémentations concrètes (BD, HTTP, sécurité). |
| **API** | Expose les fonctionnalités via des controllers REST. |

Cette séparation permet :
- Une meilleure testabilité,
- Un fort découplage entre métier et technique,
- Une évolution plus facile du projet.

---

## 5. Communication entre services

Les microservices communiquent entre eux via **HTTP REST** :

### LoanService → CatalogService
- Vérifier si un livre est disponible.
- Mettre à jour le stock après un emprunt ou un retour.

### LoanService → UserService
- Vérifier si l’utilisateur existe.
- Valider les informations utilisateur.

---

## 6. Modèles de données
### Modèle conceptuel de données
![MCD](/UML/Bookhub_MCD.png)
### Modèle logique de données
![MLD](/UML/Bookhub_MLD.png)
---

## 7. Choix techniques et justification

| Technologie | Justification |
|-------------|---------------|
| **.NET 8** | Performance, modernité, support LTS, adapté aux microservices. |
| **Blazor WebAssembly** | Application web moderne sans JavaScript lourd. |
| **PostgreSQL** | Base robuste, performante et adaptée aux applications cloud. |
| **Docker & Docker Compose** | Facilite le déploiement et garantit un environnement homogène. |
| **JWT** | Standard sécurisé pour l’authentification stateless. |
| **Architecture hexagonale** | Favorise la maintenabilité et les tests. |

---