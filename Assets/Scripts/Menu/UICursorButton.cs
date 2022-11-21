using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICursorButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        var cursorManager = GameObject.Find("Cursor").GetComponent<MouseCursorMan>();
        cursorManager.cursor = cursorManager.cursorHand;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var cursorManager = GameObject.Find("Cursor").GetComponent<MouseCursorMan>();
        cursorManager.cursor = cursorManager.cursorNormal;
    }

}
