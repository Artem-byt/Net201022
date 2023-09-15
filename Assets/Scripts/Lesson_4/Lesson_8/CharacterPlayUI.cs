using Photon.Pun.Demo.Cockpit.Forms;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterPlayUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _levelUI;
    [SerializeField] private TMP_Text _goldUI;
    [SerializeField] private TMP_Text _damageUI;
    [SerializeField] private TMP_Text _maxHPUI;
    [SerializeField] private TMP_Text _xpUI;

    public TMP_Text LevelUI => _levelUI;
    public TMP_Text GoldUI => _goldUI;
    public TMP_Text DamageUI => _damageUI;
    public TMP_Text MaxHPUI => _maxHPUI;
    public TMP_Text XpUI => _xpUI;
}
