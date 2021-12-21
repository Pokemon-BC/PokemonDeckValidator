using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using PKMN.Cards;

public class DeckListValidator : MonoBehaviour
{
    public Button submitButton;
    public Text decklist;
    public FormatSelect formatSelect;
    public GameObject topLevelUI;
    public Transform cardRoot;
    public Transform deckNoteRoots;

    public Button backButton;

    public GameObject cardPrefab;
    public GameObject notePrefab;

    // Start is called before the first frame update
    void Start()
    {
        topLevelUI.SetActive(false);
        submitButton.onClick.AddListener(SubmitButton);
        backButton.onClick.AddListener(BackButton);
    }

    private void SubmitButton()
    {
        Debug.Log("Submit button pressed");
        PokemonFormat targetFormat = formatSelect.GetSelectedFormat();
        Debug.Log("Text is " + decklist.text);
        PokemonDeck loadedDeck = PokemonLoader.LoadDeck(decklist.text);
        targetFormat.CheckDeckInFormat(loadedDeck);
        Debug.Log("Deck loaded with " + loadedDeck.totalCards + " cards");
        foreach(CardInDeck cid in loadedDeck.deckCards)
        {
            if(cid.reference == null)
            {
                continue;
            }
            GameObject createdCard = GameObject.Instantiate(cardPrefab);
            ReportedPokemonCard rpc = createdCard.GetComponent<ReportedPokemonCard>();
            createdCard.transform.SetParent(cardRoot);

            rpc.Configure(cid);
        }
        foreach(FormatedNote fn in loadedDeck.deckNotes)
        {
            DisplayDeckNote(fn);
        }

        // Final Check
        bool anyInvalidCards = false;
        for(int i = 0, count = loadedDeck.deckCards.Count; i < count; i++)
        {
            CardInDeck cid = loadedDeck.deckCards[i];
            if(cid.notes.Exists((e) => e.severity == NoteType.INVALID))
            {
                anyInvalidCards = true;
                break;
            }
        }
        bool deckErrors = loadedDeck.deckNotes.Exists((e) => e.severity == NoteType.INVALID);

        if(anyInvalidCards)
        {
            DisplayDeckNote(new FormatedNote(NoteType.INVALID, "Cards in this deck are not valid."));
        }
        else
        {
            DisplayDeckNote(new FormatedNote(NoteType.NOTE, "All cards are valid."));
        }
        if (deckErrors || anyInvalidCards)
        {
            DisplayDeckNote(new FormatedNote(NoteType.INVALID, "Deck list not valid."));
        }
        else
        {
            DisplayDeckNote(new FormatedNote(NoteType.SUCCESS, "Deck list validated."));
        }

        topLevelUI.SetActive(true);
    }

    private void DisplayDeckNote(FormatedNote fn)
    {
        GameObject newText = GameObject.Instantiate(notePrefab);
        newText.transform.SetParent(deckNoteRoots, false);
        newText.transform.localScale = Vector3.one;
        if (newText.TryGetComponent<Text>(out Text actualText))
        {
            actualText.text = fn.text;
            actualText.color = MapIssueToColor(fn.severity);
        }
    }

    private Color MapIssueToColor(NoteType nt)
    {
        switch (nt)
        {
            case NoteType.NOTE:
                return Color.white;
            case NoteType.ERROR:
                return Color.yellow;
            case NoteType.INVALID:
                return Color.red;
            case NoteType.SUCCESS:
                return Color.green;
            default:
                return Color.red;
        }
    }

    private void BackButton()
    {
        for(int i = 0, count = deckNoteRoots.childCount; i < count; i++)
        {
            Transform current = deckNoteRoots.GetChild(i);
            Destroy(current.gameObject);
        }
        for(int i = 0, count = cardRoot.childCount; i < count; i++)
        {
            Transform current = cardRoot.GetChild(i);
            Destroy(current.gameObject);
        }
        topLevelUI.SetActive(false);
        decklist.text = "";
    }
}
