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
    [SerializeField] private TMP_Text _goldUI;
    [SerializeField] private TMP_Text _damageUI;
    [SerializeField] private TMP_Text _maxHPUI;
    [SerializeField] private TMP_Text _xpUI;

    private PlayerManager _target;

    public TMP_Text LevelUI => _levelUI;
    public TMP_Text GoldUI => _goldUI;
    public TMP_Text DamageUI => _damageUI;
    public TMP_Text MaxHPUI => _maxHPUI;
    public TMP_Text XpUI => _xpUI;

    void Awake()
    {
        transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
    }

    private void Start()
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
        GoldUI.text = "GOLD " + statistics[CharacterPlayFabCall.GOLD].ToString();
        LevelUI.text = "Level " + statistics[CharacterPlayFabCall.LEVEL].ToString();
        DamageUI.text = "Damage " + statistics[CharacterPlayFabCall.DAMAGE].ToString();
        MaxHPUI.text = "Max HP " + statistics[CharacterPlayFabCall.HP].ToString();
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
