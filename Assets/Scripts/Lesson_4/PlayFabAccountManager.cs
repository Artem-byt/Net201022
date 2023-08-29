using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class PlayFabAccountManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _titleLabel;

    private void Start()
    { 
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountSuccess, OnFailure); 
    }

    private void OnGetAccountSuccess(GetAccountInfoResult  result) 
    { 
        _titleLabel.text = $"Welcome back, Player ID{result.AccountInfo.PlayFabId}, {result.AccountInfo.Username}"; 
    }
     
    private void OnFailure(PlayFabError error) 
    { 
        var errorMessage = error.GenerateErrorReport(); 
        Debug.LogError($"Something went wrong:{errorMessage}"); 
    }
}
