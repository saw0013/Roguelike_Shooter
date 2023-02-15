using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefaultItemAmmoUI : MonoBehaviour
{
    [SerializeField] private TMP_Text textTime;

    private float timeBuff = 60;

    private PlayerData owner;

    private bool isDestroy = false;

    void Start()
    {
        textTime.text = $"�����: {timeBuff}";
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
                    textTime.text = $"�����: {second}";
            }
            else
            {
                GetComponent<Image>().CrossFadeColor(new Color(0.5f, 0, 1, 0.4f), 2, false, true);
                Destroy(textTime.gameObject);
                owner.StopBuffAmmo();
            }
        }
    }

    public void RegisterOwner(PlayerData ownerItem) => owner = ownerItem;
}
