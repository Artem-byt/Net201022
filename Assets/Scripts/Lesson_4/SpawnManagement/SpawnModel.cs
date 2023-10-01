using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class SpawnModel : MonoBehaviour, IOnEventCallback
{
   [SerializeField] private List<Transform> _transforms = new List<Transform>();
    [SerializeField] private List<Transform> _transformsAI = new List<Transform>();
    [SerializeField] private bool[] _isFreePoition = new bool[4];

    public List<Transform> Transforms => _transforms;
    public List<Transform> TransformsAI => _transformsAI;
    public bool[] IsFreePoition => _isFreePoition;
    public bool isNewPlayer;

    public Transform FreePosition;

    private void Awake()
    {
        PhotonNetwork.AddCallbackTarget(this);
        FreePosition = GetSpawn();
    }

    private Transform GetSpawn()
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

    public Transform GetAISpawn()
    {
        return _transformsAI[0];
    }

    public void OnEvent(EventData photonEvent)
    {
        switch (photonEvent.Code)
        {
            case 2:
                _isFreePoition = (bool[])photonEvent.CustomData;
                isNewPlayer = true;
                FreePosition = GetSpawn();
                break;
        }
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }


}
