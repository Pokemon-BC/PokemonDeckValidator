using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using PKMN.Cards;

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
        //UprisingFormat tempFormat = new UprisingFormat();
        StandardFormat tempFormat = new StandardFormat();
        Debug.Log("Text is " + decklist.text);
        PokemonDeck loadedDeck = PokemonLoader.LoadDeck(decklist.text);
        tempFormat.CheckDeckInFormat(loadedDeck);
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
        topLevelUI.SetActive(true);
    }
}
