using UnityEngine;
using System.Collections.Generic;
using Project.Database.Models;

namespace Project.Game.Characters.Services
{
    public class CharacterVisualizationService
    {
        private static CharacterVisualizationService _instance;
        public static CharacterVisualizationService Instance => _instance ??= new CharacterVisualizationService();
        
        private List<Character> _activeCharacters = new List<Character>();
        private Transform _parentTransform;
        
        private CharacterVisualizationService() { }
        
        public void SetParentTransform(Transform parent)
        {
            _parentTransform = parent;
        }
        
        public List<Character> InstantiateCharacters(List<CharacterData> characterDataList, GameObject spawnZone)
        {
            if (spawnZone == null || characterDataList == null || characterDataList.Count == 0)
            {
                return new List<Character>();
            }
            
            List<Character> newCharacters = CharacterFactory.CreateCharactersFromDataList(
                characterDataList,
                spawnZone,
                _parentTransform
            );
            
            _activeCharacters.AddRange(newCharacters);
            return newCharacters;
        }
        
        public void MoveCharactersToZone(GameObject targetZone, float speed = 1.0f)
        {
            if (targetZone == null) return;
            
            foreach (Character character in _activeCharacters)
            {
                character.MoveToRandomPositionInZone(targetZone, speed);
            }
        }
        
        public void DestroyAllCharacters()
        {
            foreach (Character character in _activeCharacters)
            {
                character?.Destroy();
            }
            
            _activeCharacters.Clear();
        }
        
        public int GetActiveCharacterCount()
        {
            return _activeCharacters.Count;
        }
    }
}