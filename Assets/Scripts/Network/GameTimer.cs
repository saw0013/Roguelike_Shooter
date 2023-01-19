using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameTimer : NetworkBehaviour
{
    [SyncVar, HideInInspector]
    public float timer = 1 * 60;  // 1 минута до окончания

    private int minutes;
    private int seconds;
    public string showTime;         // Фактический обратный отсчет таймера отображается в формате 09:37.

    [SyncVar]
    public bool running = false;    // Таймер все еще работает?

    [SerializeField]private TMP_Text clockText;     // Текст таймера, отображаемый в UI

    public UnityEvent ClockReady;   // Событие, которое вызывается, когда таймер достигает нуля

    void Update()
    {
        if (!running)
        {
            return;
        }

        // Уменьшаем значение таймера по мере отсчета времени на:
        if (timer > 0)
            timer -= Time.deltaTime;

        // Преобразование таймера в строку
        minutes = Mathf.FloorToInt(timer / 60F);
        seconds = Mathf.FloorToInt(timer - minutes * 60);
        showTime = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (timer < 0)
        {
            running = false;
            showTime = "00:00";
            // Callback event когда таймер достигает нуля
            ClockReady.Invoke();
        }

        if (clockText)
        {
            clockText.text = showTime;
        }
        else Debug.LogError("GameTimer.Update(): timer = null.");
    }
}

