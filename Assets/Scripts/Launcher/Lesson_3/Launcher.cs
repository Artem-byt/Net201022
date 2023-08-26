using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] private string _playFabTitle;
    [SerializeField] private Button _connectPhotonBtn;
    [SerializeField] private Button _logInBtn;
    [SerializeField] private TMP_Text _textConnectStatus;
    [SerializeField] private TMP_Text _textLogInStatus;

    private string gameVersion = "1";

    private void Start()
    {
        _logInBtn.onClick.AddListener(LogIn);
        _connectPhotonBtn.onClick.AddListener(Connect);
    }

    private void LogIn()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            PlayFabSettings.staticSettings.TitleId = _playFabTitle;

        var request = new LoginWithCustomIDRequest
        {
            CustomId = "GeekBrains_Sem4",
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request,
            result =>
            {
                Debug.Log(result.PlayFabId);
                PhotonNetwork.AuthValues = new AuthenticationValues(result.PlayFabId);
                PhotonNetwork.NickName = result.PlayFabId;
                CastomizeTextFiels(Color.green, "OK");
                _logInBtn.interactable = false;
                _connectPhotonBtn.interactable = true;
            },
            error => 
            {
                Debug.LogError(error);
                CastomizeTextFiels(Color.red, "FALSE");
            });
    }

    private void Connect()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomOrCreateRoom(roomName: $"Room N{UnityEngine.Random.Range(0, 9999)}");
        }
        else
        {
            if (!PhotonNetwork.ConnectUsingSettings())
            {
                Debug.Log("User wasn't connected to the Server");
            }
            PhotonNetwork.GameVersion = gameVersion;
            _logInBtn.interactable = false;
        }
    }

    public void Disconnect()
    {
        PhotonNetwork.Disconnect();
        Debug.Log("User was Disconnected from the Server");
    }

    private void CastomizeTextFiels(Color textColor, string text)
    {
        _textLogInStatus.text = text;
        _textLogInStatus.color = textColor;
    }

    private void CastomizeButtonConnect(Color btnColor, Color textColor, string btnText, UnityAction callback)
    {
        _connectPhotonBtn.image.color = btnColor;
        _textConnectStatus.text = btnText;
        _textConnectStatus.color = textColor;
        _connectPhotonBtn.onClick.RemoveAllListeners();
        _connectPhotonBtn.onClick.AddListener(callback);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        CastomizeButtonConnect(Color.red, Color.white, "Connect Photon", Connect);
        _logInBtn.interactable = true;
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("OnConnectedToMaster");
        CastomizeButtonConnect(Color.green, Color.black, "Disconnect Photon", Disconnect);
        if (!PhotonNetwork.InRoom)
            PhotonNetwork.JoinRandomOrCreateRoom(roomName: $"Room N{UnityEngine.Random.Range(0, 9999)}");
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("OnCreatedRoom");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log($"OnJoinedRoom {PhotonNetwork.CurrentRoom.Name}");
    }

    private void OnDestroy()
    {
        _logInBtn.onClick.RemoveAllListeners();
        _connectPhotonBtn.onClick.RemoveAllListeners();
    }
}
