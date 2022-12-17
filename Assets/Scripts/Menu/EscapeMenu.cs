using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EscapeMenu : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private RectTransform _panelWantExit;
    [SerializeField] private float _moveYEnd;
    [SerializeField] private float _moveYStart;

    [Header("Fade")]
    [Space(10)][SerializeField] Animator fadeAnimator;

    [Header("Audio")]
    [Space(10)][SerializeField] float defaultVolumeSound = 0.1f;
    [Space(10)][SerializeField] float defaultVolumeMusic = 0.8f;

    [SerializeField] AudioClip uiClick;
    [SerializeField] AudioClip uiHover;
    [SerializeField] AudioClip uiSpecial;

    [SerializeField] private AudioSource[] AudioSourceSound;
    [SerializeField] private AudioSource AudioSourceMusic;

    [SerializeField] private PlayerMovementAndLookNetwork player;

    [SerializeField] private Slider volumeSliderSound;
    [SerializeField] private Slider volumeSliderMusic;

    private void Start()
    {
        fadeAnimator.SetTrigger("FadeIn");
        SetStartVolumeSound();
        SetStartVolumeMusic();
    }

    #region Button

    public void Resume()
    {
        player.EscapeMenu(false, true);
    }

    public void AnimEnter(RectTransform panel)
    {
        panel.DOAnchorPosY(_moveYEnd, 2);
    }

    public void AnimExit(RectTransform panel)
    {
        panel.DOAnchorPosY(_moveYStart, 1);
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

    #region Sound
    public void SetVolumeSound(float _volume)
    {
        if (AudioSourceSound.Length <= 0)
            return;

        foreach(AudioSource audioSource in AudioSourceSound)
        {
            if(audioSource != null)
                audioSource.volume = _volume;
        }

        // Save volume
        PlayerPrefs.SetFloat("VolumeSound", _volume);
    }

    void SetStartVolumeSound()
    {
        if (!PlayerPrefs.HasKey("VolumeSound"))
        {
            PlayerPrefs.SetFloat("VolumeSound", defaultVolumeSound);
            LoadVolumeSound();
        }
        else
        {
            LoadVolumeSound();
        }
    }

    public void LoadVolumeSound()
    {
        volumeSliderSound.value = PlayerPrefs.GetFloat("VolumeSound");
    }
    #endregion

    #region Music
    public void SetVolumeMusic(float _volume)
    {
        // Adjust volume
        AudioSourceMusic.volume = _volume;

        // Save volume
        PlayerPrefs.SetFloat("VolumeMusic", _volume);
    }

    void SetStartVolumeMusic()
    {
        if (!PlayerPrefs.HasKey("VolumeMusic"))
        {
            PlayerPrefs.SetFloat("VolumeMusic", defaultVolumeMusic);
            LoadVolumeMusic();
        }
        else
        {
            LoadVolumeMusic();
        }
    }

    public void LoadVolumeMusic()
    {
        volumeSliderMusic.value = PlayerPrefs.GetFloat("VolumeMusic");
    }
    #endregion

    public void UIClick()
    {
        AudioSourceSound[0].PlayOneShot(uiClick);
    }

    public void UIHover()
    {
        AudioSourceSound[0].PlayOneShot(uiHover);
    }

    public void UISpecial()
    {
        AudioSourceSound[0].PlayOneShot(uiSpecial);
    }

    #endregion
}
