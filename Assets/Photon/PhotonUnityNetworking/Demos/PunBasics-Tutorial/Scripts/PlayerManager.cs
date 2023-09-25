using ExitGames.Client.Photon;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Photon.Pun.Demo.PunBasics
{
#pragma warning disable 649

    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable, IOnEventCallback
    {
        //[Tooltip("The current Health of our player")]
        //public float Health = 1f;
        public float CurrentHealth;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        public CharacterResult CharacterResult { get; set; }

        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        private GameObject _playerUiPrefab;

        [SerializeField]
        private GameObject _endGamePrefab;

        [SerializeField]
        private GameObject _playerUiStatsPrefab;

        [SerializeField]
        private Rigidbody _rigidBody;

        [SerializeField]
        private float _force = 2f;

        [Tooltip("The Beams GameObject to control")]
        [SerializeField]
        private GameObject _beams;

        [SerializeField]
        private PlayerTryAttempts _playerTryAttempts;

        [SerializeField]
        private PLayerGroundChecker _pLayerGroundChecker;

        bool IsFiring;
        private bool _isEndGame;

        private float _id;

        private string _playFabId;
        private GameObject _endGameUI;
        public Transform SpawnPosition;

        public float Id
        {
            get => photonView.ViewID;
            set => _id = value;
        }

        private CharacterStatsUI _characterPlayUI;
        private int _damage;
        private float _damageModifier = 0.1f;

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

            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
            }

            PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), result =>
            {
                _playFabId = result.AccountInfo.PlayFabId;
                Debug.Log(_playFabId + " : PlayFabId");
            }, error => Debug.Log("Error playFabId"));


            CurrentHealth = 1f;

            DontDestroyOnLoad(gameObject);
        }

        private void UpdateClientDamage(Dictionary<string, int> statistics)
        {
            _damage = statistics[CharacterPlayFabCall.DAMAGE];
        }

        private void GetUserData(string PlayFabId, string keyData)
        {
            PlayFabClientAPI.GetUserData(new GetUserDataRequest
            {
                PlayFabId = _playFabId
            },
             result =>
             {
                 if (result.Data.ContainsKey(keyData))
                 {
                     Debug.Log($"{keyData}: {result.Data[keyData].Value} : {this.gameObject.name} : {_playFabId}");
                     CurrentHealth = float.Parse(result.Data[keyData].Value);
                 }
             },
             error => Debug.Log("OnGetDataError"));
        }

        public void SpawnPlayer()
        {
            transform.position = SpawnPosition.position;
            gameObject.SetActive(true);
        }

        public void Start()
        {
            CameraWork _cameraWork = gameObject.GetComponent<CameraWork>();

            if (_cameraWork != null)
            {
                if (photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> CameraWork Component on player Prefab.", this);
            }

            if (_playerUiStatsPrefab != null && photonView.IsMine)
            {
                _characterPlayUI = Instantiate(this._playerUiStatsPrefab).GetComponentInChildren<CharacterStatsUI>();
                _characterPlayUI.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }

            if (_playerUiStatsPrefab != null && photonView.IsMine)
            {
                _endGameUI = Instantiate(_endGamePrefab);
                _endGameUI.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
            }

            if (this._playerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(this._playerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><b>Missing</b></Color> PlayerUiPrefab reference on player Prefab.", this);
            }
            if (photonView.IsMine)
            {
                CharacterPlayFabCall.GetCHaracterStatistics(UpdateClientDamage, CharacterResult.CharacterId);
            }
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

        private bool leavingRoom;

        private void Update()
        {
            if (photonView.IsMine && !_isEndGame)
            {
                this.ProcessInputs();
            }

            if (this._beams != null && this.IsFiring != this._beams.activeInHierarchy && !_isEndGame)
            {
                this._beams.SetActive(this.IsFiring);
            }

        }

        private void ShowInventory(GetUserInventoryResult result)
        {
            var firstItem = result.Inventory.First();
            ConsumeOption(firstItem.ItemInstanceId);
        }

        private void ConsumeOption(string itemInstanceId)
        {
            PlayFabClientAPI.ConsumeItem(new ConsumeItemRequest
            {
                ConsumeCount = 1,
                ItemInstanceId = itemInstanceId
            }, result =>
            {
                Debug.Log("Complete ConsumeItem");
                CurrentHealth += 0.5f;
            }, error => Debug.Log("Error ConsumeItem"));
        }

        public override void OnLeftRoom()
        {
            this.leavingRoom = false;
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }

            if (!other.name.Contains("Beam"))
            {
                return;
            }
             ;
            CurrentHealth -= _damage * _damageModifier;
            if (CurrentHealth < 0)
            {
                Debug.Log("SendData");
                int killId = other.GetComponentInParent<PlayerManager>().photonView.ViewID;
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
                SendOptions sendOptions = new SendOptions { Reliability = true };
                PhotonNetwork.RaiseEvent(1, killId, raiseEventOptions, sendOptions);
                Debug.Log(CurrentHealth.ToString() + " : CurrentHealth");
                gameObject.SetActive(false);
                _playerTryAttempts.OnTriedAttempt();
                
            }
        }

        public void OnTriggerStay(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }

            if (!other.name.Contains("Beam"))
            {
                return;
            }

            CurrentHealth -= _damage * _damageModifier * Time.deltaTime;
            if (CurrentHealth < 0)
            {
                Debug.Log("SendData");
                int killId = other.GetComponentInParent<PlayerManager>().photonView.ViewID;
                RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
                SendOptions sendOptions = new SendOptions { Reliability = true };
                PhotonNetwork.RaiseEvent(1, killId, raiseEventOptions, sendOptions);
                Debug.Log(CurrentHealth.ToString() + " : CurrentHealth");
                gameObject.SetActive(false);
                _playerTryAttempts.OnTriedAttempt();
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }
            if (!other.name.Contains("Beam"))
            {
                return;
            }



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

            GameObject _uiGo = Instantiate(this._playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);

            if (photonView.IsMine)
            {
                _characterPlayUI = Instantiate(this._playerUiStatsPrefab).GetComponentInChildren<CharacterStatsUI>();
                _characterPlayUI.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }

        }



#if UNITY_5_4_OR_NEWER
        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }
