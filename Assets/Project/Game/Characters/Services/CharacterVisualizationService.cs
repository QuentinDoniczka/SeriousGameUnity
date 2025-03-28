using UnityEngine;
using System.Collections.Generic;
using Project.Database.Models;

namespace Project.Game.Characters.Services
{
    /// <summary>
    /// Manages the visualization, spawning, and movement of characters in the game world
    /// </summary>
    /// <remarks>
    /// Detailed file description:
    /// <list type="bullet">
    /// <item>Provides character instantiation from data models</item>
    /// <item>Handles character movement with various positioning strategies</item>
    /// <item>Implements singleton pattern for global access</item>
    /// <item>Manages character lifecycle and reference tracking</item>
    /// </list>
    /// </remarks>
    /// <author>Quentin Doniczka</author>
    /// <date>28/03/2025</date>
    public class CharacterVisualizationService
    {
        private static CharacterVisualizationService _instance;
        public static CharacterVisualizationService Instance => _instance ??= new CharacterVisualizationService();
        
        private List<Character> _activeCharacters = new();
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
        
        public Character GetCharacterById(int id)
        {
            return _activeCharacters.Find(c => c.Id == id);
        }
        
        public List<Character> GetCharactersByIds(List<int> ids)
        {
            if (ids == null || ids.Count == 0)
                return new List<Character>();
                
            List<Character> characters = new List<Character>();
            foreach (int id in ids)
            {
                Character character = GetCharacterById(id);
                if (character != null)
                {
                    characters.Add(character);
                }
            }
            
            return characters;
        }
        
        public void MoveCharactersToZoneRandom(GameObject targetZone, float speed = 1.5f)
        {
            if (targetZone == null) return;
            
            foreach (Character character in _activeCharacters)
            {
                character.MoveToRandomPositionInZone(targetZone, speed);
            }
        }
        
        public void MoveCharactersToZone(List<Character> characters, GameObject targetZone, float speed = 1.5f)
        {
            if (targetZone == null || characters == null || characters.Count == 0) return;
            
            int count = characters.Count;
            int columns = Mathf.CeilToInt(Mathf.Sqrt(count));
            int rows = Mathf.CeilToInt((float)count / columns);
            
            float margin = 0.1f;
            float usableWidth = 1.0f - 2 * margin;
            float usableHeight = 1.0f - 2 * margin;
            
            for (int i = 0; i < count; i++)
            {
                int col = i % columns;
                int row = i / columns;
                
                float normalizedX = margin + (col + 0.5f) * (usableWidth / columns);
                float normalizedY = margin + (row + 0.5f) * (usableHeight / rows);
                
                Vector2 normalizedPosition = new Vector2(normalizedX, normalizedY);
                characters[i].MoveToPositionInZone(targetZone, normalizedPosition, speed);
            }
        }
        
        public void MoveCharactersByIdsToZone(List<int> characterIds, GameObject targetZone, float speed = 1.5f)
        {
            List<Character> characters = GetCharactersByIds(characterIds);
            MoveCharactersToZone(characters, targetZone, speed);
        }
        
        public void MoveFilteredCharactersToZone(List<int> targetIds, GameObject targetZone, float speed = 1.5f)
        {
            if (targetZone == null || targetIds == null || targetIds.Count == 0) return;
    
            // Créer une liste ordonnée selon l'ordre des targetIds
            List<Character> charactersToMove = new List<Character>();
            List<Character> charactersToReset = new List<Character>();
    
            // Ajouter les personnages dans l'ordre exact de targetIds
            foreach (int id in targetIds)
            {
                Character character = _activeCharacters.Find(c => c.Id == id);
                if (character != null)
                {
                    charactersToMove.Add(character);
                }
            }
            
            foreach (Character character in _activeCharacters)
            {
                if (!targetIds.Contains(character.Id))
                {
                    charactersToReset.Add(character);
                }
            }
            
            MoveCharactersToZone(charactersToMove, targetZone, speed);
            
            foreach (Character character in charactersToReset)
            {
                character.MoveToStartPosition(speed);
            }
        }
        
        public void MoveCharactersToStartPositions(GameObject targetZone, float speed = 1.5f)
        {
            if (targetZone == null) return;
            
            foreach (Character character in _activeCharacters)
            {
                character.MoveToStartPosition(speed);
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