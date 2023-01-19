using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameTimer : NetworkBehaviour
{
    [SyncVar, HideInInspector]
    public float timer = 1 * 60;  // 1 ������ �� ���������

    private int minutes;
    private int seconds;
    public string showTime;         // ����������� �������� ������ ������� ������������ � ������� 09:37.

    [SyncVar]
    public bool running = false;    // ������ ��� ��� ��������?

    [SerializeField]private TMP_Text clockText;     // ����� �������, ������������ � UI

    public UnityEvent ClockReady;   // �������, ������� ����������, ����� ������ ��������� ����

    void Update()
    {
        if (!running)
        {
            return;
        }

        // ��������� �������� ������� �� ���� ������� ������� ��:
        if (timer > 0)
            timer -= Time.deltaTime;

        // �������������� ������� � ������
        minutes = Mathf.FloorToInt(timer / 60F);
        seconds = Mathf.FloorToInt(timer - minutes * 60);
        showTime = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (timer < 0)
        {
            running = false;
            showTime = "00:00";
            // Callback event ����� ������ ��������� ����
            ClockReady.Invoke();
        }

        if (clockText)
        {
            clockText.text = showTime;
        }
        else Debug.LogError("GameTimer.Update(): timer = null.");
    }
}

