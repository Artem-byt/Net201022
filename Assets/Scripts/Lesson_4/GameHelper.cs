using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHelper : MonoBehaviour
{
    [SerializeField] private GameObject _playerHelper;
    [SerializeField] private float _deltaTimeForActiveUI;

    private bool _isHelp = false;
    private void Start()
    {
        _playerHelper.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !_isHelp)
        {
            _isHelp = true;
            StartCoroutine(ActivateUIForGameplayHelp());
        }
    }

    private IEnumerator ActivateUIForGameplayHelp()
    {
        _playerHelper.gameObject.SetActive(true);
        yield return new WaitForSeconds(_deltaTimeForActiveUI);
        _isHelp = false;
        _playerHelper.gameObject.SetActive(false);
    }
}
