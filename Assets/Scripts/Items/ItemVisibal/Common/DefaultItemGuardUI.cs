using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefaultItemGuardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text textTime;

    private float timeBuff = 60;

    private PlayerData owner;

    private bool isDestroy = false;

    void Start()
    {
        textTime.text = $"Время: {timeBuff}";
    }

    void Update()
    {
        if (!isDestroy)
        {
            if (timeBuff > 0)
            {
                timeBuff -= Time.deltaTime;
                var second = Mathf.RoundToInt(timeBuff);
                if (textTime != null)
                    textTime.text = $"Время: {second}";
            }
            else
            {
                textTime.gameObject.SetActive(false);
                GetComponent<Image>().CrossFadeColor(new Color(0, 0, 1, 0.4f), 2, false, true);
                owner.StopBuffGuard();
                isDestroy = true;
            }
        }
    }

    public void RegisterOwner(PlayerData ownerItem) => owner = ownerItem;

    public void UpdateBuff()
    {
        //Заново формируем таймер
        timeBuff += 60;
        isDestroy = false;

        //Снова включаем таймер
        textTime.gameObject.SetActive(true);
        GetComponent<Image>().CrossFadeColor(new Color(0, 0, 1, 1f), 2, false, true);

    }
}
