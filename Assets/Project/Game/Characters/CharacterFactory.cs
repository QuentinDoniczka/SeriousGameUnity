using UnityEngine;
using System.Collections.Generic;

namespace Project.Game.Characters
{
    public static class CharacterFactory
    {
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
            Vector2 clampedPosition = new Vector2(
                Mathf.Clamp01(normalizedPosition.x),
                Mathf.Clamp01(normalizedPosition.y)
            );
            
            Vector2 position = Vector2.zero;
            
            if (safeZone != null)
            {
                Renderer renderer = safeZone.GetComponent<Renderer>();
                if (renderer != null)
                {
                    Bounds bounds = renderer.bounds;
                    float x = Mathf.Lerp(bounds.min.x, bounds.max.x, clampedPosition.x);
                    float y = Mathf.Lerp(bounds.min.y, bounds.max.y, clampedPosition.y);
                    position = new Vector2(x, y);
                }
                else
                {
                    RectTransform rectTransform = safeZone.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        Vector3[] corners = new Vector3[4];
                        rectTransform.GetWorldCorners(corners);
                        
                        float minX = corners[0].x;
                        float minY = corners[0].y;
                        float maxX = corners[2].x;
                        float maxY = corners[2].y;
                        
                        float x = Mathf.Lerp(minX, maxX, clampedPosition.x);
                        float y = Mathf.Lerp(minY, maxY, clampedPosition.y);
                        position = new Vector2(x, y);
                    }
                    else
                    {
                        Debug.LogWarning("SafeZone has no Renderer or RectTransform to determine its bounds");
                        position = safeZone.transform.position;
                    }
                }
            }
            
            return CreateFromPrefab(prefabPath, position, parent);
        }
        
        public static List<Character> CreateGridInSafeZone(string prefabPath, GameObject safeZone, int gridSize, Transform parent = null)
        {
            List<Character> characters = new List<Character>();
            
            if (safeZone == null || gridSize <= 0)
            {
                Debug.LogError("SafeZone undefined or invalid grid size");
                return characters;
            }
            
            float step = 1f / (gridSize + 1);
            
            for (int y = 1; y <= gridSize; y++)
            {
                for (int x = 1; x <= gridSize; x++)
                {
                    Vector2 normalizedPosition = new Vector2(
                        x * step,
                        y * step
                    );
                    
                    Character character = CreateInSafeZone(
                        prefabPath,
                        safeZone,
                        normalizedPosition,
                        parent
                    );
                    
                    if (character != null)
                    {
                        characters.Add(character);
                    }
                }
            }
            
            return characters;
        }
    }
}