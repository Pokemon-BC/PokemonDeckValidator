using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UIElements;

public class UIToolkitValidator : MonoBehaviour
{
    private DropdownField formatSelect;
    private TextField decklistEntry;
    private Button submitButton;

    private void OnEnable()
    {
        VisualElement rootElement = GetComponent<UIDocument>().rootVisualElement;

        formatSelect = rootElement.Q<DropdownField>("format-select");
        decklistEntry = rootElement.Q<TextField>("decklist-entry");
        submitButton = rootElement.Q<Button>("submit-button");

        submitButton.RegisterCallback<ClickEvent>(ev => SubmitCallback(ev));
    }

    private void SubmitCallback(ClickEvent ev)
    {
        Debug.Log("Submit pressed");
    }
}
