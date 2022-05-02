using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using PKMN.Cards;

public class GCSpringFling : PokemonFormat
{
    protected override bool RequireExpandedLegal { get => true; }
    protected override List<ShortCard> FormatBanList { get => bannedCards; }
    private List<ShortCard> bannedCards = new List<ShortCard>()
    {
        new ShortCard("CEC", "208"),
        new ShortCard("DAA", "162")
    };

    protected override void CustomFormatRules(PokemonDeck deck)
    {
        // If we encounter a dual type card before the type is set it's an issue so go back to them afterwards.
        List<CardInDeck> callbacks = new List<CardInDeck>();

        for (int i = 0, count = deck.DeckCards.Count; i < count; i++)
        {
            CardInDeck current = deck.DeckCards[i];
            if (current.Reference.Supertype == CardSupertype.POKEMON)
            {
                if (current.Reference.HasRulebox())
                {
                    current.AddNote(NoteType.INVALID, "Cards with Ruleboxes are not allowed");
                }
                if (!PokemonContainsCoinFlip(current.Reference))
                {
                    current.AddNote(NoteType.INVALID, "Cards without coin flips are not allowed");
                }
            }
            else if (current.Reference.Supertype == CardSupertype.TRAINER)
            {
                if (!TrainerContainsCoinFlip(current.Reference))
                {
                    current.AddNote(NoteType.INVALID, "Cards without coin flips are not allowed");
                }
            }
            else if (current.Reference.Supertype == CardSupertype.ENERGY)
            {
                // Pass
            }
            else
            {
                // Pass
            }
        }
    }

    private bool PokemonContainsCoinFlip(PokemonCard card)
    {
        if (card.Attacks.Length != 0)
        {
            for (int i = 0, count = card.Attacks.Length; i < count; i++)
            {
                PokemonAttack current = card.Attacks[i];
                if (Regex.Match(current.Text.ToLower(), @"flip\s(a|\d)\scoins?").Success)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool TrainerContainsCoinFlip(PokemonCard card)
    {
        // I am not writing a custom regex just to accomidate one card.
        if (card.Name == "Blunder Policy" || card.Name == "Ilima" || card.Name == "Battle City")
        {
            return true;
        }
        if (card.Rules.Length != 0)
        {
            for (int i = 0, count = card.Rules.Length; i < count; i++)
            {
                string current = card.Rules[i];
                if (Regex.Match(current.ToLower(), @"flip\s(a|\d)\scoins?").Success)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
