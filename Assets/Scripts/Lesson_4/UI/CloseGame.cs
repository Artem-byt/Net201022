using Photon.Pun;
using PlayFab;
using UnityEngine;
using UnityEngine.UI;

public class CloseGame : MonoBehaviour
{
    [SerializeField] private Button _btnClose;

    private void Start()
    {
        _btnClose.onClick.AddListener(CloseGameApp);
    }

    private void CloseGameApp()
    {
        PlayFabClientAPI.ForgetAllCredentials();
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
    }

    private void OnDestroy()
    {
        _btnClose.onClick.RemoveAllListeners();
    }
}
