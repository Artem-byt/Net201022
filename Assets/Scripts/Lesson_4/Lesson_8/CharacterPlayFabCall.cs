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
                { DAMAGE,0},
                { GOLD, 0 },
                { HP,1}
            };
            UpdateCharacterStatistics(callback, result.CharacterId, dictionary); 
        },
        Debug.LogError);
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
        Debug.LogError); 
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
        error => Debug.Log(error.GenerateErrorReport()));
    }

    public static void CompletePurchaseForCharacterSlots()
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
