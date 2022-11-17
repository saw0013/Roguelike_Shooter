using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class MainMenuManager : MonoBehaviour
{
    #region Variables

    [Header("On/Off")]
    [Space(5)] [SerializeField] bool showBackground;
    [SerializeField] bool showSocial1;
    [SerializeField] bool showSocial2;
    [SerializeField] bool showSocial3;
    [SerializeField] bool showVersion;
    [SerializeField] bool showFade;

    [Header("Sprites")]
    [SerializeField] Sprite buttons;

    [Header("Version")]
    [Space(10)] [SerializeField] string version = "v.0105";

    [Header("Texts")]
    [Space(10)] [SerializeField] string play = "Play";
    [SerializeField] string settings = "Settings";
    [SerializeField] string quit = "Quit";

    [Header("Audio")]
    [Space(10)][SerializeField] float defaultVolumeSound = 0.1f;
    [Space(10)][SerializeField] float defaultVolumeMusic = 0.8f;

    [SerializeField] AudioClip uiClick;
    [SerializeField] AudioClip uiHover;
    [SerializeField] AudioClip uiSpecial;

    // Components
    [Header("Components")]
    [SerializeField] RectTransform welcomePanel;
    [SerializeField] GameObject homePanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject connectionPanel;

    [SerializeField] private TMP_InputField _nameField;

    [Header("Buttons")]
    [SerializeField] private Button _buttonDone;

    [Header("Fade")]
    [Space(10)] [SerializeField] Animator fadeAnimator;

    [Header("Texts")]
    [Space(10)] [SerializeField] TextMeshProUGUI playText;
    [SerializeField] TextMeshProUGUI settingsText;
    [SerializeField] TextMeshProUGUI quitText;
    [SerializeField] TextMeshProUGUI versionText;   

    [Header("Settings")]
    [SerializeField] private Slider volumeSliderSound;
    [SerializeField] private Slider volumeSliderMusic;

    [SerializeField] TMP_Dropdown resolutionDropdown;

    [Header("Audio")]
    [SerializeField] private AudioSource _audioSourceSound;

    [SerializeField] private AudioSource[] _audioSourceMusic;

    Resolution[] resolutions;

    #endregion

    void Start()
    {
        if (!PlayerPrefs.HasKey("PlayerName")) WelcomePlayer(); 


        SetStartUI();
        SetStartVolumeSound();
        SetStartVolumeMusic();
        PrepareResolutions();
    }

    private void Update()
    {
        if (!PlayerPrefs.HasKey("PlayerName"))
        {
            if(!string.IsNullOrWhiteSpace(_nameField.text)) _buttonDone.interactable = true;
            else _buttonDone.interactable = false;
        }
    }

    private void WelcomePlayer()
    {
        welcomePanel.gameObject.SetActive(true);
        welcomePanel.DOScale(new Vector3(0.7791322f, 0.7791322f, 0.7791322f), 2);
    }

    private void SetStartUI()
    {
        fadeAnimator.SetTrigger("FadeIn");
        homePanel.SetActive(true);
        settingsPanel.SetActive(false);
        connectionPanel.SetActive(false);
    }

    public void UIEditorUpdate()
    {
        #region Sprites
        // Fade
        fadeAnimator.gameObject.SetActive(showFade);

        #endregion

        #region Texts

        if (playText != null)
            playText.text = play;

        if (settingsText != null)
            settingsText.text = settings;

        if (quitText != null)
            quitText.text = quit;

        // Version number
        versionText.gameObject.SetActive(showVersion);
        if (versionText != null)
            versionText.text = version;

        #endregion
    }

    #region Levels
    public void Quit() => Application.Quit();

    public void ButtonDone()
    {
        PlayerPrefs.SetString("PlayerName", _nameField.text);
        var _tweenOn = welcomePanel.DOScale(new Vector3(0.001039826f, 0.001039826f, 0.001039826f), 2);
        _tweenOn.onComplete = () => welcomePanel.gameObject.SetActive(false);   
        Debug.Log(_nameField.text);
    }

    #endregion

    #region Audio

    #region Sound
    public void SetVolumeSound(float _volume)
    {
        _audioSourceSound.volume = _volume;

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
        foreach(AudioSource audioSource in _audioSourceMusic)
        {
            audioSource.volume = _volume;
        }

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
        _audioSourceSound.PlayOneShot(uiClick);
    }

    public void UIHover()
    {
        _audioSourceSound.PlayOneShot(uiHover);
    }

    public void UISpecial()
    {
        _audioSourceSound.PlayOneShot(uiSpecial);
    }

    #endregion

    #region Graphics & Resolution Settings

    public void SetQuality(int _qualityIndex)
    {
        QualitySettings.SetQualityLevel(_qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void PrepareResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;

            if(!options.Contains(option))
                options.Add(option);

            if(i == resolutions.Length - 1)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int _resolutionIndex)
    {
        Resolution resolution = resolutions[_resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    #endregion

    #region UNITY_EDITOR

    [ContextMenu("ClearPrefs")]
    private void ClearPrefs()
    {
        PlayerPrefs.DeleteKey("PlayerName");
    }
    #endregion
}
