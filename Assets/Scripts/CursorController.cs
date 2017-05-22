using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour {

    [SerializeField] private Sprite cursorDefault;
    [SerializeField] private Sprite cursorDefaultSelect;
    [SerializeField] private Sprite cursorHover;
    [SerializeField] private Sprite cursorHoverSelect;

    private SpriteRenderer spriteRenderer;

    Vector3 position;

    // Use this for initialization
    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        Cursor.visible = false;
        SetCursor(CursorState.Hidden);
    }

    void Update() {

        position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        position.z = 0;
        this.transform.position = position;

    }

    public void SetCursor(CursorState state) {

        switch (state) {

            case CursorState.Default:
                spriteRenderer.sprite = cursorDefault;
                break;
            case CursorState.DefaultSelect:
                spriteRenderer.sprite = cursorDefaultSelect;
                break;
            case CursorState.Hover:
                spriteRenderer.sprite = cursorHover;
                break;
            case CursorState.HoverSelect:
                spriteRenderer.sprite = cursorHoverSelect;
                break;
            case CursorState.Hidden:
                spriteRenderer.sprite = null;
                break;

        }

    }

}

public enum CursorState {
    Hidden,
    Default,
    DefaultSelect,
    Hover,
    HoverSelect
}