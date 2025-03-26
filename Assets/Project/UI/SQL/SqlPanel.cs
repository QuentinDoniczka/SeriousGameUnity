using UnityEngine;
using UnityEngine.UIElements;
using Project.Core.Events;
using Project.Core.Service;
using Project.Game.Characters.Services;
using System.Collections;
using System.Collections.Generic;
using Project.Database.Models;
using Project.Game.Characters;

namespace Project.UI.SQL
{ 
    public class SqlPanel : MonoBehaviour
    {
        [SerializeField] private GameObject spawnZone;
        [SerializeField] private GameObject resultZone;
        
        private UIDocument _document;
        private Button _moveUnitsButton;
        private Button _moveBackButton;
        private Button _hintButton;
        private Button _prevButton;
        private Button _nextButton;
        private Button _exitButton;
        private TextField _queryTextField;
        private Label _taskDescriptionLabel;
        
        private bool _isProcessingClick = false;
        private LevelData _levelData;
        
        private void Awake()
        {
            FindSafeZone();
            CharacterVisualizationService.Instance.SetParentTransform(transform);
        }
        
        private void Start()
        {
            _levelData = ServiceManager.Instance.Sql.GetCurrentLevelData();
            if (_levelData != null)
            {
                InitializeCharacters();
                UpdateTaskDescription();
                ExecuteMageQuery();
            }
        }
        
        private void ExecuteMageQuery()
        {
            string mageQuery = "SELECT * FROM army WHERE role = 'mage'";
            Debug.Log($"Exécution de la requête: {mageQuery}");
            
            CharacterDataService.Instance.ExecuteCharacterQuery(mageQuery, mageResults => {
                Debug.Log($"Mages trouvés: {mageResults.Count}");
                
                foreach (var mage in mageResults)
                {
                    Debug.Log($"Mage: {mage.Name}, Faction: {mage.Faction}, Rôle: {mage.Role}");
                }
            });
        }
        
        private void InitializeCharacters()
        {
            CharacterDataService.Instance.LoadAllCharacters();
            if (CharacterDataService.Instance.HasCharacters())
            {
                CharacterVisualizationService.Instance.DestroyAllCharacters();
                CharacterVisualizationService.Instance.InstantiateCharacters(
                    CharacterDataService.Instance.GetAllCharacters(),
                    spawnZone
                );
            }
        }
        
        private void FindSafeZone()
        {
            if (spawnZone == null)
            {
                spawnZone = GameObject.Find("safe zone");
            }
            
            if (resultZone == null)
            {
                resultZone = GameObject.Find("result zone");
            }
        }
        
        private void UpdateTaskDescription()
        {
            if (_taskDescriptionLabel == null || _levelData?.tasks == null || _levelData.tasks.Count == 0) return;
            
            var currentTask = _levelData.tasks[0];
            _taskDescriptionLabel.text = $"{currentTask.name}: {currentTask.description}";
        }
        
        private void OnEnable()
        {
            InitializeUI();
            RegisterEventHandlers();
        }
        
        private void InitializeUI()
        {
            _document = GetComponent<UIDocument>();
            if (_document == null) return;
            
            var root = _document.rootVisualElement;
            // Boutons d'action
            _moveUnitsButton = root.Q<Button>("move-units-button");
            _moveBackButton = root.Q<Button>("move-back-button");
            
            // Boutons de navigation
            _hintButton = root.Q<Button>("hint-button");
            _prevButton = root.Q<Button>("prev-button");
            _nextButton = root.Q<Button>("next-button");
            _exitButton = root.Q<Button>("exit-button");
            
            // Autres éléments UI
            _queryTextField = root.Q<TextField>("query-input");
            _taskDescriptionLabel = root.Q<Label>("task-description");
        }
        
