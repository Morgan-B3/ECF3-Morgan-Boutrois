# Architecture de BookHub

## Vue d’ensemble

BookHub est une plateforme de gestion de bibliothèque numérique basée sur une architecture microservices, développée avec .NET 8 et Blazor WebAssembly.

L’architecture repose sur les principes suivants :
- Des microservices indépendants (Catalog, User, Loan)
- Une architecture hexagonale (Ports & Adapters)
- Une base de données par service
- Une Communication via HTTP REST
- Déploiement conteneurisé avec Docker Compose

## Architecture globale



## Architecture des microservices

Chaque microservice adopte une architecture hexagonale :

- Domain : règles métier et entités
- Application : cas d’usage
- Infrastructure : implémentations techniques
- Api : exposition REST

## Choix techniques

- .NET 8 pour le backend
- Blazor WebAssembly pour le frontend
- PostgreSQL pour la persistance
- Docker pour la conteneurisation