using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.UI;

public class LobbyAdministrator : MonoBehaviour, ILobbyCallbacks, IConnectionCallbacks, IMatchmakingCallbacks
{
    [SerializeField] private ServerSettings _serverSettings;
    [SerializeField] private TMP_Text _statusUIText;
    [SerializeField] private LobbyUIWindow _windowUI;

    [SerializeField] private Button _lobbyButton;

    private LoadBalancingClient _lbc;
    private TypedLobby _defaultLobby = new TypedLobby("defaultLobby", LobbyType.Default);
    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();



    private void Start()
    {
        _lbc = new LoadBalancingClient();
        _lbc.AddCallbackTarget(this);


        _lbc.ConnectUsingSettings(_serverSettings.AppSettings);
        _lobbyButton.onClick.AddListener(OnConnectLobby);
        _windowUI.PublicRoom.onClick.AddListener(OnCreatePublic);
        _windowUI.PrivateRoom.onClick.AddListener(OnCreatePrivate);
        _windowUI.CloseLobby.onClick.AddListener(OnLeftLobbyBtn);

        _windowUI.UIRoomsHandler.OnClickButtonUI += OnBtnRoomClick;
        _windowUI.Connect.onClick.AddListener(OnConnectRoom);

        _windowUI.CloseRoom.onClick.AddListener(OnCloseRoom);
        _windowUI.OpenRoom.onClick.AddListener(OnOpenRoom);

        _windowUI.ConnectPrivateRoom.onClick.AddListener(OnPrivateRoomConnect);
        _windowUI.BtnCancelConnectToPrivateRoom.onClick.AddListener(OnClosePrivateRoomOptions);
        _windowUI.BtnAcceptConnectToPrivateRoom.onClick.AddListener(OnAcceptRoomOptions);
    }

    private void OnAcceptRoomOptions()
    {
        var enterRoomParams = new EnterRoomParams
        {
            RoomName = _windowUI.InputFieldNameOfPrivateRoom.text
        };
        _lbc.OpJoinRoom(enterRoomParams);
        _windowUI.PanelOptionsPrivateRoom.SetActive(false);
    }

    private void OnClosePrivateRoomOptions()
    {
        _windowUI.PanelOptionsPrivateRoom.SetActive(false);
    }

    private void OnPrivateRoomConnect()
    {
        _windowUI.PanelOptionsPrivateRoom.SetActive(true);
    }

    private void OnCloseRoom()
    {
        Debug.Log("Room closed");
        _lbc.CurrentRoom.IsOpen = false;
    }

    private void OnOpenRoom()
    {
        Debug.Log("Room opened");
        _lbc.CurrentRoom.IsOpen = true;
    }

    private void Update()
    {
        if (_lbc == null)
        {
            return;
        }

        _lbc.Service();

        var state = _lbc.State.ToString();
        _statusUIText.text = state;
    }

    private void OnConnectRoom()
    {
        _windowUI.CurrentRoom.Item1.image.color = Color.white;
        var enterRoomParams = new EnterRoomParams
        {
            RoomName = _windowUI.CurrentRoom.Item2.Name
        };
        _lbc.OpJoinRoom(enterRoomParams);
        _windowUI.CurrentRoom = (null, null);
        _windowUI.Connect.interactable = false;
    }

    private void OnBtnRoomClick(Button button, RoomInfo roomInfo)
    {
        _windowUI.CurrentRoom = (button, roomInfo);
        button.image.color = Color.gray;
        _windowUI.Connect.interactable = true;
    }

    private void OnCreatePublic()
    {
        var roomOptions = new RoomOptions
        {
            MaxPlayers = 6,
            PublishUserId = true
        };

        var enterRoomParams = new EnterRoomParams
        {
            RoomOptions = roomOptions,
            Lobby = _defaultLobby
        };

        _lbc.OpCreateRoom(enterRoomParams);
    }

    private void OnCreatePrivate()
    {
        var roomOptions = new RoomOptions
        {
            MaxPlayers = 6,
            IsVisible = false,
            PublishUserId = true
        };

        var roomName = "PrivateRoom_" + Random.Range(0, 1000000);
        var enterRoomParams = new EnterRoomParams
        {
            RoomName = roomName,
            RoomOptions = roomOptions,
            Lobby = _defaultLobby
        };
        GUIUtility.systemCopyBuffer = roomName;
        Debug.Log("Имя комнаты скопировано в буфер обмена: " + roomName);
        _lbc.OpCreateRoom(enterRoomParams);
    }

    private void OnConnectLobby()
    {
        _windowUI.gameObject.SetActive(true);
        _lobbyButton.interactable = false;
        _lbc.OpJoinLobby(_defaultLobby);
    }
    private void OnLeftLobbyBtn()
    {
        _lbc.OpLeaveLobby();
        _lobbyButton.interactable = true; ;
        _windowUI.LobbyWindow.SetActive(false);
        _windowUI.Connect.interactable = false;
    }

    private void UpdateCacheRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
    }

    public void OnConnected()
    {
        Debug.Log("On Connected");
    }

    public void OnConnectedToMaster()
    {
        Debug.Log("On Connected To Master");
        _lobbyButton.interactable = true;
    }

    public void OnCreatedRoom()
    {
        Debug.Log("On Created Room");
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Create Room Failed");
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {

    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {

    }

    public void OnDisconnected(DisconnectCause cause)
    {
        cachedRoomList.Clear();
        _windowUI.UIRoomsHandler.ClearList();
    }

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {

    }

    public void OnJoinedLobby()
    {
        cachedRoomList.Clear();
        _windowUI.UIRoomsHandler.ClearList();
        _windowUI.LobbyWindow.SetActive(true);
    }

    public void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        _windowUI.CloseRoom.interactable = true;
        _windowUI.OpenRoom.interactable = true;
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Join Random Room Failed");
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Join Room Failed");
    }

    public void OnLeftLobby()
    {
        cachedRoomList.Clear();
        _windowUI.UIRoomsHandler.ClearList();
        Debug.Log("Left Lobby");
    }

    public void OnLeftRoom()
    {
        _windowUI.CloseRoom.interactable = false;
        _windowUI.OpenRoom.interactable = false;
    }

    public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {

    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {

    }

    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Update Room List");
        UpdateCacheRoomList(roomList);
        _windowUI.UIRoomsHandler.HandleUI(roomList);

    }

    private void OnDestroy()
    {
        _lbc.RemoveCallbackTarget(this);
        _lobbyButton.onClick.RemoveAllListeners();
        _windowUI.PublicRoom.onClick.RemoveAllListeners();
        _windowUI.PrivateRoom.onClick.RemoveAllListeners();
        _windowUI.CloseLobby.onClick.RemoveAllListeners();

        _windowUI.UIRoomsHandler.OnClickButtonUI -= OnBtnRoomClick;
        _windowUI.Connect.onClick.RemoveAllListeners();


        _windowUI.CloseRoom.onClick.RemoveAllListeners();
        _windowUI.OpenRoom.onClick.RemoveAllListeners();

        _windowUI.ConnectPrivateRoom.onClick.RemoveAllListeners();
        _windowUI.BtnCancelConnectToPrivateRoom.onClick.RemoveAllListeners();
        _windowUI.BtnAcceptConnectToPrivateRoom.onClick.RemoveAllListeners();
    }


}
