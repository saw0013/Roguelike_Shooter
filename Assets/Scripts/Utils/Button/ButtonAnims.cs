using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using FMODUnity;
using FMOD.Studio;
using UnityEngine;

public class ButtonAnims : MonoBehaviour
{
    [SerializeField, EventRef] private string _uiClick;
    [SerializeField, EventRef] private string _uiHover;
    [SerializeField, EventRef] private string _uiSpecial;

    private RectTransform button;

    private void Start() => button = GetComponent<RectTransform>();

    public void UIClick()
    {
        RuntimeManager.PlayOneShot(_uiClick);
        button.DOScale(new Vector3(0.5f, 0.5f, 0.5f), Time.deltaTime * 10);
    }

    public void UIHover()
    {
        RuntimeManager.PlayOneShot(_uiHover);
        button.DOScale(new Vector3(0.6f, 0.6f, 0.6f), Time.deltaTime * 10);
    }

    public void UIUnHover()
    {
        button.DOScale(new Vector3(0.5f, 0.5f, 0.5f), Time.deltaTime * 10);
    }

    public void UISpecial()
    {
        RuntimeManager.PlayOneShot(_uiSpecial);
    }
}
