using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyAdministrator : MonoBehaviour, ILobbyCallbacks, IConnectionCallbacks, IMatchmakingCallbacks
{
    [SerializeField] private ServerSettings _serverSettings;
    [SerializeField] private TMP_Text _statusUIText;
    [SerializeField] private LobbyUIWindow _windowUI;

    [SerializeField] private Button _lobbyButton;

    private TypedLobby _defaultLobby = new TypedLobby("defaultLobby", LobbyType.Default);
    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();


    [Header("The maximum number of players per room")]
    [SerializeField] private byte maxPlayersPerRoom = 4;


    private void Start()
    {
        PhotonNetwork.AddCallbackTarget(this);
        PhotonNetwork.AutomaticallySyncScene = true;


        PhotonNetwork.ConnectUsingSettings(_serverSettings.AppSettings);
        _lobbyButton.onClick.AddListener(OnConnectLobby);
        _windowUI.PublicRoom.onClick.AddListener(OnCreatePublic);
        _windowUI.PrivateRoom.onClick.AddListener(OnCreatePrivate);
        _windowUI.CloseLobby.onClick.AddListener(OnLeftLobbyBtn);

        _windowUI.UIRoomsHandler.OnClickButtonUI += OnBtnRoomClick;
        _windowUI.Connect.onClick.AddListener(OnConnectRoom);

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
        PhotonNetwork.NetworkingClient.OpJoinRoom(enterRoomParams);
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

    private void Update()
    {
        if (PhotonNetwork.NetworkingClient == null)
        {
            return;
        }

        PhotonNetwork.NetworkingClient.Service();

        var state = PhotonNetwork.NetworkingClient.State.ToString();
        _statusUIText.text = state;
    }

    private void OnConnectRoom()
    {
        _windowUI.CurrentRoom.Item1.image.color = Color.white;
        var enterRoomParams = new EnterRoomParams
        {
            RoomName = _windowUI.CurrentRoom.Item2.Name
        };
        PhotonNetwork.NetworkingClient.OpJoinRoom(enterRoomParams);
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
            MaxPlayers = maxPlayersPerRoom,
            PublishUserId = true
        };

        var enterRoomParams = new EnterRoomParams
        {
            RoomOptions = roomOptions,
            Lobby = _defaultLobby
        };

        PhotonNetwork.NetworkingClient.OpCreateRoom(enterRoomParams);
    }

    private void OnCreatePrivate()
    {
        var roomOptions = new RoomOptions
        {
            MaxPlayers = maxPlayersPerRoom,
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
        PhotonNetwork.NetworkingClient.OpCreateRoom(enterRoomParams);
    }

    private void OnConnectLobby()
    {
        _windowUI.gameObject.SetActive(true);
        _lobbyButton.interactable = false;
        PhotonNetwork.NetworkingClient.OpJoinLobby(_defaultLobby);
    }
    private void OnLeftLobbyBtn()
    {
        PhotonNetwork.NetworkingClient.OpLeaveLobby();
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
        Debug.Log("Connected");
    }

    public void OnConnectedToMaster()
    {
        Debug.Log("On Connected To Master");
        _lobbyButton.interactable = true;
        PlayFabLoginCall.GetAccountInfo(OnSuccessGetAccountInfo, OnFailure, new GetAccountInfoRequest()); 
    }

    private void OnSuccessGetAccountInfo(GetAccountInfoResult result)
    {
        Debug.Log(result.AccountInfo.Username + " : Username");
        PhotonNetwork.NickName = result.AccountInfo.Username;
    }

    private void OnFailure(PlayFabError error)
    {
        Debug.Log("Ошибка получения данных об игроке: " + error);
    }

    public void OnCreatedRoom()
    {
        Debug.Log("On Created Room");
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Create Room Failed: {returnCode} - {message}");
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
        Debug.Log(debugMessage);
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {

    }

    public void OnFriendListUpdate(List<Photon.Realtime.FriendInfo> friendList)
    {

    }

    public void OnDisconnected(DisconnectCause cause)
    {
        cachedRoomList.Clear();
        _windowUI.UIRoomsHandler.ClearList();
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

        if (PhotonNetwork.NetworkingClient.CurrentRoom.PlayerCount == 1)
        {
            Debug.Log("Loading the 'Room for 1' ");
            PhotonNetwork.LoadLevel(ConstantStrings.SCENE_LEVEL_1);
        }
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Join Random Room Failed: {returnCode} - {message}");
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"Join Room Failed: {returnCode} - {message}");
    }

    public void OnLeftLobby()
    {
        Debug.Log("Left Lobby");
        cachedRoomList.Clear();
        _windowUI.UIRoomsHandler.ClearList();
    }


    public void OnLeftRoom()
    {
        Debug.Log("Left Room");
    }

    public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
        Debug.Log("Lobby Statistics Update");
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
        Debug.Log("Region List Received");
    }

    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Update Room List");
        UpdateCacheRoomList(roomList);
        _windowUI.UIRoomsHandler.HandleUI(roomList);

    }

    private void OnDestroy()
    {
        PhotonNetwork.NetworkingClient.RemoveCallbackTarget(this);
        _lobbyButton.onClick.RemoveAllListeners();
        _windowUI.PublicRoom.onClick.RemoveAllListeners();
        _windowUI.PrivateRoom.onClick.RemoveAllListeners();
        _windowUI.CloseLobby.onClick.RemoveAllListeners();

        _windowUI.UIRoomsHandler.OnClickButtonUI -= OnBtnRoomClick;
        _windowUI.Connect.onClick.RemoveAllListeners();

        _windowUI.ConnectPrivateRoom.onClick.RemoveAllListeners();
        _windowUI.BtnCancelConnectToPrivateRoom.onClick.RemoveAllListeners();
        _windowUI.BtnAcceptConnectToPrivateRoom.onClick.RemoveAllListeners();
    }


}
