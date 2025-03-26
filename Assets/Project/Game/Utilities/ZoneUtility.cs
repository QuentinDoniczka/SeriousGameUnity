using UnityEngine;

namespace Project.Game.Utilities
{
    public static class ZoneUtility
    {
        public static Rect GetZoneBounds(GameObject zone)
        {
            if (zone == null)
                return new Rect(0, 0, 1, 1);
                
            RectTransform rectTransform = zone.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                Vector3[] corners = new Vector3[4];
                rectTransform.GetWorldCorners(corners);
                
                float minX = corners[0].x;
                float minY = corners[0].y;
                float maxX = corners[2].x;
                float maxY = corners[2].y;
                
                return new Rect(minX, minY, maxX - minX, maxY - minY);
            }
            
            Renderer renderer = zone.GetComponent<Renderer>();
            if (renderer != null)
            {
                Bounds bounds = renderer.bounds;
                return new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y);
            }
            
            return new Rect(zone.transform.position.x, zone.transform.position.y, 1, 1);
        }
        
        public static Vector2 GetPositionInZone(GameObject zone, Vector2 normalizedPosition)
        {
            Vector2 clampedPosition = new Vector2(
                Mathf.Clamp01(normalizedPosition.x),
                Mathf.Clamp01(normalizedPosition.y)
            );
            
            Rect bounds = GetZoneBounds(zone);
            
            float x = Mathf.Lerp(bounds.xMin, bounds.xMax, clampedPosition.x);
            float y = Mathf.Lerp(bounds.yMin, bounds.yMax, clampedPosition.y);
            
            return new Vector2(x, y);
        }
        
        public static Vector2 GetRandomPositionInZone(GameObject zone)
        {
            return GetPositionInZone(zone, new Vector2(Random.value, Random.value));
        }
    }
}