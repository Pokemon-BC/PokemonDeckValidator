using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PKMN.Cards
{
    public static class CardExtensions
    {
        public static bool HasRulebox(this PokemonCard card)
        {
            if (card.Supertype == CardSupertype.POKEMON)
            {
                if (Array.Exists(card.Subtypes, SubtypeIsRulebox))
                {
                    return true;
                }
                else if (Array.Exists(card.Rules, RuleTextIsRulebox))
                {
                    return true;
                }
            }
            else if (card.Supertype == CardSupertype.TRAINER)
            {
                if (card.Rules.Length != 0)
                {
                    if (Array.Exists(card.Rules, RuleTextIsRulebox))
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else if (card.Supertype == CardSupertype.ENERGY)
            {
                if (card.Rules.Length != 0)
                {
                    if (Array.Exists(card.Rules, RuleTextIsRulebox))
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }

            return false;

            bool SubtypeIsRulebox(CardSubtype subtype)
            {
                return subtype == CardSubtype.EX || subtype == CardSubtype.MEGA || subtype == CardSubtype.BREAK ||
                       subtype == CardSubtype.GX || subtype == CardSubtype.V || subtype == CardSubtype.VMAX ||
                       subtype == CardSubtype.V_UNION || subtype == CardSubtype.VSTAR;
            }

            bool RuleTextIsRulebox(string rule)
            {
                return rule.Contains("ACE SPEC") || rule.Contains("(Prism Star)") || rule.Contains("Radiant Pokémon");
            }
        }

        public static bool IsBasicEnergy(this PokemonCard card)
        {
            if (card.Supertype == CardSupertype.ENERGY && Array.Exists(card.Subtypes, (e) => e == CardSubtype.BASIC))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
