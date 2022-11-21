using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursorMan : MonoBehaviour
{
    [SerializeField] internal Texture2D cursor, cursorNormal, cursorHand, cursorInputText, cursorAttack, cursorNo;

    private void Awake() => DontDestroyOnLoad(this);
    void Start() => Cursor.visible = false;

    void OnGUI()
    {
        GUI.depth = 1;
        GUI.DrawTexture(new Rect(Input.mousePosition.x - 12.0f, Screen.height - Input.mousePosition.y - 2.0f, 20.0f, 20.0f), cursor);

    }
}
