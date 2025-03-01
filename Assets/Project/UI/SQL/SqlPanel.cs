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
            
            var root = _document.rootVisualElement;
            _queryInput = root.Q<TextField>("query-input");
            _executeButton = root.Q<Button>("execute-button");
            _taskNameLabel = root.Q<Label>("task-name");
            _taskDescriptionLabel = root.Q<Label>("task-description");
            _hintLabel = root.Q<Label>("hint");
            _resultLabel = root.Q<Label>("result");
            
            _executeButton.clicked += OnExecuteClicked;
            
            EventManager.Instance.Subscribe(SqlEventType.QueryValidated, OnQueryValidated);
            UpdateUI();
        }
        
        private void OnDisable()
        {
            _executeButton.clicked -= OnExecuteClicked;
            EventManager.Instance.Unsubscribe(SqlEventType.QueryValidated, OnQueryValidated);
        }
        
        private void UpdateUI()
        {
            var currentTask = HudSqlManager.Instance.GetCurrentTask();
            if (currentTask == null) return;
            
            _taskNameLabel.text = currentTask.TaskName;
            _taskDescriptionLabel.text = currentTask.TaskDescription;
            _hintLabel.text = currentTask.Hint;
        }
        
        private void OnExecuteClicked()
        {
            var query = _queryInput.value;
            EventManager.Instance.Subscribe(SqlEventType.QuerySubmitted, OnQuerySubmitted);
            // TODO: Envoyer la requête au SqlManager et afficher le résultat
        }
        
        private void OnQueryValidated()
        {
            UpdateUI();
        }
        
        private void OnQuerySubmitted()
        {
            // TODO: Afficher le résultat de la requête
        }
    }
}