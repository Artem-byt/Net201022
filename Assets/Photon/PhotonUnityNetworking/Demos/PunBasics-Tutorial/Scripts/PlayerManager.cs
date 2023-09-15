// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerManager.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in PUN Basics Tutorial to deal with the networked player instance
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Photon.Pun.Demo.PunBasics
{
#pragma warning disable 649

    /// <summary>
    /// Player manager.
    /// Handles fire Input and Beams.
    /// </summary>
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region Public Fields

        //[Tooltip("The current Health of our player")]
        //public float Health = 1f;
        public float CurrentHealth;

        [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
        public static GameObject LocalPlayerInstance;

        public CharacterResult CharacterResult;

        public float KillId = -1;

        #endregion

        #region Private Fields

        [Tooltip("The Player's UI GameObject Prefab")]
        [SerializeField]
        private GameObject playerUiPrefab;

        [SerializeField]
        private GameObject playerUiStatsPrefab;

        [Tooltip("The Beams GameObject to control")]
        [SerializeField]
        private GameObject beams;

        //True, when the user is firing
        bool IsFiring;
        bool IsHealing;

        private float _id;

        private string _playFabId;

        public float Id
        {
            get => photonView.ViewID;
            set => _id = value;
        }

        private CharacterPlayUI _characterPlayUI;

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
        /// </summary>
        public void Awake()
        {
            if (this.beams == null)
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> Beams Reference.", this);
            }
            else
            {
                this.beams.SetActive(false);
            }

            // #Important
            // used in GameManager.cs: we keep track of the localPlayer instance to prevent instanciation when levels are synchronized
            if (photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
                MakePurchase();
            }

            PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), result =>
            {
                _playFabId = result.AccountInfo.PlayFabId;
                Debug.Log(_playFabId + " : PlayFabId");
            }, error => Debug.Log("Error playFabId"));

            CurrentHealth = 1f;
            SetData("1");


            // #Critical
            // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
            DontDestroyOnLoad(gameObject);
        }

        private void SetData(string health)
        {
            PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
            {
                Data = new Dictionary<string, string>
                {
                   { "Health", health}
                }
            },
           result =>
           {
               Debug.Log("Update User Data");
               GetUserData(_playFabId, "Health");
           },
           error => Debug.Log("OnLoginError"));
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

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
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

            // Create the UI
            
            if (this.playerUiStatsPrefab != null && photonView.IsMine)
            {
                GameObject _uiGo = Instantiate(this.playerUiStatsPrefab);
                _uiGo.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
                _characterPlayUI = _uiGo.GetComponent<CharacterPlayUI>();
                CharacterPlayFabCall.GetCHaracterStatistics(UpdateUIStatistics, CharacterResult.CharacterId);
            }

            if (this.playerUiPrefab != null)
            {
                GameObject _uiGo = Instantiate(this.playerUiPrefab);
                _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><b>Missing</b></Color> PlayerUiPrefab reference on player Prefab.", this);
            }