        private void RegisterEventHandlers()
        {
            if (_moveUnitsButton != null)
            {
                _moveUnitsButton.RegisterCallback<ClickEvent>(OnMoveUnitsButtonClicked);
            }
            
            if (_moveBackButton != null)
            {
                _moveBackButton.RegisterCallback<ClickEvent>(OnMoveBackButtonClicked);
            }
            
            if (_hintButton != null)
            {
                _hintButton.RegisterCallback<ClickEvent>(OnHintButtonClicked);
            }
            
            if (_prevButton != null)
            {
                _prevButton.RegisterCallback<ClickEvent>(OnPrevButtonClicked);
            }
            
            if (_nextButton != null)
            {
                _nextButton.RegisterCallback<ClickEvent>(OnNextButtonClicked);
            }
            
            if (_exitButton != null)
            {
                _exitButton.RegisterCallback<ClickEvent>(OnExitButtonClicked);
            }
            
            EventManager.Instance.Subscribe(SqlEventType.QueryValidated, OnQueryValidated);
        }
        
        private void OnMoveUnitsButtonClicked(ClickEvent evt)
        {
            evt.StopPropagation();
            if (!_isProcessingClick)
            {
                _isProcessingClick = true;
                CharacterVisualizationService.Instance.MoveCharactersToZone(resultZone);
                StartCoroutine(ResetClickFlag());
            }
        }
        
        private void OnMoveBackButtonClicked(ClickEvent evt)
        {
            evt.StopPropagation();
            if (!_isProcessingClick)
            {
                _isProcessingClick = true;
                CharacterVisualizationService.Instance.MoveCharactersToStartPositions(spawnZone);
                StartCoroutine(ResetClickFlag());
            }
        }
        
        private void OnHintButtonClicked(ClickEvent evt)
        {
            evt.StopPropagation();
            Debug.Log("Hint button clicked");
            // Logique d'indice à implémenter
        }
        
        private void OnPrevButtonClicked(ClickEvent evt)
        {
            evt.StopPropagation();
            Debug.Log("Previous button clicked");
            // Logique de tâche précédente à implémenter
        }
        
        private void OnNextButtonClicked(ClickEvent evt)
        {
            evt.StopPropagation();
            Debug.Log("Next button clicked");
            // Logique de tâche suivante à implémenter
        }
        
        private void OnExitButtonClicked(ClickEvent evt)
        {
            evt.StopPropagation();
            Debug.Log("Exit button clicked");
            EventManager.Instance.TriggerEvent(NavigationEventType.ToSqlMenu);
        }
        
        private void OnQueryValidated()
        {
            // Espace réservé pour une logique future si nécessaire
        }
        
        private void ExecuteQuery(string query)
        {
            if (string.IsNullOrEmpty(query)) return;
            
            CharacterDataService.Instance.ExecuteCharacterQuery(query, queryResults => {
                DisplayQueryResults(queryResults);
                EventManager.Instance.TriggerEvent(SqlEventType.QueryValidated);
            });
        }
        
        private void DisplayQueryResults(List<CharacterData> queryResults)
        {
            if (queryResults.Count == 0) return;
            
            if (resultZone != null)
            {
                CharacterVisualizationService.Instance.DestroyAllCharacters();
                CharacterVisualizationService.Instance.InstantiateCharacters(queryResults, resultZone);
            }
        }
        
        private IEnumerator ResetClickFlag()
        {
            yield return new WaitForSeconds(0.2f);
            _isProcessingClick = false;
        }
        
        private void OnDisable()
        {
            UnregisterEventHandlers();
        }
        
        private void UnregisterEventHandlers()
        {
            if (_moveUnitsButton != null)
            {
                _moveUnitsButton.UnregisterCallback<ClickEvent>(OnMoveUnitsButtonClicked);
            }
            
            if (_moveBackButton != null)
            {
                _moveBackButton.UnregisterCallback<ClickEvent>(OnMoveBackButtonClicked);
            }
            
            if (_hintButton != null)
            {
                _hintButton.UnregisterCallback<ClickEvent>(OnHintButtonClicked);
            }
            
            if (_prevButton != null)
            {
                _prevButton.UnregisterCallback<ClickEvent>(OnPrevButtonClicked);
            }
            
            if (_nextButton != null)
            {
                _nextButton.UnregisterCallback<ClickEvent>(OnNextButtonClicked);
            }
            
            if (_exitButton != null)
            {
                _exitButton.UnregisterCallback<ClickEvent>(OnExitButtonClicked);
            }
            
            EventManager.Instance.Unsubscribe(SqlEventType.QueryValidated, OnQueryValidated);
        }
        
        private void OnDestroy()
        {
            CharacterVisualizationService.Instance.DestroyAllCharacters();
        }
    }
}