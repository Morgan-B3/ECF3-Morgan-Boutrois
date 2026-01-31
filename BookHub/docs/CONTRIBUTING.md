# Guide de contribution

Voici les bonnes pratiques à suivre pour pouvoir contribuer au projet :

## Règles de développement
- Rédiger des commits clairs, détaillant de façon concise le travail effectué
- Respecter l’architecture hexagonale
- Ne pas mettre de secrets en dur dans le code, ceux-ci doivent être placés dans des fichiers .env (eux-mêmes référencés dans le .gitignore)
- Rédiger des tests unitaires pour la logique métier

## Versioning et mise en commun du code
Chaque nouvelle feature doit être développée selon le schéma suivant :
1. Créer une nouvelle branche "feature/nom-de-la-feature"
2. Implémenter les changements (création de commits)
3. Ajouter des tests pour la feature
4. Ouvrir une Pull Request pour fusionner la branche (vers "develop" pour dev, vers "main" pour la prod)