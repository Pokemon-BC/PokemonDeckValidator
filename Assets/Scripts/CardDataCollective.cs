using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDataCollective
{
    //public List<LoadedSet> allSets;
    public static Dictionary<string, PokemonSet> tcgoIdToSet;
    public static Dictionary<string, LoadedSet> internalIdToSet;

    private static bool initialized = false;
    public static void Init()
    {
        if(!initialized)
        {
            tcgoIdToSet = new Dictionary<string, PokemonSet>();
            internalIdToSet = new Dictionary<string, LoadedSet>();
            initialized = true;
            PokemonSet[] sets = SetLoader.LoadAllSets();
            for(int i = 0, count = sets.Length; i < count; i++)
            {
                string key = sets[i].ptcgoCode;
                if(sets[i].ptcgoCode == null)
                {
                    key = sets[i].id;
                }
                if(tcgoIdToSet.ContainsKey(key))
                {
                    Debug.Log("Conflict between new set " + sets[i].name + " and old set " + tcgoIdToSet[key].name);
                }
                else
                {
                    tcgoIdToSet.Add(key, sets[i]);
                }
            }
        }
    }

    public static PokemonCard LookupCard(string ptcgoId, int collId)
    {
        if (!initialized) Init();
        if(tcgoIdToSet.TryGetValue(ptcgoId, out PokemonSet pokemonSet))
        {
            LoadedSet ls;
            if(internalIdToSet.ContainsKey(pokemonSet.id))
            {
                ls = internalIdToSet[pokemonSet.id];
            }
            else
            {
                PokemonCard[] cards = CardLoader.LoadCardsInSet(pokemonSet.id);
                internalIdToSet.Add(pokemonSet.id, new LoadedSet(pokemonSet.id, ptcgoId, pokemonSet, cards));
                ls = internalIdToSet[pokemonSet.id];
            }
            return ls.setCards[collId - 1]; // -1 because arrays
        }
        else
        {
            Debug.Log("Card from problematic set " + ptcgoId + " with col ID " + collId);
            return null;
        }
    }

    // TODO take input string deck
    public static PokemonDeck LoadDeck(string decklist)
    {
        if (!initialized) Init();
        PokemonDeck result = new PokemonDeck();
        result.deckCards = new List<CardInDeck>();

        string[] lines = decklist.Split('\n');
        for (int i = 0, count = lines.Length; i < count; i++)
        {
            string line = lines[i];
            if (line.StartsWith("******"))
            {
                // Skip
            }
            else if (line.StartsWith("##"))
            {
                // Quantity Count
                string[] subsets = line.Split(' ');
                string identifier = subsets[0];
                if (identifier.Contains("Pokémon"))
                {
                    //Debug.Log("Pokemon Count is " + subsets[2]);
                    result.pokemon = int.Parse(subsets[2]);
                }
                else if (identifier.Contains("Trainer"))
                {
                    //Debug.Log("Trainer count is " + subsets[3]);
                    result.trainers = int.Parse(subsets[3]);
                }
                else if (identifier.Contains("Energy"))
                {
                    //Debug.Log("Energy count is " + subsets[2]);
                    result.energies = int.Parse(subsets[2]);
                }
            }
            else if (line.StartsWith("* "))
            {
                // Playable Card
                string[] subsets = line.Split(' ');
                int quantity = int.Parse(subsets[1]);
                int collId = int.Parse(subsets[subsets.Length - 1]);
                string setId = subsets[subsets.Length - 2];
                string name = "";
                for (int subi = 2, subcount = subsets.Length - 2; subi < subcount; subi++)
                {
                    name = name + subsets[subi];
                }

                //Debug.Log("Identified Card Called: " + name + ", qty: " + quantity + ", Set: " + setId + ", Collectors ID: " + collId);
                result.deckCards.Add(new CardInDeck(LookupCard(setId, collId), quantity));
            }
            else if (line.StartsWith("Total Cards -"))
            {
                // Final Card Count
                string[] subsets = line.Split(' ');
                //Debug.Log("Total Cards In Deck " + subsets[3]);
                result.totalCards = int.Parse(subsets[3]);
            }
        }
        return result;
    }
}

[System.Serializable]
public class LoadedSet
{
    public LoadedSet(string id, string ptcgo, PokemonSet data, PokemonCard[] cards)
    {
        setId = id;
        ptcgoId = ptcgo;
        setData = data;
        setCards = cards;
    }
    public string ptcgoId;
    public string setId;
    public PokemonSet setData;
    public PokemonCard[] setCards;
}
