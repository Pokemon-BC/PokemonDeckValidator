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
                if (Array.Exists(card.Subtypes, SubtypeIsRulebox
            ))
                {
                    return true;
                }
            }
            else if (card.Supertype == CardSupertype.TRAINER)
            {
                if (card.Rules != null)
                {
                    foreach (string s in card.Rules)
                    {
                        if (s.Contains("ACE SPEC") || s.Contains("Prism Star"))
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
            else if (card.Supertype == CardSupertype.ENERGY)
            {
                if (card.Rules != null)
                {
                    foreach (string s in card.Rules)
                    {
                        if (s.Contains("Prism Star"))
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

            bool SubtypeIsRulebox(CardSubtype subtype)
            {
                return subtype == CardSubtype.EX || subtype == CardSubtype.MEGA || subtype == CardSubtype.BREAK ||
                       subtype == CardSubtype.GX || subtype == CardSubtype.V || subtype == CardSubtype.VMAX ||
                       subtype == CardSubtype.V_UNION || subtype == CardSubtype.VSTAR;
            }
        }
    }
}
