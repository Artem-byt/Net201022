using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharachterCallUI : MonoBehaviour
{
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
            _characters.Add(result.Characters[i]);
            ChangeNameOfButton(_buttonSlots[i], result.Characters[i]);
            _buttonSlots[i].onClick.AddListener(ChooseCreatedCharacter);
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

    }

    private void ChooseCreatedCharacter()
    {

    }

    private void OnCreatedNewCharacterAccept()
    {
        if(_nameNewCharacter.text == string.Empty)
        {
            Debug.Log("���� �� ����� ���� ������");
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
