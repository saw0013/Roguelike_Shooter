using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIWindowHandle : MonoBehaviour,  IBeginDragHandler, IEndDragHandler
{
    [SerializeField] PlayerData playerData;
    public void OnBeginDrag(PointerEventData eventData)
    {
        playerData.InputIsActive(false);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        playerData.InputIsActive(true);
    }

}
