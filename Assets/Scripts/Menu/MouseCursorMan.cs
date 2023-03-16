using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursorMan : MonoBehaviour
{
    [SerializeField] internal Texture2D cursor, cursorNormal, cursorHand, cursorInputText, cursorAttack, cursorNo;
    [SerializeField] private float SizeCursor;
    public Vector2 hotSpot = Vector2.zero;

    private void Awake() => DontDestroyOnLoad(this);

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2)) //Для дебага в релизе убрать
        {
            Cursor.visible = !Cursor.visible;
           if(Cursor.visible) Cursor.lockState = CursorLockMode.None;
           else Cursor.lockState = CursorLockMode.Confined;
        }




    }

    void OnGUI()
    {
        GUI.depth = 1;
        GUI.DrawTexture(new Rect(Input.mousePosition.x - 1.0f, Screen.height - Input.mousePosition.y - 2.0f, SizeCursor, SizeCursor), cursor);

    }
}
