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

    [SerializeField]
    private GameObject aiPrefab;

    [SerializeField] private PrepareCharacterUI _CharacterUI;
    [SerializeField] private SpawnModel _spawnModel;


    [Header("Restart UI")]
    [SerializeField]
    private Button _restartBtn;
    [SerializeField]
    private TMP_Text _statusText;

    [SerializeField]
    private AudioSource _sourceHit;

    private void Start()
    {
        _CharacterUI.OnStartGame += InstantiatePlayer;
        _CharacterUI.OnStartGame += InstantiateAI;
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
            var go = PhotonNetwork.Instantiate(this.playerPrefab.name, _spawnModel.FreePosition.position, Quaternion.identity, 0);
            go.GetComponentInChildren<PlayerManager>().SourceHit = _sourceHit;
            go.GetComponentInChildren<PlayerManager>().CharacterResult = character;
            go.GetComponentInChildren<PlayerManager>().SpawnPosition = _spawnModel.FreePosition.position;
            go.GetComponentInChildren<PlayerTryAttempts>().RestartButton = _restartBtn;
            go.GetComponentInChildren<PlayerTryAttempts>().Text = _statusText;
            go.GetComponentInChildren<PlayerTryAttempts>().Initialize();
        }
        else
        {

            Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
        }
    }

    private void InstantiateAI(CharacterResult character)
    {
        if (PhotonNetwork.InRoom && !_spawnModel.isNewPlayer)
        {
            var go = PhotonNetwork.Instantiate(this.aiPrefab.name, _spawnModel.GetAISpawn().position, Quaternion.identity, 0).GetComponent<AIManager>();
            go.Points = _spawnModel.TransformsAI;
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
        PhotonNetwork.RaiseEvent(2, _spawnModel.IsFreePoition, raiseEventOptions, sendOptions);
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
        _CharacterUI.OnStartGame -= InstantiateAI;
    }


}
