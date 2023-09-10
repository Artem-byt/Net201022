using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIHandlerBase<T> : MonoBehaviour
{
    [SerializeField] protected Transform _parentOfItemBlocks;
    [SerializeField] protected GameObject _itemBlockPrefab;
    [SerializeField] protected RectTransform _parentOfItemPanel;
    [SerializeField] protected VerticalLayoutGroup _verticalLayoutGroup;

    [Header("UI")]
    [SerializeField] protected float _MinHeightOfItemBlock = 40f;
    [SerializeField] protected float _HeightOfVisiblePartUI = 70f;

    protected float _spaceBetweenBlocks = 8f;
    protected RectTransform _rectOfItemPanel;
    protected List<Button> _listOfObjects;

    private void Start()
    {
        _listOfObjects = new List<Button>();
        _rectOfItemPanel = _parentOfItemBlocks.GetComponent<RectTransform>();
        _spaceBetweenBlocks = _verticalLayoutGroup.spacing;
        _HeightOfVisiblePartUI = _parentOfItemPanel.offsetMin.y - _parentOfItemPanel.offsetMax.y;
    }

    public virtual void HandleUI(List<T> catalog)
    {
        var countOfCatalogElements = catalog.Count;
        var pffsetMaxY = countOfCatalogElements * (_MinHeightOfItemBlock + _spaceBetweenBlocks) - _HeightOfVisiblePartUI;
        _rectOfItemPanel.offsetMax = new Vector2(_rectOfItemPanel.offsetMax.x, _rectOfItemPanel.offsetMax.y + pffsetMaxY);

    }

    public void ClearList()
    {
        for (int i = 0; i < _listOfObjects.Count; i++)
        {
            Destroy(_listOfObjects[i].gameObject);
        }
        _listOfObjects.Clear();
    }
}
