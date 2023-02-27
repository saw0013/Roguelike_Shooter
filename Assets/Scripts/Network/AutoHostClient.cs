using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MirrorBasics
{
    public class AutoHostClient : MonoBehaviour
    {

        [SerializeField] NetworkManager networkManager;
        [SerializeField] TMP_Dropdown _serverSelectDropDown;
        [SerializeField] private Button btnClient;
        [SerializeField] private Button btnServer;

        [SerializeField] private Sprite no_ping, bad_ping, normal_ping, good_ping, bindServer;
        [SerializeField] private Image contentImagePing;
        int _serverIndex = 0;

        void Start()
        {
            _serverSelectDropDown.onValueChanged.AddListener(delegate
            {
                DropdownValueChanged(_serverSelectDropDown);
            });

            if (!Application.isBatchMode)
            {
                Debug.Log($"=== Client Build ===");
                //networkManager.StartClient ();
            }
            else
            {
                Debug.Log($"=== Server Build ===");
            }
            StartCoroutine(StartPing("127.0.0.1"));
        }

        public void JoinLocal()
        {
            networkManager.networkAddress = "localhost";
            networkManager.StartClient();
        }

        void DropdownValueChanged(TMP_Dropdown change)
        {
            _serverIndex = change.value;
            switch (_serverIndex)
            {
                case 0:
                    StartCoroutine(StartPing("127.0.0.1"));
                    networkManager.networkAddress = "localhost";
                    btnClient.interactable = true;
                    btnServer.interactable = true;
                    break;
                case 1:
                    StartCoroutine(StartPing("92.241.230.143"));
                    networkManager.networkAddress = "92.241.230.143";
                    btnClient.interactable = true;
                    btnServer.interactable = false;
                    break;
                case 2:
                    StartCoroutine(StartPing("92.241.230.141"));
                    networkManager.networkAddress = "92.241.230.141";
                    btnClient.interactable = true;
                    btnServer.interactable = false;
                    break;
                case 3:
                    contentImagePing.sprite = bindServer;
                    networkManager.networkAddress = "0.0.0.0";
                    btnClient.interactable = false;
                    btnServer.interactable = true;
                    break;
            }
        }

        public void StartHost()
        {
            networkManager.StartServer();
        }

        public void StartClient()
        {
            networkManager.StartClient();
        }


        IEnumerator StartPing(string ip)
        {
            WaitForSeconds f = new WaitForSeconds(0.05f);
            Ping p = new Ping(ip);
            while (p.isDone == false)
            {
                yield return f;
            }

            PingFinished(p);

        }

        public void PingFinished(Ping p)
        {
            if (p.time <= 50) contentImagePing.sprite = good_ping; //Если пинг до 50мс это отличный пинг
            else if (p.time <= 100) contentImagePing.sprite = normal_ping; //Если пинг до 100мс это нормальный пинг
            else if (p.time > 100) contentImagePing.sprite = bad_ping; //Свыше 100 пинг плохой
            else contentImagePing.sprite = no_ping;
        }
    }
}