# BookHub - Plateforme de Gestion de Bibliothèque Numérique

## Présentation

BookHub est une plateforme de gestion de bibliothèque numérique développée avec .NET 8 et Blazor WebAssembly.  
Ce projet a été réalisé dans le cadre d’un **ECF CDA**, avec pour objectif de compléter un projet existant et de documenter l’ensemble.

Le projet met en œuvre :
- Une architecture microservices avec **CatalogService**, **UserService**, et **LoanService**
- Une interface utilisateur **Blazor WebAssembly**
- Une gestion complète des emprunts de livres avec règles métiers

---

## Fonctionnalités Réalisées

### Backend
- Correction des ports des différents services
- Ajout des migrations dans chacun des services
- Modification de la fonctionnalité de création d'utilisateurs afin de pouvoir ajouter des utilisateurs aux rôles **"Admin"** et **"Librarian"**
- Service **LoanService** implémenté
  - Création, consultation et retour des emprunts
  - Règles métiers associées aux emprunts appliquées (limite maximale, calcul de retard...)
- Communication inter-services via HTTP avec CatalogService et UserService

*Note : bien que fonctionnelle techniquement, la création d'emprunts n'est pas possible depuis le frontend en raison d'un souci de gestion des JWT pour accéder aux informations de l'utilisateur connecté*

### Frontend Blazor
- Pages réalisées :
  - Détail d’un livre
  - Mes emprunts
  - Tableau de bord administrateur

## Fonctionnalités partiellement réalisées
Certaines fonctionnalités n'ont pu être terminées à temps, et ne sont donc pas fonctionnelles. Afin de garder une trace de mon travail effectué sur ces fonctionnalités, celles-ci sont disponibles sur des [**branches git**](https://github.com/Morgan-B3/ECF3-Morgan-Boutrois/branches) annexes.

### Branche "[gateway](https://github.com/Morgan-B3/ECF3-Morgan-Boutrois/tree/gateway)"
- Ajout d'une API Gateway permettant de centraliser les appels de Blazor en un seul endpoint.

### Branche "[env-variables](https://github.com/Morgan-B3/ECF3-Morgan-Boutrois/tree/env-variables)"
- Création de fichiers .env afin de sécuriser les données secrètes (clé secrète JWT, identifiants postgres...).
- Mise à jour de fichiers utilisant ces données en conséquence (docker-compose, Program.cs...)

---

## Livrables et Documentation

| Type | Lien / Localisation |
|------|-------------------|
| Architecture et structure du projet | [ARCHITECTURE.md](BookHub/docs/ARCHITECTURE.md) |
| Référence des endpoints API | [API_REFERENCE.md](BookHub/docs/API_REFERENCE.md) |
| Guide de déploiement | [DEPLOYMENT.md](BookHub/docs/DEPLOYMENT.md) |
| Guide de contribution | [CONTRIBUTING.md](BookHub/docs/CONTRIBUTING.md) |
| Modèle conceptuel de données | [MCD.png](/UML/Bookhub_MCD.png) |
| Modèle logique de données | [MLD.png](/UML/Bookhub_MLD.png) |

---

## Accès aux services
Après [démarrage du projet](BookHub/docs/DEPLOYMENT.md), ses différents services sont disponibles aux adresses suivantes :

| Service | URL |
|---------|-----|
| Frontend Blazor | http://localhost:8080 |
| CatalogService | http://localhost:5001 |
| UserService | http://localhost:5002 |
| LoanService | http://localhost:5003 |

---
