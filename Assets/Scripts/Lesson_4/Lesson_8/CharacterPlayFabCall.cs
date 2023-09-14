using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPlayFabCall
{
    public Action OnUpdateCharacterStaristics;
    private string _characterName;
    private string _characterId;

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
            _characterId = result.CharacterId;
            UpdateCharacterStatistics(); 
        },
        Debug.LogError);
    }

    public void UpdateCharacterStatistics() 
    {
        PlayFabClientAPI.UpdateCharacterStatistics(new UpdateCharacterStatisticsRequest 
        { 
            CharacterId = _characterId, 
            CharacterStatistics = new Dictionary<string, int> 
            {
                { "Level",1}, 
                { "XP",0}, 
                { "Gold",0},
                { "HP",0}
            } 
        }, result => 
        { 
            Debug.Log($"Initial stats set, telling client to update character list");
            OnUpdateCharacterStaristics?.Invoke();
        }, 
        Debug.LogError); 
    }

    public void GetCHaracterStatistics(Action<Dictionary<string, int>> callback)
    {
        PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest
        {
            CharacterId = _characterId
        },
        result =>
        {
            callback?.Invoke(result.CharacterStatistics);
        },
        error => Debug.Log(error.GenerateErrorReport()));
    }

    public void CompletePurchaseForCharacterSlots()
    {
        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
        {
            CatalogVersion = "1.0",
            ItemId = "character_token",
            Price = 1,
            VirtualCurrency = "GO"

        }, result =>
        {
            Debug.Log("Complete Purchase");
        }, error => Debug.Log("Error Purchase"));
    }

}
