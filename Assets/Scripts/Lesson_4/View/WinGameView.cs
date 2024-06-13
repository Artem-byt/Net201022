using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinGameView : MonoBehaviour, IOnEventCallback
{
    [SerializeField] private Collider _trigger;

    private void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerManager>(out var playerManager))
        {
            _trigger.enabled = false;
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
            SendOptions sendOptions = new SendOptions { Reliability = true };
            PhotonNetwork.RaiseEvent(3, playerManager.Id, raiseEventOptions, sendOptions);
        }
    }
    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case 3:
                _trigger.enabled = false;
                break;
        }
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }
}
