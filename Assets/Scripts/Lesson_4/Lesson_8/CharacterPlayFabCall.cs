using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPlayFabCall
{
    public Action OnUpdateCharacterStaristics;
    private string _characterName;

    public void OnNameChanged(string changedName) 
    {
        _characterName = changedName;
    }

    public void CreateCharacterWithItemId(string itemId) 
    { 
        PlayFabClientAPI.GrantCharacterToUser(new GrantCharacterToUserRequest 
        { 
            CharacterName = _characterName, 
            ItemId = itemId 
        }, 
        result => 
        { 
            UpdateCharacterStatistics(result.CharacterId); 
        },
        Debug.LogError);
    }

    private void UpdateCharacterStatistics(string characterId) 
    { 
        PlayFabClientAPI.UpdateCharacterStatistics(new UpdateCharacterStatisticsRequest 
        { 
            CharacterId = characterId, 
            CharacterStatistics = new Dictionary<string, int> 
            {
                { "Level",1}, 
                { "XP",0}, 
                { "Gold",0},
                { "HP",0}
            } 
        }, result => 
        { 
            Debug.Log($"Initial stats set, telling clientto update character list");
            OnUpdateCharacterStaristics?.Invoke();
        }, 
        Debug.LogError); 
    }
}
