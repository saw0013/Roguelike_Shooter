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

    void OnGUI()
    {
        GUI.depth = 1;
        GUI.DrawTexture(new Rect(Input.mousePosition.x - 1.0f, Screen.height - Input.mousePosition.y - 2.0f, SizeCursor, SizeCursor), cursor);

    }
}
