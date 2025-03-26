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
        private Button _backButton;
        private Button _moveUnitsButton;
        private Button _executeQueryButton;
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
            _backButton = root.Q<Button>("back-button");
            _moveUnitsButton = root.Q<Button>("move-units-button");
            _executeQueryButton = root.Q<Button>("execute-query-button");
            _queryTextField = root.Q<TextField>("query-input");
            _taskDescriptionLabel = root.Q<Label>("task-description");
        }
        
        private void RegisterEventHandlers()
        {
            if (_backButton != null)
            {
                _backButton.RegisterCallback<ClickEvent>(OnBackButtonClicked);
            }
            
            if (_moveUnitsButton != null)
            {
                _moveUnitsButton.RegisterCallback<ClickEvent>(OnMoveUnitsButtonClicked);
            }
            
            if (_executeQueryButton != null)
            {
                _executeQueryButton.RegisterCallback<ClickEvent>(OnExecuteQueryButtonClicked);
            }
            
            EventManager.Instance.Subscribe(SqlEventType.QueryValidated, OnQueryValidated);
        }
        
        private void OnBackButtonClicked(ClickEvent evt)
        {
            evt.StopPropagation();
            EventManager.Instance.TriggerEvent(NavigationEventType.ToSqlMenu);
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
        
        private void OnExecuteQueryButtonClicked(ClickEvent evt)
        {
            evt.StopPropagation();
            if (!_isProcessingClick && _queryTextField != null)
            {
                _isProcessingClick = true;
                ExecuteQuery(_queryTextField.value);
                StartCoroutine(ResetClickFlag());
            }
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
            if (_backButton != null)
            {
                _backButton.UnregisterCallback<ClickEvent>(OnBackButtonClicked);
            }
            
            if (_moveUnitsButton != null)
            {
                _moveUnitsButton.UnregisterCallback<ClickEvent>(OnMoveUnitsButtonClicked);
            }
            
            if (_executeQueryButton != null)
            {
                _executeQueryButton.UnregisterCallback<ClickEvent>(OnExecuteQueryButtonClicked);
            }
            
            EventManager.Instance.Unsubscribe(SqlEventType.QueryValidated, OnQueryValidated);
        }
        
        private void OnDestroy()
        {
            CharacterVisualizationService.Instance.DestroyAllCharacters();
        }
    }
}