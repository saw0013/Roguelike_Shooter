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
    public event Action<string, string> OnMessageGroupRecived;
    public event Action<string> OnSystemMessageRecived;

    public async void Initialize()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(ServerAddress + Hub)
            .Build();

        _connection.On("UserNotify", (string message) => //То что мы принимаем и обрабатываем
        {
            OnMessageRecived?.Invoke(message);
            Debug.LogWarning($"<color=Yellow>SignalR Info</color>: {message}");
        });

        _connection.On("Receive", (string message, string username) =>
        {
            OnMessageGroupRecived?.Invoke(message, username);
        });

        _connection.On("Receive", (string message) => { OnMessageRecived?.Invoke(message); });

        _connection.On("System", (string message) => { OnMessageRecived?.Invoke(message); });

        _connection.On("GameMaster", (string message) => { OnSystemMessageRecived?.Invoke(message); });

        await _connection.StartAsync();
    }

    /// <summary>
    /// Инициализация чата и группы
    /// </summary>
    /// <param name="roomId"></param>
    /// <param name="username"></param>
    /// <returns></returns>
    public async Task InitializeGroupAsync(string roomId, string username)
    {
        await _connection.SendAsync("InitializeGroup", roomId, username);
    }

    /// <summary>
    /// Отправляем сообщение в группу
    /// </summary>
    /// <param name="roomId"></param>
    /// <param name="username"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task SendMessageGroupAsync(string roomId, string username, string message)
    {
        await _connection.SendAsync("SendMessageToGroup",  roomId, username, message);
    }

    #region Тестовые методы

    /// <summary>
    /// Тестовый метод запроса не асинхронный
    /// <para>Используй <see cref="InitializeGroupAsync"/></para>
    /// </summary>
    /// <param name="roomId"></param>
    /// <param name="username"></param>
    [Obsolete("Тестовый метод. Использует главный поток", true)]
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

    event Action<string, string> OnMessageGroupRecived;

    event Action<string> OnSystemMessageRecived;

    public void Initialize();
}

#endregion
