using Photon.Pun.Demo.PunBasics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerTryAttempts : MonoBehaviour
{
    [SerializeField] private int _attempts = 3;

    [SerializeField] private PlayerManager _playerManager;

    public Button RestartButton;
    public TMP_Text Text;
    public void Initialize()
    {
        RestartButton.onClick.AddListener(TryAttempt);
    }

    private void TryAttempt()
    {
        _attempts--;
        RestartButton.gameObject.SetActive(false);
        Text.text = string.Empty;
        _playerManager.SpawnPlayer();

    }

    public void OnTriedAttempt()
    {
        if (_attempts > 0)
        {
            RestartButton.gameObject.SetActive(true);
            Text.text = $"Вы погибли, оставшихся попыток: {_attempts}";
        }
        Text.text = "У вас не осталось попыток";

    }


    private void OnDestroy()
    {
        if(RestartButton!=null)
        {
            RestartButton.onClick.RemoveAllListeners();
        }
      
    }

}
