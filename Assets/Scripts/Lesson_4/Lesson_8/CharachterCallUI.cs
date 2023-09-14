using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharachterCallUI : MonoBehaviour
{
    public event Action OnStartGame;

    [SerializeField] private List<Button> _buttonSlots;
    [SerializeField] private GameObject _panelNewCharacterCreator;
    [SerializeField] private GameObject _chooseCharacterPrefab;

    private List<CharacterResult> _characters = new();
    private List<Button> _newCharacterPanelButtons= new();
    private TMP_InputField _nameNewCharacter;
    private CharacterPlayFabCall _characterPlayFabCall;


    private void Start()
    {
        GetCharacters();
        _characterPlayFabCall = new CharacterPlayFabCall();
        _characterPlayFabCall.OnUpdateCharacterStaristics += CloseCreateNewCharacterPrompt;
        _newCharacterPanelButtons = _panelNewCharacterCreator.GetComponentsInChildren<Button>().ToList();
        _nameNewCharacter = _panelNewCharacterCreator.GetComponentInChildren<TMP_InputField>();

        _newCharacterPanelButtons[0].onClick.AddListener(CloseCreateNewCharacterPrompt); 
        _newCharacterPanelButtons[1].onClick.AddListener(OnCreatedNewCharacterAccept);
    }

    public void GetCharacters() 
    { 
        PlayFabClientAPI.GetAllUsersCharacters(new ListUsersCharactersRequest(),
            res => 
            { 
                Debug.Log($"Characters owned: +{res.Characters.Count}");
                UpdateCharactersList(res);
            }, 
            Debug.LogError); 
    }

    private void UpdateCharactersList(ListUsersCharactersResult result)
    {
        if (_characters.Count > 0)
        {
            _characters.Clear();
        }

        for (int i=0; i< result.Characters.Count; i++)
        {
            _characters.Add(result.Characters[i]); ;
            Debug.Log(i + " Trying search Error: " + _buttonSlots[i]);
            ChangeNameOfButton(_buttonSlots[i], result.Characters[i]);
            _buttonSlots[i].onClick.AddListener(ChooseCreatedCharacter);

        }
        if (result.Characters.Count < 2) 
        {
            _characterPlayFabCall.CompletePurchaseForCharacterSlots();
        }

        for (int i = _characters.Count; i < _buttonSlots.Count; i++)
        {
            _buttonSlots[i].onClick.AddListener(OpenCreateNewCharacterPrompt);
        }
    }

    public void SwitchStateUICharacters(bool characterPrefab, bool CharacterCreatePrefab)
    {
        _chooseCharacterPrefab.SetActive(characterPrefab);
        _panelNewCharacterCreator.SetActive(CharacterCreatePrefab);
    }

    private void ChangeNameOfButton(Button button, CharacterResult characterResult)
    {
        if (button != null)
        {
            button.GetComponentInChildren<TMP_Text>().text = characterResult.CharacterName;
        }
    }

    private void ChooseCreatedCharacter()
    {
        SwitchStateUICharacters(false, false);
        OnStartGame?.Invoke();
    }

    private void OnCreatedNewCharacterAccept()
    {
        if(_nameNewCharacter.text == string.Empty)
        {
            Debug.Log("Поле не может быть пустым");
            return;
        }
        _characterPlayFabCall.OnNameChanged(_nameNewCharacter.text);
        _characterPlayFabCall.CreateCharacterWithItemId("character_token");
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
