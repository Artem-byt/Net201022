// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlayerAnimatorManager.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Networking Demos
// </copyright>
// <summary>
//  Used in PUN Basics Tutorial to deal with the networked player Animator Component controls.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;

namespace Photon.Pun.Demo.PunBasics
{
	public class PlayerAnimatorManager : MonoBehaviourPun, IOnEventCallback
    {
        #region Private Fields

        [SerializeField]
	    private float directionDampTime = 0.25f;
        Animator animator;
		private bool _isEndGame;

        public void OnEvent(EventData photonEvent)
        {
            switch (photonEvent.Code)
            {
                case 3:
                    _isEndGame = true;
                    animator.SetFloat("Speed", 0);
                    animator.SetFloat("Direction", 0, directionDampTime, Time.deltaTime);
                    break;
            }
        }

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehaviour method called on GameObject by Unity during initialization phase.
        /// </summary>
        void Start () 
	    {
            PhotonNetwork.AddCallbackTarget(this);
            animator = GetComponent<Animator>();
	    }
	        
		/// <summary>
		/// MonoBehaviour method called on GameObject by Unity on every frame.
		/// </summary>
	    void Update () 
	    {

			// Prevent control is connected to Photon and represent the localPlayer
	        if( photonView.IsMine == false && PhotonNetwork.IsConnected == true )
	        {
	            return;
	        }

			if (_isEndGame)
			{
                return;
			}

			// failSafe is missing Animator component on GameObject
	        if (!animator)
	        {
				return;
			}

			// deal with Jumping
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);			

			// only allow jumping if we are running.
   //         if (stateInfo.IsName("Base Layer.Run"))
   //         {
			//	// When using trigger parameter
   //             if (Input.GetButtonDown("Fire2")) animator.SetTrigger("Jump"); 
			//}
           
			// deal with movement
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            
			// prevent negative Speed.
            if( v < 0 )
            {
                v = -v;
            }

			// set the Animator Parameters
            animator.SetFloat( "Speed", h*h+v*v );
            //animator.SetFloat( "Direction", h, directionDampTime, Time.deltaTime );
	    }

        private void OnDestroy()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        #endregion

    }
}