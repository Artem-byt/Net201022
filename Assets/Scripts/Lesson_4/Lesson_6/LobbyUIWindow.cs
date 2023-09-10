using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIWindow : MonoBehaviour
{
    public Button CloseRoom;
    public Button OpenRoom;
    public Button Connect;
    public Button CloseLobby;
    public Button PrivateRoom;
    public Button PublicRoom;
    public GameObject LobbyWindow;
    public UIRoomsHandler UIRoomsHandler;

    public Button ConnectPrivateRoom;
    public GameObject PanelOptionsPrivateRoom;
    public Button BtnCancelConnectToPrivateRoom;
    public Button BtnAcceptConnectToPrivateRoom;
    public TMP_InputField InputFieldNameOfPrivateRoom;

    public Button _sqlLobbyConnect;

    public (Button, RoomInfo) CurrentRoom;
}
