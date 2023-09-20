using UnityEngine;
using UnityEngine.UI;

public class PremitiveAnimation : MonoBehaviour
{

    [SerializeField] private Image _loading;
    [SerializeField] private float _axis = 5;

    private Image _loadingImage;
    private bool _isRotating;

    private void Update()
    {
        if (!_isRotating)
        {
            return;
        }

        _loadingImage.rectTransform.Rotate(Vector3.forward, -_axis);
    }

    public void SetRotating()
    { 
        _isRotating = !_isRotating;
        _loading.gameObject.SetActive(_isRotating);
    }
}
