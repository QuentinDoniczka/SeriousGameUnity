using UnityEngine;
using System.Collections.Generic;
using Project.Database.Models;
using Project.Database.Services;
using Project.Core.Service;

namespace Project.Game.Characters.Services
{
    public class CharacterDataService
    {
        private static CharacterDataService _instance;
        public static CharacterDataService Instance => _instance ??= new CharacterDataService();
        
        private List<CharacterData> _characterDataList = new List<CharacterData>();
        
        private CharacterDataService() { }
        
        public void LoadAllCharacters()
        {
            _characterDataList = SqlQueryExecutorService.Instance.ExtractAllCharactersData();
        }
        
        public List<CharacterData> GetAllCharacters()
        {
            if (_characterDataList.Count == 0)
            {
                LoadAllCharacters();
            }
            
            return _characterDataList;
        }
        
        public void ExecuteCharacterQuery(string query, System.Action<List<CharacterData>> onResultsReady)
        {
            SqlQueryExecutorService.Instance.ExecuteQuery(query, onResultsReady);
        }
        
        public bool HasCharacters()
        {
            return _characterDataList.Count > 0;
        }
        
        public void ClearCharacters()
        {
            _characterDataList.Clear();
        }
    }
}