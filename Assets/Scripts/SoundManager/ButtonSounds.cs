using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSounds : MonoBehaviour
{
    [SerializeField] private List<Button> _buttons = new List<Button>();
    [SerializeField] private AudioSource _audioSource;

    private void Awake()
    {
        foreach (var btn in _buttons)
        {
            btn.onClick.AddListener(_audioSource.Play);
        }
    }


    private void OnDestroy()
    {
        foreach (var btn in _buttons)
        {
            btn.onClick.RemoveListener(_audioSource.Play);
        }
    }


}
