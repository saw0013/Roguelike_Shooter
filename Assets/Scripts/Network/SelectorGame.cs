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
        // ������� ����� ����� ��� ���������� �����
        Thread pingThread = new Thread(() =>
        {
            // ������� ������ ��� �������� ������� UDP
            UdpClient udpClient = new UdpClient();
            udpClient.Connect(ipAddress, port);
            UnityEngine.Debug.LogWarning("����� ���������");
            // ���������� ����� � �������� ����� �������
            byte[] data = new byte[1];
            DateTime sendTime = DateTime.Now;
            udpClient.Send(data, data.Length);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            byte[] receiveData = udpClient.Receive(ref endPoint);
            UnityEngine.Debug.LogWarning("����� �������");
            DateTime receiveTime = DateTime.Now;
            pingTime = (int)(receiveTime - sendTime).TotalMilliseconds;
            isPingComplete = true;
        });

        // ��������� �����
        pingThread.Start();
    }

    void Update()
    {
        if (isPingComplete)
        {
            // ��������� ���������� �����
            UnityEngine.Debug.Log("Ping time: " + pingTime + "ms");
            isPingComplete = false;
        }
    }
}