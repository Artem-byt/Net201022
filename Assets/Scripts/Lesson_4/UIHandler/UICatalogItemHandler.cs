using Photon.Realtime;
using PlayFab.ClientModels;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICatalogItemHandler : UIHandlerBase<CatalogItem>
{

    public override void HandleUI(List<CatalogItem> catalog)
    {
        base.HandleUI(catalog);

        foreach(var item in catalog)
        {
            var obj = Instantiate(_itemBlockPrefab, _parentOfItemBlocks).GetComponentInChildren<TMP_Text>();
            _listOfObjects.Add(obj.gameObject.GetComponentInChildren<Button>());
            obj.text = item.DisplayName;
        }

    }

}
