using PlayFab.ClientModels;
using PlayFab;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class PlayeFabLogin_Lesson_4 : MonoBehaviour
{
    [SerializeField] private string _playFabTitle;
    [SerializeField] private TMP_Text _createErrorLabel;
    [SerializeField] private TMP_Text _signInErrorLabel;

    [SerializeField] private TMP_InputField _emailAddressInputAuth;
    [SerializeField] private TMP_InputField _usernameInputAuth;
    [SerializeField] private TMP_InputField _passwordInputAuth;

    [SerializeField] private TMP_InputField _usernameInputSign;
    [SerializeField] private TMP_InputField _passwordInputSign;

    [SerializeField] private Button _btnBackToSignIn;
    [SerializeField] private Button _btnBackToAuth;

    [SerializeField] private Button _btnSignIn;
    [SerializeField] private Button _btnAuth;

    [SerializeField] private GameObject _panelAuth;
    [SerializeField] private GameObject _panelSignIn;

    private string _mail;
    private string _pass;
    private string _username;

    private const string AuthGuidKey ="authorization-guid";

    public void Awake()
    {
        _emailAddressInputAuth.onValueChanged.AddListener(UpdateEmail);
        _usernameInputAuth.onValueChanged.AddListener(UpdateUsername);
        _passwordInputAuth.onValueChanged.AddListener(UpdatePassword);

        _usernameInputSign.onValueChanged.AddListener(UpdateUsername);
        _passwordInputSign.onValueChanged.AddListener(UpdatePassword);

        _btnBackToSignIn.onClick.AddListener(BackToSignInWindow);
        _btnBackToAuth.onClick.AddListener(BackToAuthorizationWindow);

        _btnAuth.onClick.AddListener(CreateAccount);
        _btnSignIn.onClick.AddListener(SignIn);
    }

    public void Start()
    {
        // Here we need to check whether TitleId propertyis configured in settings or not
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            /* * If not we need to assign it to the appropriate variable manually * Otherwise we can just remove this if statement at all */
            PlayFabSettings.staticSettings.TitleId = _playFabTitle;
        }

        var needCreation = PlayerPrefs.HasKey(AuthGuidKey); 
        var id = PlayerPrefs.GetString(AuthGuidKey, Guid.NewGuid().ToString()); 
        PlayFabClientAPI.LoginWithCustomID(new LoginWithCustomIDRequest() 
        { 
            CustomId = id, 
            CreateAccount = !needCreation 
        }, success => 
        {
            Debug.Log("Created Account with custom id");
            SceneManager.LoadScene("Lesson_4_Lobby");
            PlayerPrefs.SetString(AuthGuidKey, id); 
        }, OnFailure);
    }

    public void UpdateUsername(string username)
    {
        _username = username;
    }

    public void UpdateEmail(string mail)
    {
        _mail = mail;
    }

    public void UpdatePassword(string pass)
    {
        _pass = pass;
    }

    public void CreateAccount()
    {
        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest
        {
            Username = _username,
            Email = _mail,
            Password = _pass,
            RequireBothUsernameAndEmail = true
        }, OnCreateSuccess, OnFailure);
    }

    public void SignIn()
    {
        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest
        {
            Username = _username,
            Password = _pass
        }, OnSignInSuccess, OnFailure);
    }

    public void Back()
    {
        _createErrorLabel.text = "";
        _signInErrorLabel.text = "";
    }

    private void OnCreateSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log($"Creation Success:{_username}");
    }

    private void OnSignInSuccess(LoginResult result)
    {
        Debug.Log($"Sign In Success:{_username}");
    }

    private void OnFailure(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError($"Something went wrong:{errorMessage}");
        _createErrorLabel.text = errorMessage;
        _signInErrorLabel.text = errorMessage;
    }

    private void BackToAuthorizationWindow()
    {
        ChangeStateOfPanels(true, false);
    }

    private void BackToSignInWindow()
    {
        ChangeStateOfPanels(false, true);
    }

    private void ChangeStateOfPanels(bool state1, bool state2)
    {
        _panelAuth.SetActive(state1);
        _panelSignIn.SetActive(state2);
        Back();
    }

    private void OnDestroy()
    {
        _emailAddressInputAuth.onValueChanged.RemoveAllListeners();
        _usernameInputAuth.onValueChanged.RemoveAllListeners();
        _passwordInputAuth.onValueChanged.RemoveAllListeners();

        _usernameInputSign.onValueChanged.RemoveAllListeners();
        _passwordInputSign.onValueChanged.RemoveAllListeners();

        _btnBackToSignIn.onClick.RemoveAllListeners();
        _btnBackToAuth.onClick.RemoveAllListeners();

        _btnAuth.onClick.RemoveAllListeners();
        _btnSignIn.onClick.RemoveAllListeners();
    }
}
