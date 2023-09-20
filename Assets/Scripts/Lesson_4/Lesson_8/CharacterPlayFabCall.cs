using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class CharacterPlayFabCall
{

    public const string XP = "XP";
    public const string LEVEL = "Level";
    public const string DAMAGE = "Damage";
    public const string GOLD = "GOLD";
    public const string HP = "HP";

    public static void CreateCharacterWithItemId(string itemId, Action callback, string characterName) 
    {
        PlayFabClientAPI.GrantCharacterToUser(new GrantCharacterToUserRequest 
        { 
            CharacterName = characterName, 
            ItemId = itemId 
        }, 
        result => 
        {
            var dictionary = new Dictionary<string, int> 
            {
                { LEVEL,1},
                { XP,0},
                { DAMAGE,1},
                { GOLD,0},
                { HP,1}
            };
            UpdateCharacterStatistics(callback, result.CharacterId, dictionary); 
        },
        OnFailure);
    }

    public static void UpdateCharacterStatistics(Action callback, string characterId, Dictionary<string, int> characterStatistics) 
    {
        PlayFabClientAPI.UpdateCharacterStatistics(new UpdateCharacterStatisticsRequest 
        { 
            CharacterId = characterId, 
            CharacterStatistics = characterStatistics
        }, result => 
        { 
            Debug.Log($"Initial stats set, telling client to update character list");
            callback?.Invoke();
        },
        OnFailure); 
    }

    public static void GetCHaracterStatistics(Action<Dictionary<string, int>> callback, string characterId)
    {
        PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest
        {
            CharacterId = characterId
        },
        result =>
        {
            callback?.Invoke(result.CharacterStatistics);
        },
        OnFailure);
    }

    public static void CompletePurchaseForCharacterSlots(Action successCallback)
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
            successCallback?.Invoke();
        }, OnFailure);
    }

    private static void OnFailure(PlayFabError error)
    {
        Debug.Log($"Error Purchase - {error.GenerateErrorReport()}");
    }
}
