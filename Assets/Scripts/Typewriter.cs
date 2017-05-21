﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Typewriter : MonoBehaviour {

    [HeaderAttribute("References")]
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private TextMeshProUGUI inputBox;
    [SerializeField] private TextMeshProUGUI decorationText;

    [SpaceAttribute]
    [HeaderAttribute("Input Box Constrains")]
    [SerializeField] private TMP_FontAsset font;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Color styleColor = Color.white;
    [SerializeField] private char cursorIcon = '_';
    [SerializeField] private int characterLimit = 16;
    [SerializeField] private int monospaceValue = 20;
    [SerializeField] private float characterDeleteDelay = 0.1f;

    // Text deletion
    [SerializeField] private float backspaceSoundDelay = 0.1f;
    private bool isDeletingLastCharacter = false;
    private float backspaceHoldTime = 0;
    private bool isPlayingDeleteSound = false;

    // Blink cursor
    [SerializeField] private float blinkCursorShowDelay = 0.5f;
    private bool isShowingBlinkCursor = false;

    // Allowances
    [SpaceAttribute]
    [HeaderAttribute("Key constrains")]
    [SerializeField] private bool allowEveryKey = false;
    [SerializeField] private bool allowAlphabeticKeys = true;
    [SerializeField] private bool allowNumericKeys = true;

    [SerializeField] private bool canAddCharacter = true;
    private float characterResetTimer = 0;
    [SerializeField] private int textLength;
    [SerializeField] private int textPosition;

    // Use this for initialization
    void Start() {

        if (!soundManager) {
            soundManager = FindObjectOfType<SoundManager>();
        }

        if (!inputBox) {
            inputBox = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        }

        if (!decorationText) {
            decorationText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        }

        // Set the fonts
        inputBox.font = font;
        decorationText.font = font;

        string decorText = decorationText.text;
        decorationText.text = "<#" + Utils.ColorToHex(styleColor) + ">" + decorText;

        // Empty the text
        inputBox.text = "<mspace=" + monospaceValue + ">";

    }

    // Update is called once per frame
    void Update() {

        // Check for most key presses
        if (Input.anyKeyDown && (!Input.GetKey(KeyCode.Backspace) && !Input.GetKey(KeyCode.Return) && !Input.GetKey(KeyCode.Escape))) {
            textPosition = Input.inputString.Length - 1;

            // if (!canAddCharacter)
            // return;

            if (textPosition > Input.inputString.Length || textPosition < 0) {
                return;
            }

            char characterPressed = Input.inputString[textPosition];

            if (!allowEveryKey) {

                bool isValidKey = ValidateKey(characterPressed);

                if (isValidKey) {

                    AddCharacter(characterPressed);

                }

            } else {

                AddCharacter(characterPressed);

            }

        } else {

            if (Input.GetKey(KeyCode.Backspace)) {
                backspaceHoldTime += Time.deltaTime;
                DeleteLastCharacter();
            } else if (Input.GetKeyUp(KeyCode.Backspace)) {
                isDeletingLastCharacter = false;
                backspaceHoldTime = 0;
            } else {
                if (!isShowingBlinkCursor && textLength < characterLimit) {
                    StartCoroutine(ShowCursor());
                }
            }

        }

        if (!canAddCharacter) {
            characterResetTimer += Time.deltaTime;
        }

        if (characterResetTimer > 0.1f) {
            canAddCharacter = true;
            characterResetTimer = 0;
        }

    }

    bool ValidateKey(char key) {

        if (allowAlphabeticKeys) {

            // Uppercase check
            char c = 'A';

            for (int i = 0; i < 26; i++) {

                if (key == (c + i))
                    return true;

            }

            // Lowercase
            c = 'a';

            for (int i = 0; i < 26; i++) {

                if (key == (c + i))
                    return true;

            }

            if (key == ' ')
                return true;

        }

        if (allowNumericKeys) {

            // Uppercase check
            char c = '1';

            for (int i = 0; i < 9; i++) {

                if (key == (c + i))
                    return true;

            }

            if (key == '0')
                return true;

        }

        return false;

    }

    void AddCharacter(char character) {

        RemoveCursorAndSetStateTo(false, true);

        if (textLength >= characterLimit) {
            return;
        }

        soundManager.PlayKeyboardClick();

        // TODO Check if the text exceeds the size of the bounding box
        inputBox.text += "<#" + Utils.ColorToHex(textColor) + ">" + character;

        textLength++;

        canAddCharacter = false;

    }

    void DeleteLastCharacter() {

        if (isDeletingLastCharacter) {
            return;
        }

        StopCoroutine("ShowCursor");
        RemoveCursorAndSetStateTo(true, true);
        StartCoroutine(DeleteDelay());

    }

    private IEnumerator DeleteDelay() {

        string text = inputBox.text;

        int length = text.Length - 1;

        if (inputBox.text.Length >= ("<mspace=" + monospaceValue + ">").Length + 1) {

            if (length >= 0) {

                isDeletingLastCharacter = true;

                soundManager.PlayDelete();

                text = text.Remove(length - 9);

                inputBox.text = text;

                textLength--;

                if (backspaceHoldTime < 1) {
                    yield return new WaitForSeconds(characterDeleteDelay);
                } else {
                    yield return new WaitForSeconds(0.05f);
                }

                isDeletingLastCharacter = false;

            }

        }
        // Deletion is invalid
        else {

        }

        if (isShowingBlinkCursor) {
            isShowingBlinkCursor = false;
        }

    }

    private IEnumerator PlayDeleteSound() {

        if (isPlayingDeleteSound) {
            yield break;
        }

        isPlayingDeleteSound = true;
        yield return new WaitForSeconds(backspaceSoundDelay);
        soundManager.PlayDelete();
        isPlayingDeleteSound = false;

    }

    private IEnumerator ShowCursor() {

        RemoveCursorAndSetStateTo(true, false);

        inputBox.text += "<#" + Utils.ColorToHex(styleColor) + ">" + cursorIcon;
        isShowingBlinkCursor = true;

        yield return new WaitForSeconds(blinkCursorShowDelay);

        RemoveCursorAndSetStateTo(true, false);

        // This was causing it to delete the last character, it should only delete the cursor that was added
        // textMesh.text = textMesh.text.Remove (textMesh.text.Length - 1);

        yield return new WaitForSeconds(blinkCursorShowDelay);
        isShowingBlinkCursor = false;

    }

    private void RemoveCursorAndSetStateTo(bool keepCursorState, bool shouldStopAllCoroutines) {

        int cursorIndex = inputBox.text.IndexOf("<#" + Utils.ColorToHex(styleColor) + ">" + cursorIcon);
        int cursorLength = ("<#" + Utils.ColorToHex(styleColor) + ">" + cursorIcon).Length;

        if (cursorIndex != -1) {

            if (shouldStopAllCoroutines) {
                StopAllCoroutines();
            }

            inputBox.text = inputBox.text.Remove(cursorIndex, cursorLength);
            isShowingBlinkCursor = keepCursorState;

        }

    }

}