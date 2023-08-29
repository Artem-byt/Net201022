using PlayFab; 
using PlayFab.ClientModels; 
using UnityEngine;

public class PlayFabLogin_Lesson_4 : MonoBehaviour
{
    public void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            /* * If not we need to assign it to the appropriate variable manually * Otherwise we can just remove this if statement at all */
            PlayFabSettings.staticSettings.TitleId = "3C389";
        }
        var request = new LoginWithCustomIDRequest
        {
            CustomId = "GeekBrains_Sem4",
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }
    private void OnLoginSuccess(LoginResult result) 
    { 
        Debug.Log("Congratulations, you made successfulAPI call!");
    }

    private void OnLoginFailure(PlayFabError error) 
    { 
        var errorMessage = error.GenerateErrorReport(); Debug.LogError($"Something went wrong:{errorMessage}"); 
    }
}
