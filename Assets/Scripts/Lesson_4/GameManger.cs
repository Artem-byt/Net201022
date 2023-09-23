using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab.ClientModels;
using System;

public class GameManger : MonoBehaviourPunCallbacks
{

    [Tooltip("The prefab to use for representing the player")]
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField] private PrepareCharacterUI _CharacterUI;


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
           var go= PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            go.GetComponentInChildren<PlayerManager>().CharacterResult = character;
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
