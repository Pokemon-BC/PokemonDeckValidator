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

            public static PokemonSet[] LoadAddonSets()
            {
                TextAsset addonSetJson = Resources.Load<TextAsset>("pokemon-tcg-data-addon/sets/en");
                return JSonUtils.FromJson<PokemonSet>(JSonUtils.FixJson(addonSetJson.text));
            }

            public static PokemonCard[] LoadCardsInSet(string setId)
            {
                TextAsset setJson = Resources.Load<TextAsset>("pokemon-tcg-data/cards/en/" + setId);
                return JSonUtils.FromJson<PokemonCard>(JSonUtils.FixJson(setJson.text));
            }

            public static PokemonCard[] LoadCardsInAddonSet(string setId)
            {
                TextAsset setJson = Resources.Load<TextAsset>("pokemon-tcg-data-addon/cards/en/" + setId);
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
                        string collIdRaw = subsets[subsets.Length - 1];
                        string collId = SanetizeString(collIdRaw);
                        string setIdRaw = subsets[subsets.Length - 2];
                        string setId = SanetizeString(setIdRaw);
                        string name = "";
                        for (int subi = 2, subcount = subsets.Length - 2; subi < subcount; subi++)
                        {
                            name += " " + subsets[subi];
                        }
                        PokemonCard cardRef = CardDatabase.LookupCard(setId, collId);
                        if (cardRef == null)
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

            private static string SanetizeString(string raw)
            {
                //string collId = new string(collIdRaw.Where(c => !char.IsControl(c)).ToArray());
                List<char> newString = new List<char>();
                for (int i = 0, count = raw.Length; i < count; i++)
                {
                    char current = raw[i];
                    if (!char.IsControl(current))
                    {
                        newString.Add(current);
                    }
                }
                return new string(newString.ToArray());
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
                    if (current.PtcgoCode == "")
                    {
                        key = current.ID;
                    }
                    if (current.Legalities.IsStandardLegal || current.Legalities.IsExpandedLegal)
                    {
                        if (tcgoIdToSet.ContainsKey(key))
                        {
                            // Shining Fates Shiny Vault Override
                            if (key == "SHF")
                            {
                                Debug.Log("Handling Shining Fates as a unique scenario.");
                                PokemonSet inList = tcgoIdToSet[key];
                                if (inList.ID == "swsh45sv" && current.ID == "swsh45")
                                {
                                    tcgoIdToSet[key] = current;
                                }
                            }
                            // End Shining Fates Compatability Code.
                            // Begin Brilliant Stars Trainer Gallery Compatability Code
                            else if (key == "BRS")
                            {
                                Debug.Log("Handling Brilliant Stars as a unique scenario.");
                                PokemonSet inList = tcgoIdToSet[key];
                                if (inList.ID == "swsh9tg" && current.ID == "swsh9")
                                {
                                    tcgoIdToSet[key] = current;
                                }
                            }
                            // End Brilliant Stars Compatability Code
                            // Begin Astral Radiance Compatibility Code
                            else if (key == "ASR")
                            {
                                Debug.Log("Handling Astral Radiance as a unique scenario.");
                                PokemonSet inList = tcgoIdToSet[key];
                                if (inList.ID == "swsh10tg" && current.ID == "swsh10")
                                {
                                    tcgoIdToSet[key] = current;
                                }
                            }
                            // End Astral Radiance Compatability Code
                            else
                            {
                                Debug.LogWarning("Conflict between new set " + current.Name + " and old set " + tcgoIdToSet[key].Name);
                            }
                        }
                        else
                        {
                            tcgoIdToSet.Add(key, current);
                        }
                    }
                }

                // Unlike the normal sets where we don't want to load everything unless we need it, the addon sets can all be loaded immediately.
                LoadAddonSets();
            }

            public static PokemonCard LookupCard(string ptcgoId, string collId)
            {
                if (!initialized) Init();
                // TCGO Handles promos differently, handled here:
                if (ptcgoId == "PR-BLW")
                {
                    collId = "BW" + collId;
                }
                else if (ptcgoId == "PR-SM")
                {
                    collId = "SM" + collId;
                }
                else if (ptcgoId == "PR-SW")
                {
                    collId = "SWSH" + collId;
                }
                // End Promo specific code
                if (tcgoIdToSet.TryGetValue(ptcgoId, out PokemonSet pokemonSet))
                {
                    LoadedSet ls;
                    if (internalIdToSet.ContainsKey(pokemonSet.ID))
                    {
                        ls = internalIdToSet[pokemonSet.ID];
                    }
                    else
                    {
                        PokemonCard[] cards = PokemonLoader.LoadCardsInSet(pokemonSet.ID);
                        // Manual override for Shining Fates Shiny Vault
                        if (pokemonSet.ID == "swsh45")
                        {
                            internalIdToSet.Add(pokemonSet.ID, new ShiningFatesOverride(pokemonSet.ID, ptcgoId, pokemonSet, cards));
                        }
                        // Manual override for Brilliant Stars Trainer Gallery
                        else if (pokemonSet.ID == "swsh9")
                        {
                            internalIdToSet.Add(pokemonSet.ID, new BrilliantStarsOverride(pokemonSet.ID, ptcgoId, pokemonSet, cards));
                        }
                        else
                        {
                            internalIdToSet.Add(pokemonSet.ID, new LoadedSet(pokemonSet.ID, ptcgoId, pokemonSet, cards));
                        }
                        ls = internalIdToSet[pokemonSet.ID];
                    }
                    if (ls.setCards.TryGetValue(collId, out PokemonCard found))
                    {
                        return found;
                    }
                    else
                    {
                        Debug.LogWarning("Card not found, " + ptcgoId + "-" + collId);
                        return null;
                    }
                }
                else
                {
                    Debug.LogWarning("Card from problematic set " + ptcgoId + " with col ID " + collId);
                    return null;
                }
            }

            public static void LoadAddonSets()
            {
                PokemonSet[] addonSets = PokemonLoader.LoadAddonSets();
                for (int i = 0, count = addonSets.Length; i < count; i++)
                {
                    PokemonSet newset = addonSets[i];
                    string key = newset.PtcgoCode;
                    if (newset.PtcgoCode == "")
                    {
                        key = newset.ID;
                    }
                    if (newset.Legalities.IsStandardLegal || newset.Legalities.IsExpandedLegal)
                    {
                        if (tcgoIdToSet.ContainsKey(key))
                        {
                            Debug.LogWarning("Conflict between new set " + newset.Name + " and old set " + tcgoIdToSet[key].Name);
                        }
                        else
                        {
                            tcgoIdToSet.Add(key, newset);
                        }
                    }

                    PokemonCard[] cards = PokemonLoader.LoadCardsInAddonSet(newset.ID);
                    internalIdToSet.Add(newset.ID, new LoadedSet(newset.ID, newset.PtcgoCode, newset, cards));
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
                for (int i = 0, count = cards.Length; i < count; i++)
                {
                    PokemonCard current = cards[i];
                    if (setCards.ContainsKey(current.Number))
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
            UNKNOWN, STAGE_1, STAGE_2, EX, MEGA, BREAK, RESTORED, GX, V, VMAX, V_UNION, VSTAR, RADIANT,
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
