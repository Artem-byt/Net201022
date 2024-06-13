using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class PLayFabAccountInfoCalls
{
    public static void GetAllUserCharacters(ListUsersCharactersRequest listUsersCharactersRequest, Action<ListUsersCharactersResult> successCallback)
    {
        PlayFabClientAPI.GetAllUsersCharacters(listUsersCharactersRequest, successCallback, OnFailure);

    }

    public static void GetUserInventoryInfo(GetUserInventoryRequest getUserInventoryRequest, Action<GetUserInventoryResult> successCallback)
    {
        PlayFabClientAPI.GetUserInventory(getUserInventoryRequest, successCallback, OnFailure);
    }

    private static void OnFailure(PlayFabError playFabError)
    {
        Debug.Log(playFabError.GenerateErrorReport());
    }
}
