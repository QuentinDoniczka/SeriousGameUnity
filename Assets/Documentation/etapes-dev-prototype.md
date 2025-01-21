# Étapes de Développement - Prototype SQL Game

## 1. Core System - Base de données et SQL

### 1.1 Structure de données
- Création de la classe Character :
  ```
  Attributs principaux :
  - ID
  - Nom
  - Position (x,y)
  - Company (1,2,3)
  - Stats (HP, Attaque, Défense)
  - État actuel (Idle, Moving, Action)
  ```

- Création de la classe Company :
  ```
  Attributs :
  - ID
  - Nom
  - Position base
  - Liste de characters
  - Ressources
  ```

### 1.2 Système SQL
- Développer le parser SQL basique :
  - SELECT (colonnes, WHERE, JOIN)
  - Validation des commandes
  - Système de réponse/feedback

## 2. Interface Utilisateur (Priorité haute)

### 2.1 Zone de commande SQL
- Input field adapté mobile
- Système d'auto-complétion basique
- Bouton d'exécution
- Zone d'affichage des résultats/erreurs

### 2.2 HUD Gameplay
- Affichage des unités sélectionnées
- Indicateurs d'état des companies
- Feedback visuel des commandes
- Mini-map (si nécessaire)

## 3. Système de Mouvement (Core Gameplay)

### 3.1 Gestion des Characters
- Système de spawn des unités
- Contrôleur de mouvement :
  - Pathfinding basique
  - Animation de déplacement
  - Gestion des collisions

### 3.2 Interprétation SQL vers Actions
- Système de mapping :
  ```
  SELECT -> Sélection d'unités
  WHERE -> Filtrage des unités
  JOIN -> Actions spéciales
  ```
- Feedback visuel des sélections
- Système d'action basé sur les requêtes

## 4. Prototype Niveau Test

### 4.1 Environment de Test
- Terrain simple 2D
- Placement des 3 companies
- Spawning initial des characters
- Objectifs basiques

### 4.2 Conditions de Test
- Requêtes SQL de base à tester :
  ```
  SELECT * FROM company1
  SELECT character WHERE company = 1
  SELECT * FROM company1 JOIN company2
  ```
- Feedback visuel pour chaque type de requête
- Système de validation des objectifs

## 5. Système de Feedback

### 5.1 Retour Visuel
- Highlighting des unités sélectionnées
- Effets de sélection/action
- Indicateurs de mouvement
- Messages de succès/erreur

### 5.2 Système d'Aide
- Messages d'erreur SQL clairs
- Suggestions de correction
- Aide contextuelle basique

## Ordre de Développement Recommandé

1. **Phase 1 - Base (2-3 jours)**
   - Mise en place projet Unity
   - Création classes de base (Character, Company)
   - Interface utilisateur basique

2. **Phase 2 - Parser SQL (3-4 jours)**
   - Système de parsing SQL simple
   - Validation des commandes basiques
   - Retour d'erreurs

3. **Phase 3 - Mouvement (3-4 jours)**
   - Spawn des characters
   - Système de mouvement
   - Gestion des collisions

4. **Phase 4 - Intégration (4-5 jours)**
   - Connexion Parser SQL -> Actions
   - Feedback visuel
   - Test et debug

Total pour prototype fonctionnel : environ 2-3 semaines

## Tests Critiques Pour Validation

1. Entrée des commandes :
   - Test sur mobile
   - Validation des erreurs
   - Performance du parser

2. Mouvement :
   - Fluidité des déplacements
   - Gestion de multiples unités
   - Réponse aux commandes

3. Interface :
   - Lisibilité sur mobile
   - Facilité de saisie
   - Clarté du feedback