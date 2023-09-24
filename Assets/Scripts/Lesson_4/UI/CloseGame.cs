using Photon.Pun;
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
        PhotonNetwork.LeaveRoom();
    }

    private void OnDestroy()
    {
        _btnClose.onClick.RemoveAllListeners();
    }
}
