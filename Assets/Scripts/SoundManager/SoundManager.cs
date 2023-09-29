using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource _soundSource;
    [SerializeField] private AudioClip _soundBeforeGame;
    [SerializeField] private AudioClip _soundInGame;

    private string _currentScene;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        _currentScene = SceneManager.GetActiveScene().name;
        _soundSource.clip = _soundBeforeGame;
        _soundSource.volume = 0.01f;
        _soundSource.loop = true;
        _soundSource.Play();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "PunBasics-Room for 1")
        {
            _currentScene = SceneManager.GetActiveScene().name;
            _soundSource.clip = _soundInGame;
            _soundSource.volume = 0.01f;
            _soundSource.loop = true;
            _soundSource.Play();
        }
        else if (_currentScene == "PunBasics-Room for 1")
        {
            _currentScene = SceneManager.GetActiveScene().name;
            _soundSource.clip = _soundBeforeGame;
            _soundSource.volume = 0.1f;
            _soundSource.loop = true;
            _soundSource.Play();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
