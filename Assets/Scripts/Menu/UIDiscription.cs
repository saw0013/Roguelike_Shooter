using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDiscription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TMP_Text text;
    [SerializeField] private string TextHover, TextExit;

    private void Start()
    {
        GetComponent<Image>().CrossFadeColor(new Color(0, 0, 0), .1f, false, false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.text = TextHover.Replace("\\r\\n", Environment.NewLine);
        GetComponent<Image>().CrossFadeColor(new Color(0.7f, 0.7f, 0.7f), .5f, false, false);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.text = TextExit;
        //GetComponent<Image>().CrossFadeColor(new Color(0, 0f, 0f, 1f), 1, false, true);
        GetComponent<Image>().CrossFadeColor(new Color(0,0,0), .5f, false, false);
    }
}

