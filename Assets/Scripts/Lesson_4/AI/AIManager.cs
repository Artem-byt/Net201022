using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using PlayFab.ClientModels;
using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.AI;

public class AIManager : MonoBehaviourPunCallbacks, IPunObservable
{

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    [SerializeField]
    private Rigidbody _rigidBody;

    [SerializeField]
    private float _force = 2f;


    [SerializeField]
    private PlayerSoundManager _soundManager;

    [SerializeField]
    private NavMeshAgent _navMeshAgent;

    [Tooltip("The Beams GameObject to control")]
    [SerializeField]
    private GameObject _beams;

    bool IsFiring;
    private bool _isEndGame;
    private Animator _animator;

    public List<Transform> Points= new List<Transform>();


    private bool _isPatrolling = true;
    public bool IsReady = false;

    public void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
        if (this._beams == null)
        {
            Debug.LogError("<Color=Red><b>Missing</b></Color> Beams Reference.", this);
        }
        else
        {
            this._beams.SetActive(false);
        }

        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }
        _animator = GetComponent<Animator>();

        DontDestroyOnLoad(gameObject);
    }

    public void Start()
    {

#if UNITY_5_4_OR_NEWER
        // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
#endif
    }

    public override void OnDisable()
    {
        base.OnDisable();

#if UNITY_5_4_OR_NEWER
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
#endif
    }

    public void SetReary()
    {
        IsReady = true;
        _navMeshAgent.SetDestination(Points[3].position);
    }

    private void Update()
    {
        if (!_isEndGame && IsReady && photonView.IsMine)
        {
            PatrollingState();
            FollowPlayerState();
        }

        if (this._beams != null && this.IsFiring != this._beams.activeInHierarchy && !_isEndGame)
        {
            this._beams.SetActive(this.IsFiring);
        }

        if (this._beams.activeInHierarchy)
        {
            _soundManager.LaserSound();
        }
    }

    private void PatrollingState()
    {
        if (!_isPatrolling)
        {
            return;
        }
        if (_navMeshAgent.remainingDistance < _navMeshAgent.stoppingDistance && _navMeshAgent.path != null)
        {
            _navMeshAgent.ResetPath();
            _navMeshAgent.SetDestination(Points[Random.Range(0, 3)].position);
        }
    }

    private void FollowPlayerState()
    {
        Ray ray = new Ray { direction = transform.forward, origin = transform.position };
        RaycastHit raycastHit = new RaycastHit();
        var raycast = Physics.Raycast(ray, out raycastHit);
        Debug.Log(raycast + " : raycast");
        if (raycast)
        {
            var playerManager = raycastHit.collider.gameObject.GetComponent<PlayerManager>();
            Debug.Log(playerManager + " : player manager");
            if (playerManager != null)
            {
                _isPatrolling = false;
                _navMeshAgent.ResetPath();
                _navMeshAgent.SetDestination(raycastHit.collider.gameObject.transform.position);
                AttackPlayerState(_navMeshAgent.remainingDistance);
            }
            else
            {
                _isPatrolling = true;
                IsFiring = false;
            }
        }
    }

    private void AttackPlayerState(float distanceToPlayer)
    {
        if (distanceToPlayer <= 1f)
        {
            _navMeshAgent.ResetPath();
            IsFiring = true;
        }
        else
        {
            IsFiring = false;
        }
    }

    public override void OnLeftRoom()
    {

    }


#if !UNITY_5_4_OR_NEWER
        /// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }
#endif

    void CalledOnLevelWasLoaded(int level)
    {
        // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
        if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
        {
            transform.position = new Vector3(0f, 5f, 0f);
        }

    }



#if UNITY_5_4_OR_NEWER
    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
    {
        this.CalledOnLevelWasLoaded(scene.buildIndex);
    }
#endif


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(this.IsFiring);
        }
        else
        {
            // Network player, receive data
            this.IsFiring = (bool)stream.ReceiveNext();
        }


    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case 3:
                Debug.Log("EndGame");
                _isEndGame = true;
                _navMeshAgent.Stop();
                _navMeshAgent.ResetPath();
                break;
        }



    }

}
