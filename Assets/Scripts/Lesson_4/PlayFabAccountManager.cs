using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayFabAccountManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _titleLabel;
    [SerializeField] private Button _btnForgotAccount;

    private void Start()
    { 
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccountSuccess, OnFailure);
        _btnForgotAccount.onClick.AddListener(OnForgetAccount);
    }

    private void OnForgetAccount()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        SceneManager.LoadScene("Lesson_4");
    }

    private void OnGetAccountSuccess(GetAccountInfoResult  result) 
    { 
        _titleLabel.text = $"Welcome back, Player ID{result.AccountInfo.PlayFabId}, {result.AccountInfo.Created}"; 
    }
     
    private void OnFailure(PlayFabError error) 
    { 
        var errorMessage = error.GenerateErrorReport(); 
        Debug.LogError($"Something went wrong:{errorMessage}"); 
    }

    private void OnDestroy()
    {
        _btnForgotAccount.onClick.RemoveAllListeners();
    }
}
