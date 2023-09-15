using Photon.Pun.Demo.PunBasics;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab.ClientModels;
using System;

public class GameManger : MonoBehaviourPunCallbacks
{

    #region Public Fields

    #endregion

    #region Private Fields

    [Tooltip("The prefab to use for representing the player")]
    [SerializeField]
    private GameObject playerPrefab;

    [SerializeField] private PrepareCharacterUI _CharacterUI;

    #endregion

    #region MonoBehaviour CallBacks

    private void Start()
    {
        //in case we started this demo with the wrong scene being active, simply load the menu scene
        _CharacterUI.OnStartGame += InstantiatePlayer;

        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("Lesson_4");

            return;
        }

        if (PhotonNetwork.InRoom && PlayerManager.LocalPlayerInstance == null)
        {
            _CharacterUI.SwitchStateUICharacters(true, false);
        }



        //if (playerPrefab == null)
        //{ // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.

        //    Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        //}
        //else
        //{


        //    if (PhotonNetwork.InRoom && PlayerManager.LocalPlayerInstance == null)
        //    {
        //        Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

        //        // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate

        //        PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
        //    }
        //    else
        //    {

        //        Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
        //    }


        //}

    }

    private void InstantiatePlayer(CharacterResult character)
    {
        if (PhotonNetwork.InRoom && PlayerManager.LocalPlayerInstance == null)
        {
            Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

            // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate

           var go= PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
            go.GetComponentInChildren<PlayerManager>().CharacterResult = character;
        }
        else
        {

            Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
        }
    }


    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity on every frame.
    /// </summary>
    void Update()
    {
        // "back" button of phone equals "Escape". quit app if that's pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitApplication();
        }
    }

    #endregion

    #region Photon Callbacks

    public override void OnJoinedRoom()
    {
        // Note: it is possible that this monobehaviour is not created (or active) when OnJoinedRoom happens
        // due to that the Start() method also checks if the local player character was network instantiated!
        //_CharacterUI.SwitchStateUICharacters(true, false);
        //if (PlayerManager.LocalPlayerInstance == null)
        //{
        //    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

        //    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
        //    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
        //}
    }

    /// <summary>
    /// Called when a Photon Player got connected. We need to then load a bigger scene.
    /// </summary>
    /// <param name="other">Other.</param>
    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.Log("OnPlayerEnteredRoom() " + other.NickName); // not seen if you're the player connecting

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            LoadArena();
        }
    }

    /// <summary>
    /// Called when a Photon Player got disconnected. We need to load a smaller scene.
    /// </summary>
    /// <param name="other">Other.</param>
    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.Log("OnPlayerLeftRoom() " + other.NickName); // seen when other disconnects

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom

            LoadArena();
        }
    }

    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lesson_4");
    }

    #endregion

    #region Public Methods

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void QuitApplication()
    {
        Application.Quit();
    }

    #endregion

    #region Private Methods

    void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            return;
        }

        Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);

        PhotonNetwork.LoadLevel("PunBasics-Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
    }

    private void OnDestroy()
    {
        _CharacterUI.OnStartGame -= InstantiatePlayer;
    }

    #endregion

}
