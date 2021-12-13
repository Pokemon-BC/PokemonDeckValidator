using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckListValidator : MonoBehaviour
{
    public Button submitButton;
    public Text decklist;
    public GameObject topLevelUI;
    public Transform cardRoot;

    public GameObject cardPrefab;

    // Start is called before the first frame update
    void Start()
    {
        topLevelUI.SetActive(false);
        submitButton.onClick.AddListener(SubmitButton);
    }

    private void SubmitButton()
    {
        Debug.Log("Submit button pressed");
        UprisingFormat tempFormat = new UprisingFormat();
        Debug.Log("Text is " + decklist.text);
        PokemonDeck loadedDeck = CardDataCollective.LoadDeck(decklist.text);
        List<CardReported> reports = tempFormat.processDeck(loadedDeck);
        Debug.Log("Deck loaded with " + loadedDeck.totalCards + " cards and " + reports.Count + " reports");
        foreach(CardReported report in reports)
        {
            if(report.start == null)
            {
                continue;
            }
            GameObject createdCard = GameObject.Instantiate(cardPrefab);
            ReportedPokemonCard rpc = createdCard.GetComponent<ReportedPokemonCard>();
            createdCard.transform.SetParent(cardRoot);
            
            rpc.Configure(report);
        }
        topLevelUI.SetActive(true);
    }
}
