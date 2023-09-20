using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrepareCharacterUI : MonoBehaviour
{
    public event Action<CharacterResult> OnStartGame;

    [SerializeField] private List<Button> _buttonSlots;
    [SerializeField] private GameObject _panelNewCharacterCreator;
    [SerializeField] private GameObject _chooseCharacterPrefab;
    [SerializeField] private TMP_Text _goldCurrency;

    private List<CharacterResult> _characters = new List<CharacterResult>();
    private List<Button> _newCharacterPanelButtons= new List<Button>();
    private TMP_InputField _nameNewCharacter;
    private int slotsCount = 0;

    private void Start()
    {
        UpdateAccountStatsInfo();
        GetCharacters();
        _newCharacterPanelButtons = _panelNewCharacterCreator.GetComponentsInChildren<Button>().ToList();
        _nameNewCharacter = _panelNewCharacterCreator.GetComponentInChildren<TMP_InputField>();

        _newCharacterPanelButtons[0].onClick.AddListener(CloseCreateNewCharacterPrompt);
        _newCharacterPanelButtons[1].onClick.AddListener(OnCreatedNewCharacterAccept);
    }

    private void UpdateAccountStatsInfo()
    {
        PLayFabAccountInfoCalls.GetUserInventoryInfo(new GetUserInventoryRequest(), UpdateCurrencyInfoUI);
    }

    private void UpdateCurrencyInfoUI(GetUserInventoryResult getUserInventoryResult)
    {
        _goldCurrency.text = getUserInventoryResult.VirtualCurrency[ConstantStrings.GO_VIRTUAL_CURRENCY].ToString();
    }

    private void GetCharacters()
    {
        PLayFabAccountInfoCalls.GetAllUserCharacters(new ListUsersCharactersRequest(), UpdateCharactersList);
    }

    private void UpdateCharactersList(ListUsersCharactersResult result)
    {
        if (_characters.Count > 0)
        {
            _characters.Clear();
        }

        for (int i = 0; i < result.Characters.Count; i++)
        {
            _buttonSlots[i].onClick.RemoveAllListeners();
            _characters.Add(result.Characters[i]);
            ChangeNameOfButton(_buttonSlots[i], result.Characters[i].CharacterName);

            var characterResult = result.Characters[i];
            _buttonSlots[i].onClick.AddListener(() => ChooseCreatedCharacter(characterResult));

        }

        CheckSlotsForPurchase();
        for (int i = _characters.Count; i < _buttonSlots.Count; i++)
        {
            //продумать счет слотов
            if (slotsCount > 0)
            {
                OpenCreateNewCharacterPrompt();
            }
            else
            {
                CharacterPlayFabCall.CompletePurchaseForCharacterSlots(() => { OpenCreateNewCharacterPrompt(); UpdateAccountStatsInfo(); });
            }
            //_buttonSlots[i]
        }
    }

    private void CheckSlotsForPurchase()
    {
        PLayFabAccountInfoCalls.GetUserInventoryInfo(new GetUserInventoryRequest(),  CheckSlotsAccount);
    }

    private void CheckSlotsAccount(GetUserInventoryResult getUserInventoryResult)
    {
        foreach(var itemInstance in getUserInventoryResult.Inventory)
        {
            if  (itemInstance.ItemInstanceId == ConstantStrings.CHARACTER_TOKEN)
            {
                slotsCount++;
            }
        }
    }

    public void SwitchStateUICharacters(bool characterPrefab, bool CharacterCreatePrefab)
    {
        _chooseCharacterPrefab.SetActive(characterPrefab);
        _panelNewCharacterCreator.SetActive(CharacterCreatePrefab);
    }

    private void ChangeNameOfButton(Button button, string characterName)
    {
        if (button != null)
        {
            button.GetComponentInChildren<TMP_Text>().text = characterName;
        }
    }

    private void ChooseCreatedCharacter(CharacterResult characterResult)
    {
        SwitchStateUICharacters(false, false);
        OnStartGame?.Invoke(characterResult);
    }

    private void OnCreatedNewCharacterAccept()
    {
        if(_nameNewCharacter.text == string.Empty)
        {
            Debug.Log("ѕоле не может быть пустым");
            return;
        }

        CharacterPlayFabCall.CreateCharacterWithItemId("character_token", GetCharacters, _nameNewCharacter.text);
        SwitchStateUICharacters(true, false);
    }

    private void OpenCreateNewCharacterPrompt() 
    {
        SwitchStateUICharacters(false, true);
    }
    private void CloseCreateNewCharacterPrompt() 
    {
        SwitchStateUICharacters(false, false);
    }

    private void OnDestroy()
    {
        _characters.Clear();

        for (int i = 0; i < _buttonSlots.Count; i++)
        {
            _buttonSlots[i].onClick.RemoveAllListeners();
        }
    }
}
