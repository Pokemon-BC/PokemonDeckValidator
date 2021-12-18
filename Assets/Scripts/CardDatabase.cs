using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PKMN
{
    namespace Cards
    {
        public class PokemonLoader
        {
            public static PokemonSet[] LoadAllSets()
            {
                TextAsset setJson = Resources.Load<TextAsset>("pokemon-tcg-data/sets/en");
                //Debug.Log(setJson);
                return JSonUtils.FromJson<PokemonSet>(JSonUtils.FixJson(setJson.text));
            }

            public static PokemonCard[] LoadCardsInSet(string setId)
            {
                TextAsset setJson = Resources.Load<TextAsset>("pokemon-tcg-data/cards/en/" + setId);
                //Debug.Log(setJson);
                return JSonUtils.FromJson<PokemonCard>(JSonUtils.FixJson(setJson.text));
            }
        }

        static class CardHelper
        {
            public static CardSupertype GetSupertype(PokemonCard card)
            {
                string supertype = card.supertype;
                if (supertype == "Pokémon")
                {
                    return CardSupertype.POKEMON;
                }
                else if (supertype == "Trainer")
                {
                    return CardSupertype.TRAINER;
                }
                else if (supertype == "Energy")
                {
                    return CardSupertype.ENERGY;
                }
                else
                {
                    return CardSupertype.UNKNOWN;
                }
            }
            
            private static Dictionary<string, PokemonType> typeMapping = new Dictionary<string, PokemonType>(){
                {"Grass", PokemonType.GRASS },
                {"Fire", PokemonType.FIRE},
                {"Water", PokemonType.WATER},
                {"Lightning", PokemonType.LIGHTNING},
                {"Psychic", PokemonType.PSYCHIC},
                {"Fighting", PokemonType.FIGHTING},
                {"Darkness", PokemonType.DARKNESS},
                {"Metal", PokemonType.METAL},
                {"Fairy", PokemonType.FAIRY},
                {"Colorless", PokemonType.COLORLESS},
                {"Dragon", PokemonType.DRAGON}
            };
            public static PokemonType[] GetTypes(string[] types)
            {
                PokemonType[] result = new PokemonType[types.Length];
                for(int i = 0, count = types.Length; i < count; i++)
                {
                    if(typeMapping.TryGetValue(types[i], out PokemonType value))
                    {
                        result[i] = value;
                    }
                    else
                    {
                        Debug.LogError("Problem mapping type " + types[i]);
                        result[i] = PokemonType.NONE;
                    }
                }
                return result;
            }

            public static PokemonType GetType(string type)
            {
                if (typeMapping.TryGetValue(type, out PokemonType value))
                {
                    return value;
                }
                else
                {
                    Debug.LogError("Problem mapping type " + type);
                    return PokemonType.NONE;
                }
            }

            private static Dictionary<string, CardSubtype> subtypeMap = new Dictionary<string, CardSubtype>()
            {
                //Pokemon
                {"Stage 1", CardSubtype.STAGE_1},
                {"Stage 2", CardSubtype.STAGE_2},
                {"Restored", CardSubtype.RESTORED},
                {"EX", CardSubtype.EX},
                {"MEGA", CardSubtype.MEGA},
                {"BREAK", CardSubtype.BREAK},
                {"GX", CardSubtype.GX},
                {"TAG TEAM", CardSubtype.TAG_TEAM},
                {"V", CardSubtype.V},
                {"VMAX", CardSubtype.VMAX},
                // Trainers
                {"Item", CardSubtype.ITEM},
                {"Pokémon Tool", CardSubtype.PKMN_TOOL},
                {"Pokémon Tool F", CardSubtype.PKMN_TOOL_FLARE},
                {"Supporter", CardSubtype.SUPPORTER},
                {"Stadium", CardSubtype.STADIUM},
                // Energy
                {"Special", CardSubtype.SPECIAL},
                // Common
                {"Basic", CardSubtype.BASIC},
                {"Single Strike", CardSubtype.SINGLE_STRIKE},
                {"Rapid Strike", CardSubtype.RAPID_STRIKE},
                {"Fusion Strike", CardSubtype.FUSION_STRIKE}
            };
            public static CardSubtype[] GetSubtypes(string[] subtypes)
            {
                CardSubtype[] result = new CardSubtype[subtypes.Length];
                for (int i = 0, count = subtypes.Length; i < count; i++)
                {
                    if (subtypeMap.TryGetValue(subtypes[i], out CardSubtype value))
                    {
                        result[i] = value;
                    }
                    else
                    {
                        Debug.LogError("Problem mapping type " + subtypes[i]);
                        result[i] = CardSubtype.UNKNOWN;
                    }
                }
                return result;
            }
        }
        
        [System.Serializable]
        public class PokemonSet
        {
            public string id;
            public string name;
            public string series;
            public int printedTotal;
            public int total;
            public Legalities legalities;
            public string ptcgoCode;
            public string releaseDate;
            public string updatedAt;
            public SetImages images;
        }

        [System.Serializable]
        public class Legalities
        {
            public string unlimited;
            public string standard;
            public string expanded;

            public bool Unlimited
            {
                get
                {
                    return unlimited != null && unlimited != "" && unlimited != "Banned";
                }
            }

            public bool Expanded
            {
                get
                {
                    return expanded != null && expanded != "" && expanded != "Banned";
                }
            }

            public bool Standard
            {
                get
                {
                    return standard != null && standard != "" && standard != "Banned";
                }
            }
        }

        [System.Serializable]
        public class SetImages
        {
            public string symbol;
            public string logo;
        }

        [System.Serializable]
        public class PokemonCard
        {
            public string id;
            public string name;
            public string supertype;
            public string[] subtypes;
            public string hp;
            public string[] types;
            public string evolvesFrom;
            public string[] evolvesTo;
            public string[] rules;
            public PokemonAbility[] abilities;
            public PokemonAttack[] attacks;
            public WeaknessResistance[] weaknesses;
            public WeaknessResistance[] resistances;
            public int convertedRetreatCost;
            public int number;
            public string artist;
            public string rarity;
            public string flavorText;
            public int[] nationalPokedexNumbers;
            public Legalities legalities;
            public CardImages images;

            private CardSupertype _supertype = CardSupertype.UNKNOWN;
            public CardSupertype Supertype
            {
                get
                {
                    if (_supertype != CardSupertype.UNKNOWN)
                    {
                        return _supertype;
                    }
                    else
                    {
                        return _supertype = CardHelper.GetSupertype(this);
                    }
                }
            }

            private PokemonType[] _types;
            public PokemonType[] Types
            {
                get
                {
                    if(_types !=  null)
                    {
                        return _types;
                    }
                    else
                    {
                        return _types = CardHelper.GetTypes(types);
                    }
                }
            }

            private CardSubtype[] _subtypes;
            public CardSubtype[] Subtypes
            {
                get
                {
                    if(_subtypes != null)
                    {
                        return _subtypes;
                    }
                    else
                    {
                        return _subtypes = CardHelper.GetSubtypes(subtypes);
                    }
                }
            }
        }

        

        [System.Serializable]
        public class CardImages
        {
            public string small;
            public string large;
        }

        [System.Serializable]
        public class WeaknessResistance
        {
            public string type;
            public string value;

            public PokemonType _type;
            public PokemonType Type
            {
                get
                {
                    return CardHelper.GetType(type);
                }
            }
        }

        [System.Serializable]
        public class PokemonAttack
        {
            public string name;
            public string[] cost;
            public int convertedEnergyCost;
            public string damage;
            public string text;

            private PokemonType[] _cost;
            public PokemonType[] Cost
            {
                get
                {
                    if(_cost != null)
                    {
                        return _cost;
                    }
                    else
                    {
                        return _cost = CardHelper.GetTypes(cost);
                    }
                }
            }
        }

        [System.Serializable]
        public class PokemonAbility
        {
            public string name;
            public string text;
            public string type;
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
            UNKNOWN, STAGE_1, STAGE_2, EX, MEGA, BREAK, RESTORED, GX, V, VMAX,
            // Trainers
            ITEM, PKMN_TOOL, PKMN_TOOL_FLARE, SUPPORTER, STADIUM,
            // Energy
            SPECIAL,
            // Common
            BASIC, TAG_TEAM, RAPID_STRIKE, SINGLE_STRIKE, FUSION_STRIKE
        }
    }
}
public class CardDatabase
{
    
}
