# Structure du Projet Unity

```
Assets/
│
├── Project/                   
│   ├── Api/                                # Tout ce qui concerne les APIs
│   │   ├── ApiServiceManager.cs            # Gestionnaire des services API
│   │   ├── ApiConfig.cs                    # Config API
│   │   │
│   │   ├── Constants/                      # Constantes API
│   │   │   └── ValidationMessages.cs       # Messages de validation
│   │   │
│   │   ├── Endpoints/                      # Endpoints API
│   │   │   └── UserApiEndpoints.cs         # Endpoints utilisateur
│   │   │
│   │   ├── Validators/                     # Validations des APIs
│   │   │   ├── IApiValidator.cs            # Interface de validation
│   │   │   ├── UserNameValidator.cs        # Validation de nom d'utilisateur
│   │   │   ├── EmailValidator.cs           # Validation d'email
│   │   │   └── PasswordValidator.cs        # Validation de mot de passe
│   │   │
│   │   ├── Models/                         # DTOs
│   │   │   ├── Validators/                 # Validations des modèles
│   │   │   │   ├── IValidator.cs           # Interface de validation
│   │   │       ├── ValidationResult.cs     # Résultat de validation
│   │   │   │   ├── LoginValidator.cs       # Validation de login
│   │   │   │   └── RegisterValidator.cs    # Validation d'inscription
│   │   │   │
│   │   │   ├── Requests/                   # Modèles pour les requêtes API
│   │   │   │   ├── LoginRequest.cs
│   │   │   │   └── RegisterRequest.cs
│   │   │   │
│   │   │   └── Responses/                  # Modèles pour les réponses API
│   │   │       ├── LoginResponse.cs        # Réponse de login
│   │   │       └── RegisterResponse.cs     # Réponse d'inscription
│   │   │
│   │   └── User/                           # Services API utilisateur
│   │       ├── IUserApiService.cs 
│   │       └── UserApiService.cs
│   │
│   ├── Core/                               # Core systems
│   │   ├── CoreManager.cs                  # Manager principal persistant
│   │   │
│   │   ├── Services/                       # Services system
│   │   │   └── ServiceManager.cs           # Central service manager
│   │   │
│   │   └── Events/                         # Système d'events
│   │       ├── EventManager.cs             # Gestionnaire des events
│   │       ├── EventsType.cs               # Types d'events
│   │       └── NavigationParameters.cs     # Paramètres de navigation
│   │
│   ├── Database/                           # Système de base de données
│   │   ├── Models/                         # Modèles de données
│   │   │   ├── LevelDataModels.cs          # Modèles pour les données de niveau
│   │   │   └── TableSchema.cs              # Schéma de table SQL
│   │   │
│   │   ├── Services/                       # Services de base de données
│   │   │   ├── SqlConnectionService.cs     # Service de connexion SQL
│   │   │   ├── SqlDebugger.cs              # Service de débogage SQL
│   │   │   ├── SqlQueryService.cs          # Service d'exécution de requêtes SQL
│   │   │   ├── LevelDataService.cs         # Service de gestion des données de niveau
│   │   │   └── SqlTableBuilder.cs          # Service de création de tables SQL
│   │   │
│   │   ├── SqlManager.cs                   # Gestionnaire principal SQL (coordonne les services)
│   │   └── QueryValidator.cs               # Validation des requêtes SQL
│   │
│   ├── Game/                               # Logique de jeu
│   │   └── Managers/
│   │       ├── GameManager.cs              # Manager principal du jeu
│   │
│   ├── UI/                                 # Interface utilisateur
│   │   ├── HUD/
│   │   │   ├── HUD_SQL_Manager.cs          # Gestionnaire global du HUD SQL
│   │   │   └── SqlQueryData.cs             # Structure de données pour les requêtes SQL
│   │   │
│   │   ├── SQL/                            # Dossier pour les UI SQL
│   │   │   ├── SqlPanel.cs                 # Panel SQL
│   │   │   ├── sqlHud.uxml                 # Structure UI du HUD SQL 
│   │   │   └── sqlHud.uss                  # Styles du HUD SQL
│   │   │
│   │   ├── Menu/                           # Menu
│   │   │   ├── MainMenuPanel.cs            # Menu principal
│   │   │   └── SqlMenuPanel.cs             # Menu SQL
│   │   │   
│   │   └── Auth/                           # Dossier pour les UI d'authentification
│   │       ├── LoginPanel.cs               # Panel de login
│   │       └── RegisterPanel.cs            # Panel d'inscription
│   │    
│   └── Scenes/                             # Navigation et scènes
│       ├── Managers/
│       │   └── SceneManager.cs             # Gestion de la navigation entre scènes
│       │
│       ├── Boot/                           # Scènes de démarrage
│       │   └── Logo.unity
│       │
│       ├── Menu/                           # Scènes de menus
│       │   └── MainMenu.unity
│       │
│       ├── Auth/                           # Dossier pour l'authentification
│       │   ├── Login.unity                 # Scène de connexion
│       │   └── Register.unity              # Scène d'inscription
│       │
│       └── Levels/                         # Niveaux de jeu
│           └── SqlLevel.unity              # Niveau SQL
│
├── Resources/                              # Assets chargés dynamiquement
│   └── LevelData/                          # Données des niveaux
│       └── level_test.json                 # Données du niveau de test SQL
│
└── Plugins/                                # Plugins externes
    └── SQLite/                             # DLL SQLite pour Unity
        └── Mono.Data.Sqlite.dll
```

## Patterns de conception implémentés

1. **Singleton** - Utilisé pour les managers globaux comme `EventManager`, `SceneManager`, `SqlManager`
2. **Service Locator** - `ServiceManager` permet d'accéder aux services depuis n'importe où
3. **Observer** - Le système d'événements avec `EventManager` permet une communication découplée entre composants
4. **Stratégie** - Séparation des algorithmes dans différents services
5. **SOLID**
    - **Single Responsibility Principle** - Chaque service a une unique responsabilité
    - **Open/Closed Principle** - Le système est extensible sans modifier les classes existantes
    - **Dependency Inversion** - Dépendances injectées dans les constructeurs