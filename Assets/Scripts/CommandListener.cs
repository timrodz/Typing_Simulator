using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CommandListener : MonoBehaviour {

    [Header("References")]
    [SerializeField] private Typewriter typewriter;
    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private CursorController cursorController;

    [Space]
    [Header("Attributes")]
    [SerializeField] private bool allowLogging = true;

    private void Start() {

        if (!typewriter) {
            typewriter = FindObjectOfType<Typewriter>();
        }

        if (!cursorController) {
            cursorController = FindObjectOfType<CursorController>();
        }

        logText.text = "<mspace=" + typewriter.monospaceValue + ">";

    }

    public void InterpretCommand(string command) {

        Debug.Log("Message to interpret: " + command);

        string output = command;

        // Switch on lowercase commands (Less checking)
        switch (command.ToLower()) {

            case "help":
            case "/help":
                output = "Help toggled";
                break;
            case "select":
            case "/select":
                output = "Select an object to edit";
                typewriter.AllowTyping(false);
                cursorController.SetCursor(CursorState.Default);
                break;
        }

        Debug.Log(output);

        if (allowLogging) {
            logText.text += output + "\n";
        }

    }

}
