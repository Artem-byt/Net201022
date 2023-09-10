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
            var obj = Instantiate(_itemBlockPrefab, _parentOfItemBlocks).GetComponentInChildren<TMP_Text>();
            _listOfObjects.Add(obj.gameObject.GetComponentInChildren<Button>());
            obj.text = item.Name;
        }

    }
}
