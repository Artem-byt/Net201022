using Photon.Realtime;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(LoadBalancerReference), menuName = "LoadBalancerReference/" +
nameof(LoadBalancerReference), order = 0)]
public class LoadBalancerReference : ScriptableObject
{
    public LoadBalancingClient LoadBalancingClient { get; set; }
}
