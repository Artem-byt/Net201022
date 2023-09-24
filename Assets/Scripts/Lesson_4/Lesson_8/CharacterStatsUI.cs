using Photon.Pun.Demo.Cockpit.Forms;
using Photon.Pun.Demo.PunBasics;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterStatsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _levelUI;
    [SerializeField] private TMP_Text _xpUI;

    private PlayerManager _target;

    public TMP_Text LevelUI => _levelUI;

    public TMP_Text XpUI => _xpUI;

    void Awake()
    {
        transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }

    private void Start()
    {
        UpdateClientStatistics();
    }

    public void UpdateClientStatistics()
    {
        CharacterPlayFabCall.GetCHaracterStatistics(UpdateUIStatistics, _target.CharacterResult.CharacterId);
    }

    void Update()
    {
        // Destroy itself if the target is null, It's a fail safe when Photon is destroying Instances of a Player over the network
        if (_target == null)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void UpdateUIStatistics(Dictionary<string, int> statistics)
    {
        XpUI.text = "XP " + statistics[CharacterPlayFabCall.XP].ToString();
        LevelUI.text = "Level " + statistics[CharacterPlayFabCall.LEVEL].ToString();
    }

    public void SetTarget(PlayerManager target)
    {

        if (target == null)
        {
            Debug.LogError("<Color=Red><b>Missing</b></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
            return;
        }

        // Cache references for efficiency because we are going to reuse them.
        _target = target;

    }
}
