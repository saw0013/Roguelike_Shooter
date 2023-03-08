using System.Threading.Tasks;
using FMOD;
using UnityEngine;


public class ChatManager : MonoBehaviour
{
    public void START_TEST() => Initialize();

    private async void Initialize()
    {
        BroadcastService bs = new BroadcastService();
        bs.Initialize();
        await Task.Delay(1000);
        UnityEngine.Debug.LogWarning("���������");
        bs.OnMessageRecived += Bs_OnMessageRecived;
        //bs.SendTest("AABB1", "anomal3"); //������� ����������
        await bs.InitializeGroupAsync("AABB1", "anomal3");
    }

    private void Bs_OnMessageRecived(string obj)
    {
        UnityEngine.Debug.LogWarning("<color=Blue>ASYNC SignalR</color>: ������ ����������� Action � ������ " + System.Threading.Thread.CurrentThread.ManagedThreadId);
    }
}
