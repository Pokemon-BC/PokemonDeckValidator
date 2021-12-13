using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMethodology : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TextAsset deckTest = Resources.Load<TextAsset>("Testing/TestDeck2");
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
        foreach(PokemonCard pc in uprising.GetInvalidCards())
        {
            Debug.Log("Invalid card " + pc.name);
        }
        foreach(string clause in uprising.GetInvalidClauses())
        {
            Debug.Log("Invalid clause: " + clause);
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
                if(cid.reference.supertype == "Pokémon" && cid.reference.evolvesTo == null)
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
                if(reference.supertype == "Pokémon")
                {
                    if(cid.quantity > 4)
                    {
                        invalidCards.Add(reference);
                    }
                    else if(reference.evolvesTo == null || reference.evolvesTo.Length == 0)
                    {
                        invalidCards.Add(reference);
                    }
                }
            }
        }

        return invalidCards.Count == 0 && invalidClauses.Count == 0;
    }
}
