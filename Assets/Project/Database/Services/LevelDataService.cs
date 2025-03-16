﻿using UnityEngine;
using Project.Database.Models;
using Newtonsoft.Json;

namespace Project.Database.Services
{
    public class LevelDataService
    {
        private string _currentLevelJson;

        public string LoadLevelJson(string levelName)
        {
            string resourcePath = $"LevelData/{levelName}";
            TextAsset jsonFile = Resources.Load<TextAsset>(resourcePath);

            if (jsonFile == null)
            {
                Debug.LogError($"Failed to load level file: {levelName}");
                return null;
            }

            _currentLevelJson = jsonFile.text;
            return _currentLevelJson;
        }

        public void SetLevelJson(string jsonContent)
        {
            if (string.IsNullOrEmpty(jsonContent))
            {
                Debug.LogError("JSON content is empty");
                return;
            }

            _currentLevelJson = jsonContent;
        }

        public LevelData GetCurrentLevelData()
        {
            if (string.IsNullOrEmpty(_currentLevelJson))
            {
                Debug.LogError("No level data loaded");
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<LevelData>(_currentLevelJson);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error parsing level data: {e.Message}");
                return null;
            }
        }

        public bool ValidateLevelData(string jsonContent)
        {
            if (string.IsNullOrEmpty(jsonContent))
            {
                return false;
            }

            try
            {
                var levelData = JsonConvert.DeserializeObject<LevelData>(jsonContent);
                return levelData != null && levelData.tables != null;
            }
            catch
            {
                return false;
            }
        }
    }
}