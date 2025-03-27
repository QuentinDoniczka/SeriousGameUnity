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
        private Button _moveBackButton;
        private Button _hintButton;
        private Button _prevButton;
        private Button _nextButton;
        private Button _exitButton;
        private Button _closeHintButton;
        private DropdownField _tableDropdown;
        private DropdownField _fieldDropdown;
        private DropdownField _operatorDropdown;
        private DropdownField _valueDropdown;
        private DropdownField _groupDropdown;
        private VisualElement _hintPopup;
        private VisualElement _whereContinuationRow;
        private Label _queryPreviewLabel;
        private Label _taskDescriptionLabel;
        private Label _hintTextLabel;
        
        private bool _isProcessingQuery = false;
        private LevelData _levelData;
        private List<int> _lastQueryResultIds = new List<int>();
        private int _currentTaskIndex = 0;
        
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
            }
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
            if (_taskDescriptionLabel == null || _levelData?.tasks == null || _levelData.tasks.Count <= _currentTaskIndex) return;
            
            var currentTask = _levelData.tasks[_currentTaskIndex];
            _taskDescriptionLabel.text = $"{currentTask.name}: {currentTask.description}";
            
            if (_hintTextLabel != null && !string.IsNullOrEmpty(currentTask.hint))
            {
                _hintTextLabel.text = currentTask.hint;
            }
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
            
            _tableDropdown = root.Q<DropdownField>("table-dropdown");
            _fieldDropdown = root.Q<DropdownField>("field-dropdown");
            _operatorDropdown = root.Q<DropdownField>("operator-dropdown");
            _valueDropdown = root.Q<DropdownField>("value-dropdown");
            _groupDropdown = root.Q<DropdownField>("group-dropdown");
            
            _whereContinuationRow = root.Q<VisualElement>("row-3b");
            
            _queryPreviewLabel = root.Q<Label>("query-preview");
            _taskDescriptionLabel = root.Q<Label>("task-description");
            _hintTextLabel = root.Q<Label>("hint-text");
            
            _hintPopup = root.Q<VisualElement>("hint-popup");
            
            _moveBackButton = root.Q<Button>("move-back-button");
            
            _hintButton = root.Q<Button>("hint-button");
            _prevButton = root.Q<Button>("prev-button");
            _nextButton = root.Q<Button>("next-button");
            _exitButton = root.Q<Button>("exit-button");
            _closeHintButton = root.Q<Button>("close-hint-button");
            
            if (_hintPopup != null)
            {
                _hintPopup.AddToClassList("hidden");
            }
            
            if (_whereContinuationRow != null && _fieldDropdown != null && _fieldDropdown.value == "--")
            {
                _whereContinuationRow.style.display = DisplayStyle.None;
            }
            
            InitializeDropdowns();
        }
        
        private void InitializeDropdowns()
        {
            if (_tableDropdown != null)
            {
                _tableDropdown.choices = new List<string> { "--", "army" };
                _tableDropdown.value = "--";
                _tableDropdown.RegisterValueChangedCallback(OnDropdownValueChanged);
            }
            
            if (_fieldDropdown != null)
            {
                _fieldDropdown.choices = new List<string> { "--", "id", "role" };
                _fieldDropdown.value = "--";
                _fieldDropdown.RegisterValueChangedCallback(OnFieldDropdownChanged);
            }
            
            if (_operatorDropdown != null)
            {
                _operatorDropdown.choices = new List<string> { "--", "=", ">", "<", ">=", "<=" };
                _operatorDropdown.value = "--";
                _operatorDropdown.RegisterValueChangedCallback(OnDropdownValueChanged);
            }
            
            if (_valueDropdown != null)
            {
                _valueDropdown.choices = new List<string> { "--" };
                _valueDropdown.value = "--";
                _valueDropdown.RegisterValueChangedCallback(OnDropdownValueChanged);
            }
            
            if (_groupDropdown != null)
            {
                _groupDropdown.choices = new List<string> { "--", "id ASC", "id DESC", "role ASC", "role DESC", "faction ASC", "faction DESC" };
                _groupDropdown.value = "--";
                _groupDropdown.RegisterValueChangedCallback(OnDropdownValueChanged);
            }
        }
        
        private void OnFieldDropdownChanged(ChangeEvent<string> evt)
        {
            if (_whereContinuationRow != null)
            {
                _whereContinuationRow.style.display = evt.newValue == "--" 
                    ? DisplayStyle.None 
                    : DisplayStyle.Flex;
            }
            
            UpdateValueDropdownOptions();
            OnDropdownValueChanged(evt);
        }
        
        private void UpdateValueDropdownOptions()
        {
            if (_valueDropdown == null || _fieldDropdown == null) return;
            
            List<string> options = new List<string> { "--" };
            
            switch (_fieldDropdown.value)
            {
                case "id":
                    for (int i = 1; i <= 16; i++)
                    {
                        options.Add(i.ToString());
                    }
                    break;
                    
                case "role":
                    options.AddRange(new[] { "soldier", "mage", "knight", "healer" });
                    break;
                    
                default:
                    break;
            }
            
            string currentValue = _valueDropdown.value;
            _valueDropdown.choices = options;
            
            if (options.Contains(currentValue))
            {
                _valueDropdown.value = currentValue;
            }
            else
            {
                _valueDropdown.value = "--";
            }
        }
        
        private void OnDropdownValueChanged(ChangeEvent<string> evt)
        {
            UpdateQueryPreview();
            
            if (evt.target == _tableDropdown && evt.newValue == "--")
            {
                CharacterVisualizationService.Instance.MoveCharactersToStartPositions(spawnZone);
                return;
            }
            
            TryExecuteQuery();
        }
        
        private void UpdateQueryPreview()
        {
            if (_queryPreviewLabel == null) return;
    
            string table = _tableDropdown.value == "--" ? "" : _tableDropdown.value;
            string whereClause = BuildWhereClause();
            string orderByClause = _groupDropdown.value == "--" ? "" : $" ORDER BY {_groupDropdown.value}";
    
            string query = "SELECT *";
            if (!string.IsNullOrEmpty(table))
            {
                query += $" FROM {table}{whereClause}{orderByClause}";
            }
    
            _queryPreviewLabel.text = query;
        }
        
        private string BuildWhereClause()
        {
            if (_fieldDropdown.value == "--")
            {
                return "";
            }
            
            if (_operatorDropdown.value == "--" || _valueDropdown.value == "--")
            {
                return " WHERE ...";
            }
            
            string fieldName = _fieldDropdown.value;
            string operatorSymbol = _operatorDropdown.value;
            string fieldValue = _valueDropdown.value;
            
            if (fieldName == "role")
            {
                fieldValue = $"'{fieldValue}'";
            }
            
            return $" WHERE {fieldName} {operatorSymbol} {fieldValue}";
        }
        
        private void TryExecuteQuery()
        {
            if (_isProcessingQuery) return;
            
            string table = _tableDropdown.value == "--" ? "" : _tableDropdown.value;
            
            if (!string.IsNullOrEmpty(table))
            {
                string whereClause = "";
                
                if (_fieldDropdown.value != "--" && _operatorDropdown.value != "--" && _valueDropdown.value != "--")
                {
                    whereClause = BuildWhereClause();
                }
                
                string orderByClause = _groupDropdown.value == "--" ? "" : $" ORDER BY {_groupDropdown.value}";
                string query = $"SELECT * FROM {table}{whereClause}{orderByClause}";
                
                _isProcessingQuery = true;
                ExecuteQuery(query);
                StartCoroutine(ResetQueryProcessingFlag());
            }
        }
        
        private IEnumerator ResetQueryProcessingFlag()
        {
            yield return new WaitForSeconds(0.5f);
            _isProcessingQuery = false;
        }
        
        private void RegisterEventHandlers()
        {
            if (_moveBackButton != null)
            {
                _moveBackButton.RegisterCallback<ClickEvent>(OnMoveBackButtonClicked);
            }
            
            if (_hintButton != null)
            {
                _hintButton.RegisterCallback<ClickEvent>(OnHintButtonClicked);
            }
            
            if (_closeHintButton != null)
            {
                _closeHintButton.RegisterCallback<ClickEvent>(OnCloseHintButtonClicked);
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
        
        private void OnMoveBackButtonClicked(ClickEvent evt)
        {
            evt.StopPropagation();
            if (!_isProcessingQuery)
            {
                _isProcessingQuery = true;
                CharacterVisualizationService.Instance.MoveCharactersToStartPositions(spawnZone);
                
                if (_tableDropdown != null) _tableDropdown.value = "--";
                if (_fieldDropdown != null) _fieldDropdown.value = "--";
                if (_operatorDropdown != null) _operatorDropdown.value = "--";
                if (_valueDropdown != null) _valueDropdown.value = "--";
                if (_groupDropdown != null) _groupDropdown.value = "--";
                
                if (_whereContinuationRow != null)
                {
                    _whereContinuationRow.style.display = DisplayStyle.None;
                }
                
                UpdateQueryPreview();
                StartCoroutine(ResetQueryProcessingFlag());
            }
        }
        
        private void OnHintButtonClicked(ClickEvent evt)
        {
            evt.StopPropagation();
            if (_hintPopup != null)
            {
                UpdateTaskDescription();
                _hintPopup.RemoveFromClassList("hidden");
            }
        }
        
        private void OnCloseHintButtonClicked(ClickEvent evt)
        {
            evt.StopPropagation();
            if (_hintPopup != null)
            {
                _hintPopup.AddToClassList("hidden");
            }
        }
        
        private void OnPrevButtonClicked(ClickEvent evt)
        {
            evt.StopPropagation();
            if (_levelData?.tasks != null && _currentTaskIndex > 0)
            {
                _currentTaskIndex--;
                UpdateTaskDescription();
            }
        }
        
        private void OnNextButtonClicked(ClickEvent evt)
        {
            evt.StopPropagation();
            if (_levelData?.tasks != null && _currentTaskIndex < _levelData.tasks.Count - 1)
            {
                _currentTaskIndex++;
                UpdateTaskDescription();
            }
        }
        
        private void OnExitButtonClicked(ClickEvent evt)
        {
            evt.StopPropagation();
            EventManager.Instance.TriggerEvent(NavigationEventType.ToSqlMenu);
        }
        
        private void OnQueryValidated()
        {
            if (_levelData?.tasks != null && _levelData.tasks.Count > _currentTaskIndex)
            {
               
            }
        }
        
        private void ExecuteQuery(string query)
        {
            if (string.IsNullOrEmpty(query)) return;
            
            Debug.Log($"Exécution de la requête: {query}");
            
            CharacterDataService.Instance.ExecuteCharacterQuery(query, queryResults => {
                Debug.Log($"Résultats trouvés: {queryResults.Count}");
                
                _lastQueryResultIds.Clear();
                foreach (var character in queryResults)
                {
                    Debug.Log($"Personnage: {character.Name}, Faction: {character.Faction}, Rôle: {character.Role}");
                    _lastQueryResultIds.Add(character.Id);
                }
                
                if (_lastQueryResultIds.Count > 0)
                {
                    CharacterVisualizationService.Instance.MoveFilteredCharactersToZone(_lastQueryResultIds, resultZone);
                }
                
                EventManager.Instance.TriggerEvent(SqlEventType.QueryValidated);
            });
        }
        
        private void OnDisable()
        {
            UnregisterEventHandlers();
        }
        
        private void UnregisterEventHandlers()
        {
            if (_moveBackButton != null)
            {
                _moveBackButton.UnregisterCallback<ClickEvent>(OnMoveBackButtonClicked);
            }
            
            if (_hintButton != null)
            {
                _hintButton.UnregisterCallback<ClickEvent>(OnHintButtonClicked);
            }
            
            if (_closeHintButton != null)
            {
                _closeHintButton.UnregisterCallback<ClickEvent>(OnCloseHintButtonClicked);
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
            
            if (_tableDropdown != null)
            {
                _tableDropdown.UnregisterValueChangedCallback(OnDropdownValueChanged);
            }
            
            if (_fieldDropdown != null)
            {
                _fieldDropdown.UnregisterValueChangedCallback(OnFieldDropdownChanged);
            }
            
            if (_operatorDropdown != null)
            {
                _operatorDropdown.UnregisterValueChangedCallback(OnDropdownValueChanged);
            }
            
            if (_valueDropdown != null)
            {
                _valueDropdown.UnregisterValueChangedCallback(OnDropdownValueChanged);
            }
            
            if (_groupDropdown != null)
            {
                _groupDropdown.UnregisterValueChangedCallback(OnDropdownValueChanged);
            }
            
            EventManager.Instance.Unsubscribe(SqlEventType.QueryValidated, OnQueryValidated);
        }
        
        private void OnDestroy()
        {
            CharacterVisualizationService.Instance.DestroyAllCharacters();
        }
    }
}