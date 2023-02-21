using UnityEngine;
using UnityEngine.EventSystems;

public enum CloseOption
{
    DoNothing,
    DeactivateWindow,
    DestroyWindow
}
[AddComponentMenu("Cosmoground/UIWindowDrag")]
public class UIWindow : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // Опция закрытия
    public CloseOption onClose = CloseOption.DeactivateWindow;

    // перетащил UIWindow на случай, если он понадобится где-то еще
    public static UIWindow currentlyDragged;

    // кэш
    Transform window;

    void Awake()
    {
        // кэш parent окна
        window = transform.parent;
    }

    public void HandleDrag(PointerEventData d)
    {
        // отправить сообщение, если родитель должен знать об этом
        window.SendMessage("OnWindowDrag", d, SendMessageOptions.DontRequireReceiver);

        // переместить родителя
        window.Translate(d.delta);
    }

    public void OnBeginDrag(PointerEventData d)
    {
        currentlyDragged = this;
        HandleDrag(d);
    }

    public void OnDrag(PointerEventData d)
    {
        HandleDrag(d);
    }

    public void OnEndDrag(PointerEventData d)
    {
        HandleDrag(d);
        currentlyDragged = null;
    }

    // OnClose вызывается кнопкой закрытия через Callbacks Inspector. 
    public void OnClose()
    {
        // отправляем сообщение в случае необходимости
        // примечание: важно не называть ее так же, как ЭТА функция, чтобы избежать тупика
        window.SendMessage("OnWindowClose", SendMessageOptions.DontRequireReceiver);

        // спрятать окно
        if (onClose == CloseOption.DeactivateWindow)
            window.gameObject.SetActive(false);

        // уничтожить если надо
        if (onClose == CloseOption.DestroyWindow)
            Destroy(window.gameObject);
    }
}
