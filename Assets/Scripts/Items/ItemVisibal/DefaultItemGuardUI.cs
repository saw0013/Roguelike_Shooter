using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefaultItemGuardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text textTime;

    private float timeBuff = 60;

    private PlayerData owner;

    void Start()
    {
        textTime.text = $"Время: {timeBuff}";
    }

    void Update()
    {
        if (timeBuff > 0)
        {
            timeBuff -= Time.deltaTime;
            var second = Mathf.RoundToInt(timeBuff);
            textTime.text = $"Время: {second}";
        }
        else
        {
            GetComponent<Image>().CrossFadeColor(new Color(0, 0, 1, 0.4f), 2, false, true);
            Destroy(textTime.gameObject);
            owner.StopBuffGuard();
        }
    }

    public void RegisterOwner(PlayerData ownerItem) => owner = ownerItem;
}
