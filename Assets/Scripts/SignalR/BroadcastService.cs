using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;

public class BroadcastService : IBroadcatService, IDisposable
{

#if UNITY_EDITOR || UNITY_EDITOR_WIN || UNITY_EDITOR_64
    public static string ServerAddress = "https://localhost:7778";
#else 
public static string ServerAddress = "http://localhost:7778";
#endif

    private const string Hub = "/chat";

    private HubConnection _connection;

    public event Action<string> OnMessageRecived;
    public event Action<string, string> OnMessageGroupRecived;
    public event Action<string, string> OnMessageGroupLobbyRecived;
    public event Action<string> OnSystemMessageRecived;


    public static new BroadcastService singleton { get; private set; }

    private bool isInit = false;

    public async void Initialize()
    {
        Debug.LogWarning(isInit + " KTO");
      
        if (isInit) return;

        _connection = new HubConnectionBuilder()
            .WithUrl(ServerAddress + Hub)
            .Build();

        _connection.On("UserNotify", (string message) => //�� ��� �� ��������� � ������������
        {
            OnMessageRecived?.Invoke(message);
            Debug.LogWarning($"<color=Yellow>SignalR Info</color>: {message}");
        });

        _connection.On("ReceiveLobbyGroup", (string username, string message) => { OnMessageGroupLobbyRecived?.Invoke(username, message); }); //��������� ��������� � ����

        _connection.On("ReceiveGroup", (string username, string message) => { OnMessageGroupRecived?.Invoke(username, message); }); //��������� ��������� � ����

        _connection.On("ReceiveClient", (string message) => { OnMessageRecived?.Invoke(message); }); //��������� ��������� �����

        _connection.On("System", (string message) => { OnSystemMessageRecived?.Invoke(message); }); //��������� ��������� ��������� (������������ ������ � WEB)

        _connection.On("GameMaster", (string message) => { OnSystemMessageRecived?.Invoke(message); }); //TODO : ��������� �� ���� �����

        await _connection.StartAsync();

        isInit = true;
    }

    /// <summary>
    /// ������������� ���� � ������
    /// </summary>
    /// <param name="roomId"></param>
    /// <param name="username"></param>
    /// <returns></returns>
    public async Task InitializeLobbyAsync(string roomId, string username)
    {
        await _connection.SendAsync("InitializeLobby", roomId, username);
    }

    public async Task InitializeMatchAsync(string roomId)
    {
        await _connection.SendAsync("InitializeMatch", roomId);
    }

    /// <summary>
    /// ���������� ��������� � ������
    /// </summary>
    /// <param name="roomId"></param>
    /// <param name="username"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task SendMessageGroupLobbyAsync(string roomId, string username, string message)
    {
        await _connection.SendAsync("SendMessageToLobbyGroup", roomId, username, message);
    }

    public async Task SendMessageGroupAsync(string roomId, string username, string message)
    {
        await _connection.SendAsync("SendMessageToGroup", roomId, username, message);
    }



    #region �������� ������

    /// <summary>
    /// �������� ��������� �� ������� �� ������. ����� ����
    /// </summary>
    /// <param name="username"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    [Obsolete("�������� �����. ������������� ��� ����� ����", true)]
    public async Task SendMessageTestAsync(string username, string message)
    {
        await _connection.SendAsync("SendMessageTest", username, message);
    }

    /// <summary>
    /// �������� ����� ������� �� �����������
    /// <para>��������� <see cref="InitializeGroupAsync"/></para>
    /// </summary>
    /// <param name="roomId"></param>
    /// <param name="username"></param>
    [Obsolete("�������� �����. ���������� ������� �����", true)]
    internal void SendTest(string roomId, string username)
    {
        _connection.InvokeAsync("InitializeGroup", roomId, username);
    }

    #endregion

    public void Dispose()
    {
        OnMessageRecived = null;
        OnMessageGroupRecived = null;
        OnSystemMessageRecived = null;
    }

    ~BroadcastService() { }
}


#region Interface Abstraction

public interface IBroadcatService
{
    event Action<string> OnMessageRecived;

    event Action<string, string> OnMessageGroupRecived;
    event Action<string, string> OnMessageGroupLobbyRecived;

    event Action<string> OnSystemMessageRecived;

    public void Initialize();
}

#endregion
