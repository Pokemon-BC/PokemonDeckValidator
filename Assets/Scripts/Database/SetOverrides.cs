using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PKMN.Cards
{
    public class ShiningFatesOverride : LoadedSet
    {
        public ShiningFatesOverride(string id, string ptcgo, PokemonSet data, PokemonCard[] cards) : base(id, ptcgo, data, cards)
        {
            PokemonCard[] shinyVault = PokemonLoader.LoadCardsInSet("swsh45sv");
            for(int i = 0, count = shinyVault.Length; i < count; i++)
            {
                PokemonCard current = shinyVault[i];
                setCards.Add((CardHelper.ExtractNumber(current.Number) + 73).ToString(), current);
            }
        }
    }

    public class BrilliantStarsOverride : LoadedSet
    {
        public BrilliantStarsOverride(string id, string ptcgo, PokemonSet data, PokemonCard[] cards) : base(id, ptcgo, data, cards)
        {
            PokemonCard[] trainerGallery = PokemonLoader.LoadCardsInSet("swsh9tg");
            for(int i = 0, count = trainerGallery.Length; i < count; i++)
            {
                PokemonCard current = trainerGallery[i];
                setCards.Add((CardHelper.ExtractNumber(current.Number) + 186).ToString(), current);
            }
        }
    }
}
