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
│   │       ├── GameEventManager.cs         # Gestionnaire des events
│   │       └── GameEvents.cs               # Définition des events
│   │
│   ├── Database/               
│   │   ├── Models/                    
│   │   │   └── TableSchema.cs         
│   │   │
│   │   ├── Services/                  
│   │   │   ├── JsonSchemaAnalyzer.cs  
│   │   │   └── SqlTableBuilder.cs     
│   │   │
│   │   ├── SQLManager.cs             
│   │   └── QueryValidator.cs         
│   │
    │   ├── Game/                           # Logique de jeu
│   │   └── Managers/
│   │       ├── GameManager.cs              # Manager principal
│   │
│   ├── UI/                                 # Interface utilisateur
│   │   ├── HUD/
│   │   │   ├── HudSqlManager.cs          # Gestionnaire global du HUD SQL
│   │   │   └── SqlQueryData.cs             # Structure de données pour les requêtes SQL
│   │   │
│   │   ├── SQL/                            # dossier pour les UI SQL
│   │   │   ├── SqlPanel.cs                 # Panel SQL
│   │   │   ├── SqlHud.uxml                 # Structure UI du HUD SQL 
│   │   │   └── SqlHud.uss                  # Styles du HUD SQL
│   │   │
│   │   ├── Menu/                           # Menu
│   │   │   ├── MainMenuPanel.cs            # Menu principal
│   │   │   └── SqlMenuPanel.cs             # Menu SQL
│   │   │   
│   │   └── Auth/                           # Nouveau dossier pour les UI d'auth
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
│       ├── Auth/                           # Nouveau dossier pour l'authentification
│       │   ├── Login.unity                 # Scène de connexion
│       │   └── Register.unity              # Scène d'inscription
│       │
│       └── Levels/                         # Niveaux de jeu
│           └── Sql.unity                   # Niveau SQL
│
├── Resources/                              # Assets chargés dynamiquement
│   └── LevelData/                          # Données des niveaux
│       └── Sql/                            # Données du niveau SQL
│           ├── SqlTestLevelData.json       # Données du niveau de test SQL
│           └── SqlLevelData.json
│
└── Plugins/                                # Plugins externes
   └── SQLite/                              # DLL SQLite pour Unity
       └── Mono.Data.Sqlite.dll

on oublira pas l'intégration de paterne comme le patern observation