using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class SpawnModel : MonoBehaviour, IOnEventCallback
{
   [SerializeField] private List<Transform> _transforms = new List<Transform>();
    [SerializeField] private bool[] _isFreePoition = new bool[4];

    public List<Transform> Transforms => _transforms;
    public bool[] IsFreePoition => _isFreePoition;

    private void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public Transform GetSpawn()
    {
        for(int i = 0; i < _transforms.Count; i++)
        {
            if (_isFreePoition[i])
            {
                _isFreePoition[i] = false;
                return _transforms[i];
            }
        }

        return null;
    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case 2:
                _isFreePoition = (bool[])photonEvent.CustomData;
                break;
        }
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }


}