#endif

        void ProcessInputs()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    //	return;
                }

                if (!this.IsFiring)
                {
                    this.IsFiring = true;
                }
            }

            if (Input.GetKeyDown(KeyCode.Space) && _pLayerGroundChecker.isGround)
            {
                _rigidBody.AddForce(Vector3.up * _force);

            }

            if (Input.GetButtonUp("Fire1"))
            {
                this.IsFiring = false;
            }
            RotatePlayer();

        }

        private void RotatePlayer()
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            Vector3 forward = Camera.main.transform.forward;
            Vector3 right= Camera.main.transform.right;
            forward.y = 0;
            right.y = 0;
            forward = forward.normalized;
            right= right.normalized;

            Vector3 forwardRelativeVerticalInput = v * forward;
            Vector3 roightRelativeVerticalInput = h * right;
            var movementDirection
              = forwardRelativeVerticalInput + roightRelativeVerticalInput;
            if (movementDirection.sqrMagnitude > 1.0f)
            {
                movementDirection.Normalize();
            }
            Vector3 rotationTarget = this.transform.position + movementDirection;
            this.transform.LookAt(rotationTarget);
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(this.IsFiring);
                stream.SendNext(CurrentHealth);
                stream.SendNext(this.Id);
            }
            else
            {
                // Network player, receive data
                this.IsFiring = (bool)stream.ReceiveNext();
                CurrentHealth = (float)stream.ReceiveNext();
                this.Id = (float)stream.ReceiveNext();
            }


        }

        public void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case 1:
                    int killId = (int)photonEvent.CustomData;
                    Debug.Log("GetData + " + killId.ToString());
                    if (killId == photonView.ViewID)
                    {

                        ChangeClientStatistics();
                    }
                    break;
                case 3:
                    Debug.Log("EndGame");
                    _isEndGame = true;
                    this._beams.SetActive(false);
                    float id = (float)photonEvent.CustomData;
                    if (id == photonView.ViewID)
                    {
                        _endGameUI.GetComponent<TMP_Text>().text = "Победитель";
                        ChangeClientStatistics();
                    }
                    else
                    {
                        _endGameUI.GetComponent<TMP_Text>().text = "Проигравший";
                    }
                    break;
            }



        }

        private void ChangeClientStatistics()
        {
            CharacterPlayFabCall.GetCHaracterStatistics(AddCharacterXP, CharacterResult.CharacterId);
        }

        private void AddCharacterXP(Dictionary<string, int> currentstatistics)
        {
            currentstatistics[CharacterPlayFabCall.XP] += 123;
            //Добавить метод повышения уровня, всегда до 500 ХР
            CharacterPlayFabCall.UpdateCharacterStatistics(UpdateUIStatistics, CharacterResult.CharacterId, currentstatistics);

        }

        private void UpdateUIStatistics()
        {
            CharacterPlayFabCall.GetCHaracterStatistics(_characterPlayUI.UpdateUIStatistics, CharacterResult.CharacterId);
        }
    }
}