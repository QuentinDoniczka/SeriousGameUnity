using UnityEngine;
using UnityEngine.UIElements;
using Project.Core.Events;
using Project.Game.Characters;
using System.Collections.Generic;
using System.Collections;

namespace Project.UI.SQL
{
    public class SqlPanel : MonoBehaviour
    {
        [SerializeField] private string characterPrefabPath = "Characters/sampleCharacterHuman";
        [SerializeField] private GameObject spawnZone;
        [SerializeField] private GameObject resultZone;
        [SerializeField] private int gridSize = 3;
        
        private UIDocument _document;
        private Button _backButton;
        private Button _moveUnitsButton;
        private VisualElement _contentArea;
        private VisualElement _navBar;
        private ScrollView _scrollView;
        private List<Character> _characters = new();
        private bool _isProcessingClick = false;
        
        private void Awake()
        {
            FindSafeZone();
            InstantiateCharacters();
        }
        
        private void FindSafeZone()
        {
            if (spawnZone == null)
            {
                spawnZone = GameObject.Find("safe zone");
            }
        }
        
        private void InstantiateCharacters()
        {
            if (spawnZone == null) return;
            
            List<Character> gridCharacters = CharacterFactory.CreateGridInSafeZone(
                characterPrefabPath,
                spawnZone,
                gridSize,
                transform
            );
            
            _characters.AddRange(gridCharacters);
        }
        
        private void OnEnable()
        {
            _document = GetComponent<UIDocument>();
            if (_document == null) return;
            
            var root = _document.rootVisualElement;
            _contentArea = root.Q<VisualElement>("content-area");
            _navBar = root.Q<VisualElement>("nav-bar");
            _scrollView = root.Q<ScrollView>("scroll-view");
            _backButton = root.Q<Button>("back-button");
            _moveUnitsButton = root.Q<Button>("move-units-button");
            
            ConfigureScrollView();
            ConfigureButtons();
        }
        
        private void ConfigureScrollView()
        {
            if (_scrollView != null)
            {
                _scrollView.elasticity = 0;
                _scrollView.scrollDecelerationRate = 0.1f;
                _scrollView.touchScrollBehavior = ScrollView.TouchScrollBehavior.Clamped;
            }
        }
        
        private void ConfigureButtons()
        {
            if (_backButton != null)
            {
                _backButton.RegisterCallback<ClickEvent>(evt => {
                    evt.StopPropagation();
                    OnBackClicked();
                });
            }
            
            if (_moveUnitsButton != null)
            {
                _moveUnitsButton.RegisterCallback<ClickEvent>(evt => {
                    evt.StopPropagation();
                    if (!_isProcessingClick)
                    {
                        _isProcessingClick = true;
                        OnMoveUnitsClicked();
                        StartCoroutine(ResetClickFlag());
                    }
                });
            }
        }
        
        private IEnumerator ResetClickFlag()
        {
            yield return new WaitForSeconds(0.2f);
            _isProcessingClick = false;
        }
        
        private void OnDisable()
        {
            if (_backButton != null)
            {
                _backButton.UnregisterCallback<ClickEvent>(evt => OnBackClicked());
            }
            
            if (_moveUnitsButton != null)
            {
                _moveUnitsButton.UnregisterCallback<ClickEvent>(evt => OnMoveUnitsClicked());
            }
        }
        
        private void OnBackClicked()
        {
            EventManager.Instance.TriggerEvent(NavigationEventType.ToSqlMenu);
        }

        private void OnMoveUnitsClicked()
        {

            foreach (Character character in _characters)
            {
                float speed = 2.0f;
                character.RandomLocation(speed, -3f, 3f, -3f, 3f);
            }

        }

        private void OnDestroy()
        {
            foreach (Character character in _characters)
            {
                character?.Destroy();
            }
            _characters.Clear();
        }
    }
}