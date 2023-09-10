using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyAdministrator : MonoBehaviour, ILobbyCallbacks, IConnectionCallbacks, IMatchmakingCallbacks
{
    [SerializeField] private ServerSettings _serverSettings;
    [SerializeField] private TMP_Text _statusUIText;
    [SerializeField] private GameObject _lobbyUIGameObject;
    [SerializeField] private UIRoomsHandler _uiRoomsHandler;
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

        var enterRoomParams = new EnterRoomParams
        {
            RoomName = "PrivateRoom",
            RoomOptions = roomOptions,
            Lobby = _defaultLobby
        };

        _lbc.OpCreateRoom(enterRoomParams);
    }

    private void OnConnectLobby()
    {
        _windowUI.gameObject.SetActive(true);
        _lbc.OpJoinLobby(_defaultLobby);
    }
    private void OnLeftLobbyBtn()
    {
        _lbc.OpLeaveLobby();
        _windowUI.LobbyWindow.SetActive(false);
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
        _uiRoomsHandler.ClearList();
    }

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        
    }

    public void OnJoinedLobby()
    {
        cachedRoomList.Clear();
        _uiRoomsHandler.ClearList();
        _lobbyUIGameObject.SetActive(true);
    }

    public void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
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
        _uiRoomsHandler.ClearList();
        Debug.Log("Left Lobby");
    }

    public void OnLeftRoom()
    {
        
    }

    public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
        
    }

    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCacheRoomList(roomList);
        _uiRoomsHandler.HandleUI(roomList);

    }

    private void OnDestroy()
    {
        _lbc.RemoveCallbackTarget(this);
        _lobbyButton.onClick.RemoveAllListeners();
        _windowUI.PublicRoom.onClick.RemoveAllListeners();
        _windowUI.PrivateRoom.onClick.RemoveAllListeners();
        _windowUI.CloseLobby.onClick.RemoveAllListeners();
    }

 
}
