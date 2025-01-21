# Structure du Projet Unity - SQL Game

```
Assets/
│
├── _Project/                   
│   │
│   ├── Core/                           # Core systems
│   │   ├── CoreManager.cs              # Manager principal persistant
│   │   ├── Services/                   # Services system
│   │   │   └── ServiceManager.cs       # Central service manager
│   │   │
│   │   ├── Auth/                   # logique d'auth
│   │   │   ├── AuthManager.cs      # Gestion de l'authentification
│   │   │   ├── UserSession.cs      # Gestion de la session utilisateur
│   │   │   └── Models/
│   │   │       └── UserModel.cs    # Modèle de données utilisateur
│   │   │
│   │   
│   │   └── Events/                     # Système d'events
│   │       ├── GameEventManager.cs     # Gestionnaire des events (Observer pattern)
│   │       └── GameEvents.cs           # Définition des events
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
│   ├── Game/           # Logique de jeu
│   │   └── Managers/
│   │       ├── GameManager.cs   # Manager principal
│   │
│   ├── UI/                     # Interface utilisateur
│   │   ├── SQLInput/
│   │   │   ├── SQLInputField.cs     # Champ de saisie SQL
│   │   │   └── SQLSuggestions.cs    # Auto-complétion
│   │   │
│   │   ├── HUD/
│   │   │   ├── UnitDisplay.cs       # Affichage des unités
│   │   │   └── StatusPanel.cs       # État du jeu
│   │   │
│   │   ├── Prefabs/
│   │   │   ├── UIElements/          # Éléments d'interface
│   │   │   └── Windows/             # Fenêtres du jeu
│   │   │
│   │   └── Auth/                    # Nouveau dossier pour les UI d'auth
│   │       ├── LoginPanel.cs        # Panel de login
│   │       ├── LoginForm.cs         # Formulaire de login
│   │       ├── RegisterPanel.cs     # Panel d'inscription
│   │       └── RegisterForm.cs      # Formulaire d'inscription
│   │    
│   └── Scenes/                      # Navigation et scènes
│       ├── Managers/
│       │   └── SceneManager.cs      # Gestion de la navigation entre scènes
│       │
│       ├── Boot/                    # Scènes de démarrage
│       │   └── Logo.unity
│       │
│       ├── Menu/                    # Scènes de menus
│       │   └── MainMenu.unity
│       │
│       ├── Auth/                    # Nouveau dossier pour l'authentification
│       │   ├── Login.unity         # Scène de connexion
│       │   └── Register.unity      # Scène d'inscription
│       │
│       └── Levels/                  # Niveaux de jeu
│           ├── Level_01.unity
│           └── Level_02.unity
│
├── Resources/                  # Assets chargés dynamiquement
│   └── LevelData/             # Données des niveaux
│       └── level_test.json    # Fichier de test initial
│
└── Plugins/                   # Plugins externes
   └── SQLite/               # DLL SQLite pour Unity
       └── Mono.Data.Sqlite.dll

on oublira pas l'intégration de paterne comme le patern observation