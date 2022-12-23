using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class GameTimer : NetworkBehaviour
{
    [SyncVar]
    public float timer = 15 * 60;  // 15 минут до окончания

    private int minutes;
    private int seconds;
    public string showTime;         // Фактический обратный отсчет таймера отображается в формате 09:37.
    private bool running = true;    // Таймер все еще работает?

    private TMP_Text clockText;     // Текст таймера, отображаемый в UI

    public UnityEvent ClockReady;   // Событие, которое вызывается, когда таймер достигает нуля


    void Awake()
    {
        Debug.Log("GameTimer.Awake(): start");

        //Будем искать таймер????
        GameObject texttimer = GameObject.Find("textTimer");
        if (texttimer)
        {
            clockText = texttimer.GetComponent<TMP_Text>();

            if (clockText == null)
                Debug.LogError("GameTimer.Awake(): Cannot find TMP_Text.");
        }
        else Debug.LogError("GameTimer.Awake(): Cannot find textTimer.");
    }

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
}
