using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EscapeMenu : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private RectTransform _panelWantExit;
    [SerializeField] private float _moveYEnd;
    [SerializeField] private float _moveYStart;

    private MainMenuManager mainMenuManager;

    [Header("Fade")]
    [Space(10)][SerializeField] Animator fadeAnimator;

    [Header("Audio")]
    [Space(10)][SerializeField] float defaultVolumeSound = 0.1f;
    [Space(10)][SerializeField] float defaultVolumeMusic = 0.8f;

    private VCA vcaSound;
    private VCA vcaMusic;

    private MainMenuManager menuManager;

    [SerializeField] private PlayerMovementAndLookNetwork player;

    [SerializeField] private Slider volumeSliderSound;
    [SerializeField] private Slider volumeSliderMusic;

    private void Start()
    {
        mainMenuManager = FindObjectOfType<MainMenuManager>();
        mainMenuManager.RegisterEscapeMenu(this);

        vcaSound = RuntimeManager.GetVCA("vca:/Sound");

        vcaMusic = RuntimeManager.GetVCA("vca:/Music");

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
        vcaSound.setVolume(_volume);

        // Save volume
        PlayerPrefs.SetFloat("VolumeSound", _volume);

        mainMenuManager.LoadVolumeSound();
    }

    void SetStartVolumeSound() => LoadVolumeSound();

    public void LoadVolumeSound() => volumeSliderSound.value = mainMenuManager.GetSoundVolume();
    #endregion

    #region Music
    public void SetVolumeMusic(float _volume)
    {
        // Adjust volume
        vcaMusic.setVolume(_volume);

        // Save volume
        PlayerPrefs.SetFloat("VolumeMusic", _volume);

        mainMenuManager.LoadVolumeMusic();
    }

    void SetStartVolumeMusic() => LoadVolumeMusic();

    public void LoadVolumeMusic() => volumeSliderMusic.value = mainMenuManager.GetMusicVolume();

    #endregion

    public void RegisterMainMenuManager(MainMenuManager mainMenu) => menuManager = mainMenu;

    #endregion
}
