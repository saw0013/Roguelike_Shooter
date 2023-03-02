using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using UnityEngine;

public class SelectorGame : MonoBehaviour
{
    private bool isPingComplete = false;
    private int pingTime = -1;

    public void Ping()
    {
        PingServer("92.241.230.143", 7777);
    }

    public void PingServer(string ipAddress, int port)
    {
        // Создаем новый поток для выполнения пинга
        Thread pingThread = new Thread(() =>
        {
            // Создаем объект для отправки пакетов UDP
            UdpClient udpClient = new UdpClient();
            udpClient.Connect(ipAddress, port);
            UnityEngine.Debug.LogWarning("ПАКЕТ ОТПРАВЛЕН");
            // Отправляем пакет и замеряем время отклика
            byte[] data = new byte[1];
            DateTime sendTime = DateTime.Now;
            udpClient.Send(data, data.Length);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] receiveData = udpClient.Receive(ref endPoint);
            UnityEngine.Debug.LogWarning("ПАКЕТ ПОЛУЧЕН");
            DateTime receiveTime = DateTime.Now;
            pingTime = (int)(receiveTime - sendTime).TotalMilliseconds;
            isPingComplete = true;
        });

        // Запускаем поток
        pingThread.Start();
    }

    void Update()
    {
        if (isPingComplete)
        {
            // Обработка результата пинга
            UnityEngine.Debug.Log("Ping time: " + pingTime + "ms");
            isPingComplete = false;
        }
    }
}