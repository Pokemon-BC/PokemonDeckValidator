using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PKMN.Cards;

public class TestMethodology : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        /*TextAsset deckTest = Resources.Load<TextAsset>("Testing/TestDeck2");
        PokemonDeck loadedDeck = CardDataCollective.LoadDeck(deckTest.text);
        Debug.Log("Loaded deck with " + loadedDeck.totalCards + " cards " + loadedDeck.pokemon + " pokemon " + loadedDeck.trainers + " trainers " + loadedDeck.energies + " energies ");
        Debug.Log("Deck cards are ");
        foreach(CardInDeck cid in loadedDeck.deckCards)
        {
            if(cid.reference != null)
            {
                Debug.Log(cid.quantity + "x " + cid.reference.name);
            }
            else
            {
                Debug.Log(cid.quantity + "x Cards without a valid set");
            }
        }

        PokemonFormat uprising = new UprisingFormat();
        Debug.Log("Does list pass uprising " + uprising.IsDeckValid(loadedDeck));
        /*foreach(PokemonCard pc in uprising.GetInvalidCards())
        {
            Debug.Log("Invalid card " + pc.name);
        }
        foreach(string clause in uprising.GetInvalidClauses())
        {
            Debug.Log("Invalid clause: " + clause);
        }*/

        PokemonSet[] sets = PKMN.Cards.PokemonLoader.LoadAllSets();
        foreach(PokemonSet set in sets)
        {
            if(set.Legalities.Standard ||set.Legalities.Expanded)
            {
                PokemonCard[] cards = PKMN.Cards.PokemonLoader.LoadCardsInSet(set.ID);
                for(int i = 0, count = cards.Length; i < count; i++)
                {
                    Debug.Log("Card is valid: " + (cards[i].Name != null));
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public class UprisingFormat : PokemonFormat
{
    public List<PokemonCard> invalidCards;
    public List<string> invalidClauses;

    public UprisingFormat()
    {
        invalidCards = new List<PokemonCard>();
        invalidClauses = new List<string>();
    }

    public override List<CardReported> processDeck(PokemonDeck deckToProcess)
    {
        List<CardReported> cr = new List<CardReported>();

        foreach(CardInDeck cid in deckToProcess.deckCards)
        {
            var card = new CardReported(cid, new List<string>());
            if(cid.reference != null)
            {
                if(cid.reference.Supertype == CardSupertype.POKEMON && cid.reference.EvolvesTo == null)
                {
                    card.issues.Add("Does not Evolve");
                }
            }
            cr.Add(card);
        }

        return cr;
    }

    public override List<PokemonCard> GetInvalidCards()
    {
        return invalidCards;
    }

    public override List<string> GetInvalidClauses()
    {
        return invalidClauses;
    }

    public override bool IsDeckValid(PokemonDeck deckToCheck)
    {
        if (deckToCheck.totalCards != 60) return false;
        foreach(CardInDeck cid in deckToCheck.deckCards)
        {
            PokemonCard reference = cid.reference;
            if(reference == null)
            {
                Debug.Log("Skipping unmapped card");
            }
            else
            {
                if(reference.Supertype == CardSupertype.POKEMON)
                {
                    if(cid.quantity > 4)
                    {
                        invalidCards.Add(reference);
                    }
                    else if(reference.EvolvesTo == null || reference.EvolvesTo.Length == 0)
                    {
                        invalidCards.Add(reference);
                    }
                }
            }
        }

        return invalidCards.Count == 0 && invalidClauses.Count == 0;
    }
}
