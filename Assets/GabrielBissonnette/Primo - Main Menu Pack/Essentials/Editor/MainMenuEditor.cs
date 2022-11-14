using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MainMenuManager))]
public class MainMenuEditor : Editor
{
    #region SerializedProperty
    private SerializedProperty showBackground;
    private SerializedProperty showSocial1;
    private SerializedProperty showSocial2;
    private SerializedProperty showSocial3;
    private SerializedProperty showVersion;
    private SerializedProperty showFade;
    private SerializedProperty social1Icon;
    private SerializedProperty social2Icon;
    private SerializedProperty social3Icon;
    private SerializedProperty version;
    private SerializedProperty play;
    private SerializedProperty settings;
    private SerializedProperty quit;
    private SerializedProperty social1Link;
    private SerializedProperty social2Link;
    private SerializedProperty social3Link;
    private SerializedProperty logo;
    private SerializedProperty background;
    private SerializedProperty buttons;
    private SerializedProperty mainColor;
    private SerializedProperty secondaryColor;
    private SerializedProperty sceneToLoad;
    private SerializedProperty defaultVolume;
    private SerializedProperty uiClick;
    private SerializedProperty uiHover;
    private SerializedProperty uiSpecial;

    private SerializedProperty homePanel;
    private SerializedProperty settingsPanel;
    private SerializedProperty bannerPanel;
    private SerializedProperty social1Image;
    private SerializedProperty social2Image;
    private SerializedProperty social3Image;
    private SerializedProperty playText;
    private SerializedProperty settingsText;
    private SerializedProperty quitText;
    private SerializedProperty versionText;
    private SerializedProperty logoImage;
    private SerializedProperty backgroundImage;
    private SerializedProperty mainColorImages;
    private SerializedProperty mainColorTexts;
    private SerializedProperty secondaryColorImages;
    private SerializedProperty secondaryColorTexts;
    private SerializedProperty buttonsElements;
    private SerializedProperty fadeAnimator;
    private SerializedProperty volumeSlider;
    private SerializedProperty resolutionDropdown;
    private SerializedProperty audioSource;
    #endregion

    #region Private
    private string[] m_Tabs = { "Values", "Components" };
    private int m_TabsSelected = 0;
    MainMenuManager mainMenuManager;
    SerializedObject soTarget;
    Texture2D texturePanel1;
    Texture2D texturePanel2;
    #endregion

    private void OnEnable()
    {
        mainMenuManager = (MainMenuManager)target;
        soTarget = new SerializedObject(target);

        texturePanel1 = Resources.Load<Texture2D>("InspectorBanner1");
        texturePanel2 = Resources.Load<Texture2D>("InspectorBanner2");

        #region FindProperty
        showBackground = soTarget.FindProperty("showBackground");
        showSocial1 = soTarget.FindProperty("showSocial1");
        showSocial2 = soTarget.FindProperty("showSocial2");
        showSocial3 = soTarget.FindProperty("showSocial3");
        version = soTarget.FindProperty("version");
        play = soTarget.FindProperty("play");
        settings = soTarget.FindProperty("settings");
        quit = soTarget.FindProperty("quit");
        social1Link = soTarget.FindProperty("social1Link");
        social2Link = soTarget.FindProperty("social2Link");
        social3Link = soTarget.FindProperty("social3Link");
        social1Icon = soTarget.FindProperty("social1Icon");
        social2Icon = soTarget.FindProperty("social2Icon");
        social3Icon = soTarget.FindProperty("social3Icon");
        logo = soTarget.FindProperty("logo");
        background = soTarget.FindProperty("background");
        buttons = soTarget.FindProperty("buttons");
        mainColor = soTarget.FindProperty("mainColor");
        secondaryColor = soTarget.FindProperty("secondaryColor");
        sceneToLoad = soTarget.FindProperty("sceneToLoad");
        defaultVolume = soTarget.FindProperty("defaultVolume");
        showVersion = soTarget.FindProperty("showVersion");
        showFade = soTarget.FindProperty("showFade");
        uiClick = soTarget.FindProperty("uiClick");
        uiHover = soTarget.FindProperty("uiHover");
        uiSpecial = soTarget.FindProperty("uiSpecial");

        homePanel = soTarget.FindProperty("homePanel");
        settingsPanel = soTarget.FindProperty("settingsPanel");
        bannerPanel = soTarget.FindProperty("bannerPanel");
        social1Image = soTarget.FindProperty("social1Image");
        social2Image = soTarget.FindProperty("social2Image");
        social3Image = soTarget.FindProperty("social3Image");
        playText = soTarget.FindProperty("playText");
        settingsText = soTarget.FindProperty("settingsText");
        quitText = soTarget.FindProperty("quitText");
        versionText = soTarget.FindProperty("versionText");
        logoImage = soTarget.FindProperty("logoImage");
        backgroundImage = soTarget.FindProperty("backgroundImage");
        mainColorImages = soTarget.FindProperty("mainColorImages");
        mainColorTexts = soTarget.FindProperty("mainColorTexts");
        secondaryColorImages = soTarget.FindProperty("secondaryColorImages");
        secondaryColorTexts = soTarget.FindProperty("secondaryColorTexts");
        buttonsElements = soTarget.FindProperty("buttonsElements");
        fadeAnimator = soTarget.FindProperty("fadeAnimator");
        volumeSlider = soTarget.FindProperty("volumeSlider");
        resolutionDropdown = soTarget.FindProperty("resolutionDropdown");
        audioSource = soTarget.FindProperty("audioSource");
        #endregion
    }

    public override void OnInspectorGUI()
    {
        soTarget.Update();
        EditorGUI.BeginChangeCheck();

        #region Tabs

        EditorGUILayout.BeginHorizontal();
        m_TabsSelected = GUILayout.Toolbar(m_TabsSelected, m_Tabs);
        EditorGUILayout.EndHorizontal();
        #endregion

        if (EditorGUI.EndChangeCheck())
        {
            soTarget.ApplyModifiedProperties();
        }

        mainMenuManager = (MainMenuManager)target;
        mainMenuManager.UIEditorUpdate();
    }
}
