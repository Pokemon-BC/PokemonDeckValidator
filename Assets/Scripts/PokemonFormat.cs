using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PKMN.Cards;
public abstract class PokemonFormat
{
    public abstract List<CardReported> processDeck(PokemonDeck deckToProcess);
    public abstract bool IsDeckValid(PokemonDeck deckToCheck);
    public abstract List<PokemonCard> GetInvalidCards();
    public abstract List<string> GetInvalidClauses();
}

public class PokemonDeck
{
    public List<CardInDeck> deckCards;
    public int pokemon, trainers, energies;
    public int totalCards;
}

public class CardInDeck
{
    public PokemonCard reference;
    public int quantity;

    public CardInDeck(PokemonCard pokemonCard, int quantity)
    {
        this.reference = pokemonCard;
        this.quantity = quantity;
    }
}

public class CardReported
{
    public CardInDeck start;
    public List<string> issues;

    public CardReported(CardInDeck cid, List<string> errs)
    {
        start = cid;
        issues = errs;
    }
}
