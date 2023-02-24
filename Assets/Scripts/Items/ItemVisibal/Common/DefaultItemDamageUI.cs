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
                textTime.gameObject.SetActive(false);
                GetComponent<Image>().CrossFadeColor(new Color(1, 0, 0, 0.4f), 2, false, true);
                owner.StopBuffDamage(25);
                isDestroy = true;
            }
        }
    }

    public void RegisterOwner(PlayerData ownerItem) => owner = ownerItem;

    public void UpdateBuff()
    {
        //������ ��������� ������
        timeBuff += 60;
        isDestroy = false;

        //����� �������� ������
        textTime.gameObject.SetActive(true);
        GetComponent<Image>().CrossFadeColor(new Color(1, 0, 0, 1f), 2, false, true);

    }
}
