using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

using PKMN.Cards;

public class GCNewStart : PokemonFormat
{
    protected override bool RequireExpandedLegal => true;
    private List<ShortCard> banList = new List<ShortCard>()
    {
        new ShortCard("SSH", 58), // Inteleon (Shady Dealings)
        new ShortCard("CRE", 43) // Inteleon (Quick Shooting)
    };
    protected override List<ShortCard> FormatBanList { get => banList; }

    private List<string> starters = new List<string>(){"Pikachu", "Bulbasaur", "Charmander", "Squirtle", "Eevee",
                                                       "Chikorita", "Cyndaquil", "Totodile",
                                                       "Treecko", "Torchic", "Mudkip",
                                                       "Turtwig", "Chimchar", "Piplup",
                                                       "Sinvy", "Tepig", "Oshawott",
                                                       "Chespin", "Fennekin", "Froakie",
                                                       "Rowlet", "Litten", "Popplio",
                                                       "Grookey", "Scorbunny", "Sobble"};

    protected override void CustomFormatRules(PokemonDeck deck)
    {
        int numberOfStarters = 0;
        List<CardInDeck> starters = new List<CardInDeck>();
        Dictionary<string, int> vunionCount = new Dictionary<string, int>();
        List<CardInDeck> vUnions = new List<CardInDeck>();

        for(int i = 0, count = deck.DeckCards.Count; i < count; i++)
        {
            CardInDeck current = deck.DeckCards[i];
            if(IsStarter(current))
            {
                if(Array.Exists(current.Reference.Subtypes, (e) => e == CardSubtype.V_UNION))
                {
                    vUnions.Add(current);
                    if(vunionCount.ContainsKey(current.Reference.Name))
                    {
                        vunionCount[current.Reference.Name]++;
                    }
                    else
                    {
                        vunionCount.Add(current.Reference.Name, 1);
                    }

                    if(vunionCount[current.Reference.Name] == 4)
                    {
                        numberOfStarters++;
                        for(int j = 0, vunionCnt = vUnions.Count; j < vunionCnt; j++)
                        {
                            CardInDeck vUnionNow = vUnions[j];
                            if(vUnionNow.Reference.Name == current.Reference.Name)
                            {
                                starters.Add(vUnionNow);
                                vUnionNow.AddNote(NoteType.NOTE, "V-Union Pokemon only count as 1 starter.");
                            }
                        }
                    }
                }
                else
                {
                    starters.Add(current);
                    numberOfStarters += current.Quantity;
                }
            }
        }

        FormatedNote starterNote;
        if(numberOfStarters >= 6)
        {
            starterNote = new FormatedNote(NoteType.NOTE, "Valid Starter Pokemon");
            deck.AddNote(NoteType.NOTE, "There are " + numberOfStarters + " starter Pokemon in this deck.");
        }
        else
        {
            starterNote = new FormatedNote(NoteType.INVALID, "Not Enough Starter Pokemon");
            deck.AddNote(NoteType.INVALID, "There are " + numberOfStarters + " starter Pokemon in this deck which does not meet the required 6.");
        }

        for(int i = 0, count = starters.Count; i < count; i++)
        {
            starters[i].AddNote(starterNote.severity, starterNote.text);
        }
    }

    private bool IsStarter(CardInDeck cid)
    {
        if(starters.Exists((e) => cid.Reference.Name.Contains(e)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
