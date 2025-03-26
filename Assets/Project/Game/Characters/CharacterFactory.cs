using UnityEngine;
using System.Collections.Generic;
using Project.Game.Utilities;

namespace Project.Game.Characters
{
    public static class CharacterFactory
    {
        private static readonly Dictionary<string, string> RoleToPrefabPath = new Dictionary<string, string>
        {
            { "soldier", "Characters/soldier" },
            { "knight", "Characters/knight" },
            { "mage", "Characters/mage" },
            { "healer", "Characters/healer" }
        };
        
        private static string GetPrefabPathForRole(string role)
        {
            return RoleToPrefabPath.TryGetValue(role.ToLower(), out string prefabPath)
                ? prefabPath
                : RoleToPrefabPath["soldier"];
        }
        
        public static Character CreateFromPrefab(string prefabPath, Vector2 position, Transform parent = null)
        {
            GameObject prefab = Resources.Load<GameObject>(prefabPath);
            
            if (prefab == null)
            {
                Debug.LogError($"Failed to load character prefab from path: {prefabPath}");
                return null;
            }
            
            GameObject instance = Object.Instantiate(prefab, position, Quaternion.identity);
            
            if (instance == null)
            {
                Debug.LogError("Failed to instantiate character prefab");
                return null;
            }
            
            Character character = new Character(instance, prefabPath);
            
            if (parent != null)
            {
                character.SetParent(parent);
            }
            
            return character;
        }
        
        public static Character CreateInSafeZone(string prefabPath, GameObject safeZone, Vector2 normalizedPosition, Transform parent = null)
        {
            Vector2 position = ZoneUtility.GetPositionInZone(safeZone, normalizedPosition);
            return CreateFromPrefab(prefabPath, position, parent);
        }
        
        public static Character CreateCharacterFromData(CharacterData data, GameObject spawnZone, Vector2 normalizedPosition, Transform parent = null)
        {
            string prefabPath = GetPrefabPathForRole(data.Role);
            Character character = CreateInSafeZone(prefabPath, spawnZone, normalizedPosition, parent);
            
            if (character != null)
            {
                character.Instance.name = data.Name;
            }
            
            return character;
        }
        
        public static List<Character> CreateCharactersFromDataList(
            List<CharacterData> characterDataList, 
            GameObject spawnZone, 
            Transform parent = null, 
            string factionFilter = null)
        {
            List<Character> characters = new List<Character>();
            
            if (spawnZone == null || characterDataList == null || characterDataList.Count == 0)
            {
                Debug.LogError("Invalid parameters for CreateCharactersFromDataList");
                return characters;
            }
            
            List<CharacterData> filteredList = factionFilter != null 
                ? characterDataList.FindAll(data => data.Faction == factionFilter)
                : new List<CharacterData>(characterDataList);
            
            int count = filteredList.Count;
            int columns = 4;
            
            int rows = Mathf.CeilToInt((float)count / columns);
            
            for (int i = 0; i < count; i++)
            {
                int col = i % columns;
                int row = i / columns;
                
                float margin = 0.1f;
                float usableWidth = 1.0f - 2 * margin;
                float usableHeight = 1.0f - 2 * margin;
                
                float normalizedX = margin + (col + 0.5f) * (usableWidth / columns);
                float normalizedY = margin + (row + 0.5f) * (usableHeight / rows);
                
                Vector2 normalizedPosition = new Vector2(normalizedX, normalizedY);
                
                Character character = CreateCharacterFromData(
                    filteredList[i],
                    spawnZone,
                    normalizedPosition,
                    parent
                );
                
                if (character != null)
                {
                    characters.Add(character);
                }
            }
            
            return characters;
        }
    }
}