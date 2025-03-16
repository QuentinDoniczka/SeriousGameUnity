using UnityEngine;
using UnityEngine.UIElements;
using Project.Core.Events;
using Project.UI.HUD;

namespace Project.UI.SQL
{
    public class SqlPanel : MonoBehaviour
    {
        private UIDocument _document;
        private TextField _queryInput;
        private Button _executeButton;
        private Label _taskNameLabel;
        private Label _taskDescriptionLabel;
        private Label _hintLabel;
        private Label _resultLabel;
        
        private void OnEnable()
        {
            _document = GetComponent<UIDocument>();
            if (_document == null)
            {
                Debug.LogError("UIDocument component not found");
                return;
            }
            
            var root = _document.rootVisualElement;
            _queryInput = root.Q<TextField>("query-input");
            _executeButton = root.Q<Button>("execute-button");
            _taskNameLabel = root.Q<Label>("task-name");
            _taskDescriptionLabel = root.Q<Label>("task-description");
            _hintLabel = root.Q<Label>("hint");
            _resultLabel = root.Q<Label>("result");
            
            if (_executeButton != null)
            {
                _executeButton.clicked += OnExecuteClicked;
            }
            
            EventManager.Instance.Subscribe(SqlEventType.QueryValidated, OnQueryValidated);
            UpdateUI();
        }
        
        private void OnDisable()
        {
            if (_executeButton != null)
            {
                _executeButton.clicked -= OnExecuteClicked;
            }
            
            EventManager.Instance.Unsubscribe(SqlEventType.QueryValidated, OnQueryValidated);
        }
        
        private void UpdateUI()
        {
            var currentTask = HudSqlManager.Instance.GetCurrentTask();
            if (currentTask == null) return;
            
            if (_taskNameLabel != null)
                _taskNameLabel.text = currentTask.TaskName;
                
            if (_taskDescriptionLabel != null)
                _taskDescriptionLabel.text = currentTask.TaskDescription;
                
            if (_hintLabel != null)
                _hintLabel.text = currentTask.Hint;
        }
        
        private void OnExecuteClicked()
        {
            if (_queryInput == null) return;
            
            var query = _queryInput.value;
            if (string.IsNullOrEmpty(query)) return;
            
            HudSqlManager.Instance.SubmitQuery(query);
        }
        
        private void OnQueryValidated()
        {
            UpdateUI();
        }
    }
}