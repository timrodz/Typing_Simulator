using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Typewriter : MonoBehaviour {

    [HeaderAttribute ("References")]
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private TextMeshProUGUI inputText;
    [SerializeField] private TextMeshProUGUI decorationText;

    [SpaceAttribute]
    [HeaderAttribute ("Input Box Constrains")]
    [SerializeField] private TMP_FontAsset font;
    [SerializeField] private Color textColor = Color.white;
    [SerializeField] private Color styleColor = Color.white;
    [SerializeField] private char cursorIcon = '_';
    [SerializeField] private int characterLimit = 16;
    [SerializeField] private int monospaceValue = 12;
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
    [HeaderAttribute ("Key constrains")]
    [SerializeField] private bool allowEveryKey = false;
    [SerializeField] private bool allowAlphabeticKeys = true;
    [SerializeField] private bool allowNumericKeys = true;

    private int textLength;
    private int textPosition;
    private int startIndex;

    [SpaceAttribute]
    [HeaderAttribute ("Text logger")]
    [SerializeField] private TextMeshProUGUI logText;

    // Use this for initialization
    void Start () {

        if (!soundManager) {
            soundManager = FindObjectOfType<SoundManager> ();
        }

        if (!inputText) {
            inputText = transform.GetChild (2).GetComponent<TextMeshProUGUI> ();
        }

        if (!decorationText) {
            decorationText = transform.GetChild (3).GetComponent<TextMeshProUGUI> ();
        }

        if (!logText) {
            logText = transform.GetChild (4).GetComponent<TextMeshProUGUI> ();
        }

        // Set the fonts
        inputText.font = font;
        decorationText.font = font;

        string decorText = decorationText.text;
        decorationText.text = "<#" + Utils.ColorToHex (styleColor) + ">" + decorText;

        // Empty the text
        inputText.text = "<mspace=" + monospaceValue + ">" + "<#" + Utils.ColorToHex (textColor) + ">";

        logText.text = "<mspace=" + monospaceValue + ">";

        startIndex = inputText.text.Length;

    }

    // Update is called once per frame
    void Update () {

        // Check for most key presses
        if (Input.anyKeyDown && (!Input.GetKey (KeyCode.Backspace) && !Input.GetKey (KeyCode.Return) && !Input.GetKey (KeyCode.Escape))) {
            textPosition = Input.inputString.Length - 1;

            if (textPosition > Input.inputString.Length || textPosition < 0) {
                return;
            }

            char characterPressed = Input.inputString[textPosition];

            bool isValidKey = ValidateKey (characterPressed);

            if (isValidKey) {

                AddCharacter (characterPressed);

            }

        } else {

            if (Input.GetKeyDown (KeyCode.Return)) {

                EnterMessage ();

            } else if (Input.GetKey (KeyCode.Backspace)) {
                backspaceHoldTime += Time.deltaTime;
                DeleteLastCharacter ();
            } else if (Input.GetKeyUp (KeyCode.Backspace)) {
                isDeletingLastCharacter = false;
                backspaceHoldTime = 0;
            } else {
                if (!isShowingBlinkCursor && textLength < characterLimit) {
                    StartCoroutine (ShowCursor ());
                }
            }

        }

    }

    /// <summary>
    /// Enter message for processing
    /// </summary>
    private void EnterMessage () {

        RemoveCursorAndSetStateTo (false, true);

        if (inputText.text.Length <= startIndex)
            return;

        textLength = 0;

        logText.text += inputText.text.Substring (startIndex, inputText.text.Length - startIndex) + "\n";

        inputText.text = "<mspace=" + monospaceValue + ">" + "<#" + Utils.ColorToHex (textColor) + ">";

    }

    private bool ValidateKey (char key) {

        if (allowEveryKey) {

            if (key < 128) {
                return true;
            }

        } else {

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

        }

        return false;

    }

    private void AddCharacter (char character) {

        RemoveCursorAndSetStateTo (false, true);

        if (textLength >= characterLimit) {
            return;
        }

        soundManager.PlayKeyboardClick ();

        // TODO Check if the text exceeds the size of the bounding box
        // inputText.text += "<#" + Utils.ColorToHex (textColor) + ">" + character;
        inputText.text += character;

        textLength++;

    }

    private void DeleteLastCharacter () {

        if (isDeletingLastCharacter) {
            return;
        }

        StopCoroutine ("ShowCursor");
        RemoveCursorAndSetStateTo (true, true);
        StartCoroutine (DeleteDelay ());

    }

    private IEnumerator DeleteDelay () {

        string text = inputText.text;

        int length = text.Length - 1;

        if (inputText.text.Length >= startIndex + 1) {

            if (length >= 0) {

                isDeletingLastCharacter = true;

                soundManager.PlayDelete ();

                text = text.Remove (length);

                inputText.text = text;

                textLength--;

                if (backspaceHoldTime < 1) {
                    yield return new WaitForSeconds (characterDeleteDelay);
                } else {
                    yield return new WaitForSeconds (0.05f);
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

    private IEnumerator ShowCursor () {

        RemoveCursorAndSetStateTo (true, false);

        inputText.text += "<#" + Utils.ColorToHex (styleColor) + ">" + cursorIcon;
        isShowingBlinkCursor = true;

        yield return new WaitForSeconds (blinkCursorShowDelay);

        RemoveCursorAndSetStateTo (true, false);

        // This was causing it to delete the last character, it should only delete the cursor that was added
        // textMesh.text = textMesh.text.Remove (textMesh.text.Length - 1);

        yield return new WaitForSeconds (blinkCursorShowDelay);
        isShowingBlinkCursor = false;

    }

    private void RemoveCursorAndSetStateTo (bool keepCursorState, bool shouldStopAllCoroutines) {

        int cursorIndex = inputText.text.IndexOf ("<#" + Utils.ColorToHex (styleColor) + ">" + cursorIcon);
        int cursorLength = ("<#" + Utils.ColorToHex (styleColor) + ">" + cursorIcon).Length;

        if (cursorIndex != -1) {

            if (shouldStopAllCoroutines) {
                StopAllCoroutines ();
            }

            inputText.text = inputText.text.Remove (cursorIndex, cursorLength);
            isShowingBlinkCursor = keepCursorState;

        }

    }

}