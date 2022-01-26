using System;
using System.Collections.Generic;

using PKMN.Cards;

public class GCExpandedLimited : PokemonFormat
{
    protected override bool RequireExpandedLegal => true;

    public string[] bannedAttacks = new string[]{"Lost March", "Night March", "Mad Party", "Let's All Rollout"};

    protected override void CustomFormatRules(PokemonDeck deck)
    {
        for(int i = 0, count = deck.DeckCards.Count; i < count; i++)
        {
            CardInDeck current = deck.DeckCards[i];
            if(current.Reference.Supertype != CardSupertype.UNKNOWN)
            {
                if(CardHasRulebox(current))
                {
                    current.AddNote(NoteType.INVALID, "Cards with Rule Boxes are not allowed.");
                }
                if(current.Reference.Supertype == CardSupertype.POKEMON && CardHasBannedAttack(current))
                {
                    current.AddNote(NoteType.INVALID, "This card has a banned attack.");
                }
            }
        }
    }

    private bool CardHasRulebox(CardInDeck card)
    {
        if(card.Reference.Supertype == CardSupertype.POKEMON)
        {
            if(Array.Exists(card.Reference.Subtypes, (e) => {
                return e == CardSubtype.EX || e == CardSubtype.MEGA || e == CardSubtype.BREAK || e == CardSubtype.GX || e == CardSubtype.V || e == CardSubtype.VMAX || e == CardSubtype.V_UNION;
            }))
            {
                return true;
            }
        }
        else if(card.Reference.Supertype == CardSupertype.TRAINER)
        {
            if(card.Reference.Rules != null)
            {
                foreach(string s in card.Reference.Rules)
                {
                    if(s.Contains("ACE SPEC") || s.Contains("Prism Star"))
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        else if(card.Reference.Supertype == CardSupertype.ENERGY)
        {
            if(card.Reference.Rules != null)
            {
                foreach(string s in card.Reference.Rules)
                {
                    if(s.Contains("Prism Star"))
                    {
                        return true;
                    }
                }
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    private bool CardHasBannedAttack(CardInDeck card)
    {
        PokemonCard reference = card.Reference;
        PokemonAttack[] attacks = reference.Attacks;
        for(int i = 0, count = attacks.Length; i < count; i++)
        {
            if(Array.Exists(bannedAttacks, (e) => 
            {
                return e == attacks[i].Name;
            }))
            {
                return true;
            }
        }
        return false;
    }
}
