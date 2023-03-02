using System.Collections;
using System.Collections.Generic;
using Cosmoground;
using GetLanguage;
using Mirror;
using MirrorBasics;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginUser : MonoBehaviour
{
    [SerializeField] private TMP_Text ErrorText;
    [SerializeField] private TMP_Text LanguagePC;
    [SerializeField] private Button ButtonLogin;
    [SerializeField] TMP_InputField loginInput, passwordInput;
    private string valueLang;
    private string loginUrl = "https://blueboxproduction.ru/ucp/net/launcher_login.php";
    private float TimeToHideError = 5f;
    [SerializeField] private TMP_Text UsersOnline;
    public ShooterNetworkManager manager;

    public List<RootData> RootData { get; set; } = new List<RootData>();
    [SerializeField] private AutoHostClient ahclient;


    void Lang()
    {
        MyLangKayboard GetKey = new MyLangKayboard();
        valueLang = GetKey.GetKeyboardLayout().ToString();
        switch (valueLang)
        {
            case "1033":
                LanguagePC.text = "EN";
                break;
            case "1049":
                LanguagePC.text = "RU";
                break;

        }
    }
    void Start()
    {
        manager = ShooterNetworkManager.singleton;
        if (manager != null)
            UsersOnline.text = "Number of players: " + manager.numPlayers;
        else ErrorText.text = "ShooterNetworkManager NULL";

        Lang();
        ButtonLogin.onClick.AddListener(Login);
    }

    void Update()
    {
        Lang();
        if (TimeToHideError > 0) TimeToHideError -= Time.deltaTime;
        else
        {
            TimeToHideError = 5f;
            ErrorText.text = "";
        }

    }

    public void OpenRegister() => Application.OpenURL("https://blueboxproduction.ru/ucp/register/");

    void Login()
    {
        StartCoroutine(LoginCoroutine(loginInput.text, passwordInput.text));
    }

    IEnumerator LoginCoroutine(string login, string password)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", login);
        form.AddField("password", password);

        UnityWebRequest www = UnityWebRequest.Post(loginUrl, form);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            ErrorText.text = "������: " + www.error;
        }
        else
        {
            string responseText = www.downloadHandler.text;
            switch (responseText)
            {
                case "WrongPass":
                    ErrorText.text = "������ �����. �� ������ ����� ��� ������.";
                    break;
                case "NoAccount":
                    ErrorText.text = "������. ��� ������ ��������.";
                    break;
                case "Blank":
                    ErrorText.text = $"������. {responseText}";
                    break;
                default:
                    responseText = responseText.Replace("NOT", "");
                    RootData = JsonSerializeHelper.Deserialize<List<RootData>>(responseText);
                    foreach (var acc in RootData)
                    {
                        Debug.LogWarning($"��� ID - {acc.pubAccount.id}" +
                                        $"\r\n��� ����� - {acc.pubAccount.login}" +
                                        $"\r\n��� ������ - {acc.pubAccount.passwd}" +
                                        $"\r\n��� EMAIL - {acc.pubAccount.email}" +
                                        $"\r\n��� ������� - {acc.pubAccount.mobile_number}" +
                                        $"\r\n����� ���������� ����� {acc.pubAccount.last_login}" +
                                        $"\r\n������ {acc.myGame[0].coins}");

                        if (PlayerPrefs.HasKey("PlayerName"))
                            PlayerPrefs.DeleteKey("PlayerName");
                        PlayerPrefs.SetString("PlayerName", acc.pubAccount.login);
                        ahclient.StartClient();
                    }

                    break;
            }
        }
    }
}
