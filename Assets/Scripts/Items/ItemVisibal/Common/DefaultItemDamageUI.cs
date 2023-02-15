using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefaultItemDamageUI : MonoBehaviour
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
                GetComponent<Image>().CrossFadeColor(new Color(1, 0, 0, 0.4f), 2, false, true);
                Destroy(textTime.gameObject);
                owner.StopBuffMoveSpeed();
            }
        }
    }

    public void RegisterOwner(PlayerData ownerItem) => owner = ownerItem;
}
