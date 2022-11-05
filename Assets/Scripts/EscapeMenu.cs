using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine;

public class EscapeMenu : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private RectTransform _panelWantExit;
    [SerializeField] private float _moveYEnd;
    [SerializeField] private float _moveYStart;

    [Header("Fade")]
    [Space(10)][SerializeField] Animator fadeAnimator;

    [Header("Audio")]
    [Space(10)][SerializeField] float defaultVolume = 0.8f;
    [SerializeField] AudioClip uiClick;
    [SerializeField] AudioClip uiHover;
    [SerializeField] AudioClip uiSpecial;

    [SerializeField] private AudioSource AudioSource;

    #region Button

    public void YouWantExit()
    {
        _panelWantExit.DOAnchorPosY(_moveYEnd, 2);
    }

    public void Stay()
    {
        _panelWantExit.DOAnchorPosY(_moveYStart, 1);
    }

    public void ExitToMianMenu()
    {
        fadeAnimator.SetTrigger("FadeOut");

        StartCoroutine(WaitToLoadLevel(0));
    }

    IEnumerator WaitToLoadLevel(int sceneToLoad)
    {
        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(sceneToLoad);
    }

    #endregion

    #region Audio
    public void UIClick()
    {
        AudioSource.PlayOneShot(uiClick);
    }

    public void UIHover()
    {
        AudioSource.PlayOneShot(uiHover);
    }

    public void UISpecial()
    {
        AudioSource.PlayOneShot(uiSpecial);
    }

    #endregion
}
