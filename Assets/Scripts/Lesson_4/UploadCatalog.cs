using PlayFab;
using PlayFab.ClientModels;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public class UploadCatalog : MonoBehaviour
{
    private readonly Dictionary<string, CatalogItem> _catalog = new Dictionary<string, CatalogItem>();


    [SerializeField] private UICatalogItemHandler _catalogUIPreparer;


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
        }
       _catalogUIPreparer.HandleUI(catalog);
    }


}
