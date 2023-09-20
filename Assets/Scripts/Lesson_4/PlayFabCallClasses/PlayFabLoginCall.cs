using PlayFab;
using PlayFab.ClientModels;
using System;

public static class PlayFabLoginCall
{
    public static void LoginWithCustomId(Action<LoginResult> callbackSuccess, Action<PlayFabError> callbackError, LoginWithCustomIDRequest loginWithCustomIDRequest)
    {
        PlayFabClientAPI.LoginWithCustomID(loginWithCustomIDRequest, callbackSuccess, callbackError);
    }

    public static void RegisterPlayFabAccount(Action<RegisterPlayFabUserResult> callbackSuccess, Action<PlayFabError> callbackError, RegisterPlayFabUserRequest registerPlayFabUserRequest)
    {
        PlayFabClientAPI.RegisterPlayFabUser(registerPlayFabUserRequest, callbackSuccess, callbackError);
    }

    public static void LoginInPlayFab(Action<LoginResult> callbackSuccess, Action<PlayFabError> callbackError, LoginWithPlayFabRequest loginWithPlayFabRequest)
    {
        PlayFabClientAPI.LoginWithPlayFab(loginWithPlayFabRequest, callbackSuccess, callbackError);
    }

    public static void GetAccountInfo(Action<GetAccountInfoResult> callbackSuccess, Action<PlayFabError> callbackError, GetAccountInfoRequest getAccountInfoRequest)
    {
        PlayFabClientAPI.GetAccountInfo(getAccountInfoRequest, callbackSuccess, callbackError);
    }

}
