using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using UnityEngine;

public class BroadcastService : IBroadcatService
{
    private const string ServerAddress = "https://localhost:7778";
    private const string Hub = "/chat";

    private HubConnection _connection;

    public event Action<string> OnMessageRecived;

    public async void Initialize()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(ServerAddress + Hub)
            .Build();

        _connection.On("UserNotify", (string message) => //�� ��� �� ��������� � ������������
        {
            OnMessageRecived?.Invoke(message);
            Debug.LogWarning($"<color=Yellow>SignalR Info</color>: {message}");
        });

        await _connection.StartAsync();
    }

    /// <summary>
    /// ������������� ���� � ������
    /// </summary>
    /// <param name="roomId"></param>
    /// <param name="username"></param>
    /// <returns></returns>
    public async Task InitializeGroupAsync(string roomId, string username)
    {
        await _connection.SendAsync("InitializeGroup", roomId, username);
    }

    #region �������� ������

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

}


#region Interface Abstraction

public interface IBroadcatService
{
    event Action<string> OnMessageRecived;

    public void Initialize();
}

#endregion
