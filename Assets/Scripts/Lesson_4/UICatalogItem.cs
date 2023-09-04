using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICatalogItem : MonoBehaviour
{
    [SerializeField] private Transform _parentOfItemBlocks;
    [SerializeField] private GameObject _itemBlockPrefab;
    [SerializeField] private RectTransform _parentOfItemPanel;

    [Header("UI")]
    [SerializeField] private float _MinHeightOfItemBlock = 40f;
    [SerializeField] private float _HeightOfVisiblePartUI = 70f;

    private float _spaceBetweenBlocks = 8f;
    private RectTransform _rectOfItemPanel;

    private void Awake()
    {
        _rectOfItemPanel = _parentOfItemBlocks.GetComponent<RectTransform>();
        _spaceBetweenBlocks = GetComponentInChildren<VerticalLayoutGroup>().spacing;
        _HeightOfVisiblePartUI = _parentOfItemPanel.offsetMin.y - _parentOfItemPanel.offsetMax.y;
    }

    public void HandleUI(List<CatalogItem> catalog)
    {
        var countOfCatalogElements = catalog.Count;
        _rectOfItemPanel.offsetMin = new Vector2(_rectOfItemPanel.offsetMin.x, _rectOfItemPanel.offsetMin.y - 80f);

        var pffsetMaxY = countOfCatalogElements * (_MinHeightOfItemBlock + _spaceBetweenBlocks) - _HeightOfVisiblePartUI;
        _rectOfItemPanel.offsetMax = new Vector2(_rectOfItemPanel.offsetMax.x, _rectOfItemPanel.offsetMax.y + pffsetMaxY);

        foreach(var item in catalog)
        {
            var obj = Instantiate(_itemBlockPrefab, _parentOfItemBlocks).GetComponentInChildren<TMP_Text>();
            obj.text = item.DisplayName;
        }

    }
}
