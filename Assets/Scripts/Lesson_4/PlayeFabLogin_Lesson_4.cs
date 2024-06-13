using PlayFab.ClientModels;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;

public class PlayeFabLogin_Lesson_4 : MonoBehaviour
{
    [SerializeField] private AuthorizationUI _authorizationUI;
    [SerializeField] private string _playFabTitle;

    private NewAccountModel _newAccountModel;

    public void Awake()
    {
        _newAccountModel = new NewAccountModel();
        _authorizationUI.EmailAddressInputAuth.onValueChanged.AddListener(UpdateEmail);
        _authorizationUI.UsernameInputAuth.onValueChanged.AddListener(UpdateUsername);
        _authorizationUI.PasswordInputAuth.onValueChanged.AddListener(UpdatePassword);

        _authorizationUI.UsernameInputSignIn.onValueChanged.AddListener(UpdateUsername);
        _authorizationUI.PasswordInputSignIn.onValueChanged.AddListener(UpdatePassword);

        _authorizationUI.BtnBackToSignIn.onClick.AddListener(BackToSignInWindow);
        _authorizationUI.BtnBackToAuth.onClick.AddListener(BackToAuthorizationWindow);

        _authorizationUI.BtnNextAuth.onClick.AddListener(CreateAccount);
        _authorizationUI.BtnNextSignIn.onClick.AddListener(SignIn);

        _authorizationUI.AutoCreateUserWithCustomId.onClick.AddListener(OnAutoCreate);
    }

    public void OnAutoCreate()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = _playFabTitle;
        }

        _newAccountModel.Id = PlayerPrefs.GetString(ConstantStrings.AUTH_GUID_KEY, Guid.NewGuid().ToString());
        var needCreation = PlayerPrefs.HasKey(ConstantStrings.AUTH_GUID_KEY);
        var customIdRequest = new LoginWithCustomIDRequest() 
        { 
            CustomId = _newAccountModel.Id, 
            CreateAccount = !needCreation 
        };
        _authorizationUI.LoadingAnimation.SetRotating();
        PlayFabLoginCall.LoginWithCustomId(OnLoginWithCustomId, OnFailerAutoCreate, customIdRequest);
    }

    private void OnFailerAutoCreate(PlayFabError error)
    {
        _newAccountModel.Id = Guid.NewGuid().ToString();
        var customIdRequest = new LoginWithCustomIDRequest()
        {
            CustomId = _newAccountModel.Id,
            CreateAccount = true
        };
        _authorizationUI.LoadingAnimation.SetRotating();
        PlayFabLoginCall.LoginWithCustomId(OnLoginWithCustomId, OnFailerAutoCreate, customIdRequest);
    }

    private void OnLoginWithCustomId(LoginResult success)
    {
        Debug.Log("Created Account with custom id");
        _authorizationUI.LoadingAnimation.SetRotating();
        PlayerPrefs.SetString(ConstantStrings.AUTH_GUID_KEY, _newAccountModel.Id);
        SceneManager.LoadScene(ConstantStrings.SCENE_LOBBY);
    }

    public void UpdateUsername(string username)
    {
        _newAccountModel.Username = username;
    }

    public void UpdateEmail(string mail)
    {
        _newAccountModel.Mail = mail;
    }

    public void UpdatePassword(string pass)
    {
        _newAccountModel.Pass = pass;
    }

    public void CreateAccount()
    {
        _authorizationUI.LoadingAnimation.SetRotating();
        var registerRequest = new RegisterPlayFabUserRequest 
        { 
            Username = _newAccountModel.Username, 
            Email = _newAccountModel.Mail, 
            Password = _newAccountModel.Pass, 
            RequireBothUsernameAndEmail = true 
        };
        PlayFabLoginCall.RegisterPlayFabAccount(OnCreateSuccess, OnFailure, registerRequest);
    }

    public void SignIn()
    {
        _authorizationUI.LoadingAnimation.SetRotating();
        var loginRequest = new LoginWithPlayFabRequest 
        { 
            Username = _newAccountModel.Username, 
            Password = _newAccountModel.Pass 
        };
        PlayFabLoginCall.LoginInPlayFab(OnSignInSuccess, OnFailure, loginRequest);
    }

    public void Back()
    {
        _authorizationUI.ErrorLabelOfCreatingAcc.text = string.Empty;
        _authorizationUI.ErrorLabelOfSignIn.text = string.Empty;
    }

    private void OnCreateSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log($"Creation Success: {_newAccountModel.Username}");
        _authorizationUI.LoadingAnimation.SetRotating();
    }

    private void OnSignInSuccess(LoginResult result)
    {
        Debug.Log($"Sign In Success: {_newAccountModel.Username}");
        _authorizationUI.LoadingAnimation.SetRotating();
        SceneManager.LoadScene(ConstantStrings.SCENE_LOBBY);  
    }

    private void OnFailure(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError($"Something went wrong:{errorMessage}");
        _authorizationUI.ErrorLabelOfCreatingAcc.text = errorMessage;
        _authorizationUI.ErrorLabelOfSignIn.text = errorMessage;
        _authorizationUI.LoadingAnimation.SetRotating();
    }

    private void BackToAuthorizationWindow()
    {
        ChangeStateOfPanels(true, false);
    }

    private void BackToSignInWindow()
    {
        ChangeStateOfPanels(false, true);
    }

    private void ChangeStateOfPanels(bool stateOfPanelForAuth, bool stateOfPanelForSignIn)
    {
        _authorizationUI.PanelForAuth.SetActive(stateOfPanelForAuth);
        _authorizationUI.PanelForSignIn.SetActive(stateOfPanelForSignIn);
        Back();
    }

    private void OnDestroy()
    {
        _authorizationUI.Cancel();
    }


}
