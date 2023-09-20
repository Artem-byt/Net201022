using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AuthorizationUI : MonoBehaviour
{
    [Header("Components of panel for creating Account")]
    [SerializeField] private TMP_Text _createErrorLabel;
    [SerializeField] private TMP_InputField _emailAddressInputAuth;
    [SerializeField] private TMP_InputField _usernameInputAuth;
    [SerializeField] private TMP_InputField _passwordInputAuth;
    [SerializeField] private Button _btnBackToSignIn;
    [SerializeField] private Button _btnAuth;
    [SerializeField] private Button _autoCreateUserWithCustomID;
    [SerializeField] private GameObject _panelAuth;

    [Header("Components of panel for Sign In")]
    [SerializeField] private TMP_Text _signInErrorLabel;
    [SerializeField] private TMP_InputField _usernameInputSign;
    [SerializeField] private TMP_InputField _passwordInputSign;      
    [SerializeField] private Button _btnBackToAuth;
    [SerializeField] private Button _btnSignIn;       
    [SerializeField] private GameObject _panelSignIn;

    [Header("Animation")]
    [SerializeField] private PremitiveAnimation _loadingAnimation;

    
    public TMP_Text ErrorLabelOfCreatingAcc => _createErrorLabel;
    public TMP_Text ErrorLabelOfSignIn => _signInErrorLabel;
    public TMP_InputField EmailAddressInputAuth => _emailAddressInputAuth;
    public TMP_InputField UsernameInputAuth => _usernameInputAuth;
    public TMP_InputField PasswordInputAuth => _passwordInputAuth;
    public TMP_InputField UsernameInputSignIn => _usernameInputSign;
    public TMP_InputField PasswordInputSignIn => _passwordInputSign;
    public Button BtnBackToSignIn => _btnBackToSignIn;
    public Button BtnBackToAuth => _btnBackToAuth;
    public Button BtnNextSignIn => _btnSignIn;
    public Button BtnNextAuth => _btnAuth;
    public Button AutoCreateUserWithCustomId => _autoCreateUserWithCustomID;
    public GameObject PanelForAuth => _panelAuth;
    public GameObject PanelForSignIn => _panelSignIn;
    public PremitiveAnimation LoadingAnimation => _loadingAnimation;
    
    public void Cancel()
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

        _autoCreateUserWithCustomID.onClick.RemoveAllListeners();
    }

    private void OnDestroy()
    {
        Cancel();
    }
}
