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
    //[SerializeField] private CharacterPlayFabCall _characterPlayFabCall;

    private List<CharacterResult> _characters = new List<CharacterResult>();
    private List<Button> _newCharacterPanelButtons= new List<Button>();
    private TMP_InputField _nameNewCharacter;

    private void Start()
    {
        GetCharacters();
        _newCharacterPanelButtons = _panelNewCharacterCreator.GetComponentsInChildren<Button>().ToList();
        _nameNewCharacter = _panelNewCharacterCreator.GetComponentInChildren<TMP_InputField>();

        _newCharacterPanelButtons[0].onClick.AddListener(CloseCreateNewCharacterPrompt);
        _newCharacterPanelButtons[1].onClick.AddListener(OnCreatedNewCharacterAccept);
    }

    private void GetCharacters()
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

        for (int i = 0; i < result.Characters.Count; i++)
        {
            _buttonSlots[i].onClick.RemoveAllListeners();
            _characters.Add(result.Characters[i]);
            ChangeNameOfButton(_buttonSlots[i], result.Characters[i]);

            var characterResult = result.Characters[i];
            _buttonSlots[i].onClick.AddListener(() => ChooseCreatedCharacter(characterResult));

        }
        if (result.Characters.Count < 2)
        {
            CharacterPlayFabCall.CompletePurchaseForCharacterSlots();
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

    private void ChooseCreatedCharacter(CharacterResult characterResult)
    {
        SwitchStateUICharacters(false, false);
        OnStartGame?.Invoke(characterResult);
    }

    private void OnCreatedNewCharacterAccept()
    {
        if(_nameNewCharacter.text == string.Empty)
        {
            Debug.Log("Поле не может быть пустым");
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