#if UNITY_5_4_OR_NEWER
            // Unity 5.4 has a new scene management. register a method to call CalledOnLevelWasLoaded.
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
#endif
        }

        public void UpdateUIStatistics(Dictionary<string, int> statistics)
        {
            _characterPlayUI.XpUI.text = "XP " + statistics[CharacterPlayFabCall.XP].ToString();
            _characterPlayUI.GoldUI.text = "GOLD " + statistics[CharacterPlayFabCall.GOLD].ToString();
            _characterPlayUI.LevelUI.text = "Level " + statistics[CharacterPlayFabCall.LEVEL].ToString();
            _characterPlayUI.DamageUI.text = "Damage " + statistics[CharacterPlayFabCall.DAMAGE].ToString();
            _characterPlayUI.MaxHPUI.text = "Max HP " + statistics[CharacterPlayFabCall.HP].ToString();
        }

        private void MakePurchase()
        {
            PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
            {
                CatalogVersion = "1.0",
                ItemId = "health_potion",
                Price = 2,
                VirtualCurrency = "GO"

            }, result =>
            {
                Debug.Log("Complete Purchase");
            }, error => Debug.Log("Error Purchase"));
        }

        public override void OnDisable()
        {
            // Always call the base to remove callbacks
            base.OnDisable();

#if UNITY_5_4_OR_NEWER
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
#endif
        }

        private bool leavingRoom;

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity on every frame.
        /// Process Inputs if local player.
        /// Show and hide the beams
        /// Watch for end of game, when local player health is 0.
        /// </summary>
        private void Update()
        {
            // we only process Inputs and check health if we are the local player
            if (photonView.IsMine)
            {
                this.ProcessInputs();
                if (CurrentHealth <= 0f && !this.leavingRoom)
                {
                    Debug.Log(CurrentHealth.ToString() + " : CurrentHealth");
                    this.leavingRoom = PhotonNetwork.LeaveRoom();
                }

                if (this.IsHealing)
                {
                    this.IsHealing = false;
                    PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), result => ShowInventory(result), error => { Debug.Log("Error Healing: " + error.GenerateErrorReport()); });
                }
                
                if (KillId == Id)
                {
                    Debug.Log("Almost Done !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                }
            }

            if (this.beams != null && this.IsFiring != this.beams.activeInHierarchy)
            {
                this.beams.SetActive(this.IsFiring);
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
                SetData(CurrentHealth.ToString());
            }, error => Debug.Log("Error ConsumeItem"));
        }

        public override void OnLeftRoom()
        {
            this.leavingRoom = false;
        }

        /// <summary>
        /// MonoBehaviour method called when the Collider 'other' enters the trigger.
        /// Affect Health of the Player if the collider is a beam
        /// Note: when jumping and firing at the same, you'll find that the player's own beam intersects with itself
        /// One could move the collider further away to prevent this or check if the beam belongs to the player.
        /// </summary>
        public void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }


            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            if (!other.name.Contains("Beam"))
            {
                return;
            }
             ;
            CurrentHealth -= 0.1f;
            //if (CurrentHealth < 0)
            //{
                KillId = other.gameObject.GetComponentInParent<PlayerManager>().Id;
            Debug.Log(KillId);
            //}
            SetData(CurrentHealth.ToString());
        }

        /// <summary>
        /// MonoBehaviour method called once per frame for every Collider 'other' that is touching the trigger.
        /// We're going to affect health while the beams are interesting the player
        /// </summary>
        /// <param name="other">Other.</param>
        public void OnTriggerStay(Collider other)
        {
            // we dont' do anything if we are not the local player.
            if (!photonView.IsMine)
            {
                return;
            }

            // We are only interested in Beamers
            // we should be using tags but for the sake of distribution, let's simply check by name.
            if (!other.name.Contains("Beam"))
            {
                return;
            }

            // we slowly affect health when beam is constantly hitting us, so player has to move to prevent death.
            CurrentHealth -= 0.1f * Time.deltaTime; ;
            //if (CurrentHealth < 0)
            //{
                KillId = other.gameObject.GetComponentInParent<PlayerManager>().Id;
            //}
            SetData(CurrentHealth.ToString());
        }


#if !UNITY_5_4_OR_NEWER
        /// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }
#endif


        /// <summary>
        /// MonoBehaviour method called after a new level of index 'level' was loaded.
        /// We recreate the Player UI because it was destroy when we switched level.
        /// Also reposition the player if outside the current arena.
        /// </summary>
        /// <param name="level">Level index loaded</param>
        void CalledOnLevelWasLoaded(int level)
        {
            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }

            GameObject _uiGo = Instantiate(this.playerUiPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }

        #endregion

        #region Private Methods


#if UNITY_5_4_OR_NEWER
        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }
#endif

        /// <summary>
        /// Processes the inputs. This MUST ONLY BE USED when the player has authority over this Networked GameObject (photonView.isMine == true)
        /// </summary>
        void ProcessInputs()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                // we don't want to fire when we interact with UI buttons for example. IsPointerOverGameObject really means IsPointerOver*UI*GameObject
                // notice we don't use on on GetbuttonUp() few lines down, because one can mouse down, move over a UI element and release, which would lead to not lower the isFiring Flag.
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    //	return;
                }

                if (!this.IsFiring)
                {
                    this.IsFiring = true;
                }
            }

            if (Input.GetButtonUp("Fire1"))
            {
                if (this.IsFiring)
                {
                    this.IsFiring = false;
                }
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                if (!this.IsHealing)
                {
                    this.IsHealing = true;
                }
            }
        }

        #endregion

        #region IPunObservable implementation

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // We own this player: send the others our data
                stream.SendNext(this.IsFiring);
                stream.SendNext(CurrentHealth);
                stream.SendNext(this.Id);
                //stream.SendNext(this.KillId);
            }
            else
            {
                // Network player, receive data
                this.IsFiring = (bool)stream.ReceiveNext();
                CurrentHealth = (float)stream.ReceiveNext();
                this.Id = (float)stream.ReceiveNext();
                //this.KillId = (float)stream.ReceiveNext();
            }
        }

        #endregion
    }
}