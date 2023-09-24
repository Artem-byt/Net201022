using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab.ClientModels;
using System;
using ExitGames.Client.Photon;
using TMPro;
using UnityEngine.UI;

public class GameManger : MonoBehaviourPunCallbacks
{

    [Tooltip("The prefab to use for representing the player")]
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField] private PrepareCharacterUI _CharacterUI;
    [SerializeField] private SpawnModel spawnModel;


    [Header("Restart UI")]
    [SerializeField]
    private Button _restartBtn;
    [SerializeField]
    private TMP_Text _statusText;

    private void Start()
    {
        _CharacterUI.OnStartGame += InstantiatePlayer;
        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene(ConstantStrings.SCENE_MAIN);

            return;
        }

        if (PhotonNetwork.InRoom && PlayerManager.LocalPlayerInstance == null)
        {
            _CharacterUI.SwitchStateUICharacters(true, false);
        }
    }

    private void InstantiatePlayer(CharacterResult character)
    {
        if (PhotonNetwork.InRoom && PlayerManager.LocalPlayerInstance == null)
        {
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
            var spawn = spawnModel.GetSpawn();
           var go= PhotonNetwork.Instantiate(this.playerPrefab.name, spawn.position, Quaternion.identity, 0);
            go.GetComponentInChildren<PlayerManager>().CharacterResult = character;
            go.GetComponentInChildren<PlayerManager>().SpawnPosition = spawn;
            go.GetComponentInChildren<PlayerTryAttempts>().RestartButton = _restartBtn;
            go.GetComponentInChildren<PlayerTryAttempts>().Text = _statusText;
            go.GetComponentInChildren<PlayerTryAttempts>().Initialize();
        }
        else
        {

            Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitApplication();
        }
    }


    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
    }

    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.Log("OnPlayerEnteredRoom() " + other.NickName);
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
        SendOptions sendOptions = new SendOptions { Reliability = true };
        PhotonNetwork.RaiseEvent(2, spawnModel.IsFreePoition, raiseEventOptions, sendOptions);
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.Log("OnPlayerLeftRoom() " + other.NickName);
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(ConstantStrings.SCENE_MAIN);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        _CharacterUI.OnStartGame -= InstantiatePlayer;
    }


}
