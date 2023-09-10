using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIRoomsHandler : UIHandlerBase<RoomInfo>
{

    public override void HandleUI(List<RoomInfo> roomList)
    {
        base.HandleUI(roomList);
        ClearList();

        foreach (var item in roomList)
        {
            var obj = Instantiate(_itemBlockPrefab, _parentOfItemBlocks);
            var text = obj.GetComponentInChildren<TMP_Text>();
            var btn = obj.GetComponent<Button>();
            btn.interactable = true;
            btn.onClick.AddListener(() => { OnClickButtonUI.Invoke(btn, item); });
            _listOfObjects.Add(btn);
            text.text = item.Name;
        }

    }
}
