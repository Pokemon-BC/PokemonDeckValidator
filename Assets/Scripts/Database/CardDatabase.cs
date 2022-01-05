using System.Collections.Generic;
using UnityEngine;

using System;

namespace PKMN
{
    namespace Cards
    {
        public static class PokemonLoader
        {
            public static PokemonSet[] LoadAllSets()
            {
                TextAsset setJson = Resources.Load<TextAsset>("pokemon-tcg-data/sets/en");
                return JSonUtils.FromJson<PokemonSet>(JSonUtils.FixJson(setJson.text));
            }

            public static PokemonCard[] LoadCardsInSet(string setId)
            {
                TextAsset setJson = Resources.Load<TextAsset>("pokemon-tcg-data/cards/en/" + setId);
                return JSonUtils.FromJson<PokemonCard>(JSonUtils.FixJson(setJson.text));
            }

            public static PokemonDeck LoadDeck(string decklist)
            {
                if (!CardDatabase.Initialized) CardDatabase.Init();
                PokemonDeck result = new PokemonDeck();

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
                        // Quantity Count for card type
                        string[] subsets = line.Split(' ');
                        string identifier = subsets[0];
                        if (identifier.Contains("Pokémon"))
                        {
                            result.Pokemon = int.Parse(subsets[2]);
                        }
                        else if (identifier.Contains("Trainer"))
                        {
                            result.Trainers = int.Parse(subsets[3]);
                        }
                        else if (identifier.Contains("Energy"))
                        {
                            result.Energies = int.Parse(subsets[2]);
                        }
                    }
                    else if (line.StartsWith("* "))
                    {
                        string[] subsets = line.Split(' ');
                        int quantity = int.Parse(subsets[1]);
                        string collId = subsets[subsets.Length - 1];
                        string setId = subsets[subsets.Length - 2];
                        string name = "";
                        for (int subi = 2, subcount = subsets.Length - 2; subi < subcount; subi++)
                        {
                            name += " " + subsets[subi];
                        }
                        Debug.Log("Looking up card in set " + setId + " with collID " + collId);
                        PokemonCard cardRef = CardDatabase.LookupCard(setId, collId);
                        if(cardRef == null)
                        {
                            cardRef = PokemonCard.GenerateErrorCard(name, collId);
                        }
                        result.DeckCards.Add(new CardInDeck(cardRef, setId, quantity));
                    }
                    else if (line.StartsWith("Total Cards -"))
                    {
                        // Final Card Count
                        string[] subsets = line.Split(' ');
                        result.TotalCards = int.Parse(subsets[3]);
                    }
                }
                return result;
            }
        }

        public static class CardDatabase
        {
            public static Dictionary<string, PokemonSet> tcgoIdToSet;
            public static Dictionary<string, LoadedSet> internalIdToSet;

            private static bool initialized = false;
            public static bool Initialized { get => initialized; }

            public static void Init()
            {
                tcgoIdToSet = new Dictionary<string, PokemonSet>();
                internalIdToSet = new Dictionary<string, LoadedSet>();
                initialized = true;
                PokemonSet[] sets = PokemonLoader.LoadAllSets();
                for (int i = 0, count = sets.Length; i < count; i++)
                {
                    PokemonSet current = sets[i];
                    string key = current.PtcgoCode;
                    if (current.PtcgoCode == null)
                    {
                        key = current.ID;
                    }
                    if (current.Legalities.IsStandardLegal || current.Legalities.IsExpandedLegal)
                    {
                        if (tcgoIdToSet.ContainsKey(key))
                        {
                            Debug.Log("Conflict between new set " + current.Name + " and old set " + tcgoIdToSet[key].Name);
                        }
                        else
                        {
                            tcgoIdToSet.Add(key, current);
                        }
                    }
                }
            }

            public static PokemonCard LookupCard(string ptcgoId, string collId)
            {
                if (!initialized) Init();
                if (tcgoIdToSet.TryGetValue(ptcgoId, out PokemonSet pokemonSet))
                {
                    LoadedSet ls;
                    Debug.Log("Looking up card in set " + ptcgoId + " with coll id " + collId);
                    if (internalIdToSet.ContainsKey(pokemonSet.ID))
                    {
                        Debug.Log("Key exists");
                        ls = internalIdToSet[pokemonSet.ID];
                    }
                    else
                    {
                        PokemonCard[] cards = PokemonLoader.LoadCardsInSet(pokemonSet.ID);
                        internalIdToSet.Add(pokemonSet.ID, new LoadedSet(pokemonSet.ID, ptcgoId, pokemonSet, cards));
                        ls = internalIdToSet[pokemonSet.ID];
                    }
                    if (ls.setCards.ContainsKey(collId))
                    {
                        return ls.setCards[collId];
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    Debug.Log("Card from problematic set " + ptcgoId + " with col ID " + collId);
                    return null;
                }
            }
        }

        public class LoadedSet
        {
            public string ptcgoId;
            public string setId;
            public PokemonSet setData;
            public Dictionary<string, PokemonCard> setCards;

            public LoadedSet(string id, string ptcgo, PokemonSet data, PokemonCard[] cards)
            {
                setId = id;
                ptcgoId = ptcgo;
                setData = data;
                setCards = new Dictionary<string, PokemonCard>();
                for(int i = 0, count = cards.Length; i < count; i++)
                {
                    PokemonCard current = cards[i];
                    if(setCards.ContainsKey(current.Number))
                    {
                        Debug.LogWarning("Conflict between cards " + setCards[current.Number].ToString() + " and \n " + current.ToString());
                    }
                    else
                    {
                        setCards.Add(current.Number, current);
                    }
                }
            }

            public override bool Equals(object obj)
            {
                return obj is LoadedSet set &&
                       ptcgoId == set.ptcgoId &&
                       setId == set.setId &&
                       EqualityComparer<PokemonSet>.Default.Equals(setData, set.setData) &&
                       EqualityComparer<Dictionary<string, PokemonCard>>.Default.Equals(setCards, set.setCards);
            }

            public override int GetHashCode()
            {
                int hashCode = 738309204;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ptcgoId);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(setId);
                hashCode = hashCode * -1521134295 + EqualityComparer<PokemonSet>.Default.GetHashCode(setData);
                return hashCode;
            }
        }

        public class PokemonDeck
        {
            protected List<CardInDeck> deckCards;
            public List<CardInDeck> DeckCards { get => deckCards; }
            public int Pokemon, Trainers, Energies;
            public int TotalCards;

            protected List<FormatedNote> deckNotes;
            public List<FormatedNote> DeckNotes { get => deckNotes; }

            public PokemonDeck()
            {
                deckCards = new List<CardInDeck>();
                deckNotes = new List<FormatedNote>();
            }

            public void AddNote(NoteType severity, string message)
            {
                deckNotes.Add(new FormatedNote(severity, message));
            }
        }

        public class CardInDeck : IEquatable<CardInDeck>
        {
            protected PokemonCard reference;
            public PokemonCard Reference { get => reference; }
            protected string setId;
            public string SetId { get => setId; }
            protected int quantity;
            public int Quantity { get => quantity; }
            protected List<FormatedNote> notes;
            public List<FormatedNote> Notes { get => notes; }

            public CardInDeck(PokemonCard pokemonCard, string setId, int quantity)
            {
                this.reference = pokemonCard;
                this.setId = setId;
                this.quantity = quantity;
                notes = new List<FormatedNote>();
            }

            public void AddNote(NoteType severity, string message)
            {
                notes.Add(new FormatedNote(severity, message));
            }

            public override bool Equals(object obj)
            {
                return obj is CardInDeck cid && Equals(cid);
            }

            public bool Equals(CardInDeck cid)
            {
                return reference.Equals(cid.reference) &&
                       setId.Equals(cid.setId) &&
                       quantity == cid.quantity &&
                       CardHelper.ArraySameElements(notes.ToArray(), cid.notes.ToArray());
            }

            public override int GetHashCode()
            {
                return reference.GetHashCode() * setId.GetHashCode() * quantity.GetHashCode();
            }

            public override string ToString()
            {
                return string.Format("(CardInDeck {0}x {1}-{2})", quantity, setId, reference.Number);
            }
        }

        public class FormatedNote
        {
            public NoteType severity;
            public string text;

            public FormatedNote(NoteType severity, string text)
            {
                this.severity = severity;
                this.text = text;
            }

            public override bool Equals(object obj)
            {
                return obj is FormatedNote note &&
                       severity == note.severity &&
                       text == note.text;
            }

            public override int GetHashCode()
            {
                return severity.GetHashCode() * text.GetHashCode();
            }

            public override string ToString()
            {
                return string.Format("(FormattedNote [{0}] {1}", severity, text);
            }
        }

        public enum NoteType
        {
            // Note is just info for the player.
            // Error means a C# error occured but the deck is not necessarly invalid. (Happens when a card lookup fails.)
            // Invalid means the deck is invalid.
            // Success is used to display a validated deck.
            NOTE, ERROR, INVALID, SUCCESS
        }

        public enum CardSupertype
        {
            UNKNOWN, POKEMON, TRAINER, ENERGY
        }

        public enum PokemonType
        {
            NONE, GRASS, FIRE, WATER, LIGHTNING, PSYCHIC, FIGHTING, DARKNESS, METAL, FAIRY, COLORLESS, DRAGON
        }

        public enum CardSubtype
        {
            // Pokemon
            UNKNOWN, STAGE_1, STAGE_2, EX, MEGA, BREAK, RESTORED, GX, V, VMAX, V_UNION,
            // Trainers
            ITEM, PKMN_TOOL, PKMN_TOOL_FLARE, SUPPORTER, STADIUM,
            // Energy
            SPECIAL,
            // Common
            BASIC, TAG_TEAM, RAPID_STRIKE, SINGLE_STRIKE, FUSION_STRIKE
        }

        public enum Legality
        {
            ILLEGAL, LEGAL, BANNED
        }
    }
}
