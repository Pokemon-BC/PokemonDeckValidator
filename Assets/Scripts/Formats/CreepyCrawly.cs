using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PKMN;
using PKMN.Cards;

public class CreepyCrawly : PokemonFormat
{
    protected override bool RequireStandardLegal { get => true; }

    private List<string> bannedAttacks = new List<string>() { "Matron's Anger" };
    protected override List<string> BannedAttacks { get => bannedAttacks; }

    private List<ShortCard> bannedCards = new List<ShortCard>() { new ShortCard("RCL", "116") };
    protected override List<ShortCard> FormatBanList { get => bannedCards; }

    protected override void CustomFormatRules(PokemonDeck deck)
    {
        for (int i = 0, count = deck.DeckCards.Count; i < count; i++)
        {
            CardInDeck cid = deck.DeckCards[i];
            PokemonCard current = cid.Reference;

            if (current.Supertype == CardSupertype.POKEMON)
            {
                // Apply multi-prizer rule
                if (Array.Exists(current.Subtypes, (s) => s == CardSubtype.V || s == CardSubtype.V_UNION ||
                                 s == CardSubtype.VMAX || s == CardSubtype.VSTAR))
                {
                    cid.AddNote(NoteType.INVALID, "Multi-prizer Pokemon are not allowed in this format.");
                }
                // Apply type rule including resolving radiants
                string effectiveName = current.Name;
                if (effectiveName.Contains("Radiant"))
                {
                    effectiveName = effectiveName.Split(" ")[1];
                }

                VGPokemon currentDex = VideoGameDatabase.LookupPokemon(effectiveName);
                if (currentDex != null)
                {
                    if (currentDex.Type1 == "Bug" || currentDex.Type1 == "Ghost" ||
                        currentDex.Type2 == "Bug" || currentDex.Type2 == "Ghost")
                    {
                        // Bug or Ghost Type
                    }
                    else
                    {
                        cid.AddNote(NoteType.INVALID, "Pokemon must be Bug or Ghost Type");
                    }
                }
                else
                {
                    cid.AddNote(NoteType.ERROR, "Pokedex Lookup Failed, please check manually");
                }
            }
        }
    }
}
