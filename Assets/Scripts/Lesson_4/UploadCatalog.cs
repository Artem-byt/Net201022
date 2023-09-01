using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UploadCatalog : MonoBehaviour
{
    private readonly Dictionary<string, CatalogItem> _catalog = new Dictionary<string, CatalogItem>();

    [SerializeField] private List<TMP_Text> _itemsShow = new List<TMP_Text>();
    [SerializeField] private Transform _parentOfItemBlocks;
    [SerializeField] private GameObject _itemBlockPrefab;

    private void Start() 
    { 
        PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), OnGetCatalogSuccess, OnFailure);
    }

    private void OnFailure(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError($"Something went wrong:{errorMessage}");
    }

    private void OnGetCatalogSuccess(GetCatalogItemsResult result) 
    { 
        HandleCatalog(result.Catalog); 
        Debug.Log($"Catalog was loaded successfully!");
    }

    private void HandleCatalog(List<CatalogItem> catalog) 
    { 
        foreach (var item in catalog) 
        { 
            _catalog.Add(item.ItemId, item); 
            Debug.Log($"Catalog item {item.ItemId} was added successfully!");
            HandleUI(item);
        }
    }

    private void HandleUI(CatalogItem item)
    {
        var obj = Instantiate(_itemBlockPrefab, _parentOfItemBlocks).GetComponentInChildren<TMP_Text>();
        obj.text = item.DisplayName;
    }
}
