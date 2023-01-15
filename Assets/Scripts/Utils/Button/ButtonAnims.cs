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

    private Vector3 startScale;

    private void Start()
    {
        button = GetComponent<RectTransform>();
        startScale = transform.localScale;
    }
    public void UIClick()
    {
        RuntimeManager.PlayOneShot(_uiClick);
        button.DOScale(new Vector3(startScale.x, startScale.y, startScale.z), Time.deltaTime * 10);
    }

    public void UIHover()
    {
        RuntimeManager.PlayOneShot(_uiHover);
        button.DOScale(new Vector3(startScale.x + 0.1f, startScale.y + 0.1f, startScale.z + 0.1f), Time.deltaTime * 10);
    }

    public void UIUnHover()
    {
        button.DOScale(new Vector3(startScale.x, startScale.y, startScale.z), Time.deltaTime * 10);
    }

    public void UISpecial()
    {
        RuntimeManager.PlayOneShot(_uiSpecial);
    }
}
