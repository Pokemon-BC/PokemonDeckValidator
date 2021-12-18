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
                return JSonUtils.FromJson<PokemonSet>(JSonUtils.FixJson(setJson.text));
            }

            public static PokemonCard[] LoadCardsInSet(string setId)
            {
                TextAsset setJson = Resources.Load<TextAsset>("pokemon-tcg-data/cards/en/" + setId);
                return JSonUtils.FromJson<PokemonCard>(JSonUtils.FixJson(setJson.text));
            }
        }

        static class CardHelper
        {
            public static CardSupertype GetSupertype(string supertype)
            {
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
            [SerializeField]
            private string id;
            public string ID { get => id; }
            [SerializeField]
            private string name;
            public string Name { get => name; }
            [SerializeField]
            private string series;
            public string Series { get => series; }
            [SerializeField]
            private int printedTotal;
            public int PrintedTotal { get => printedTotal; }
            [SerializeField]
            private int total;
            public int Total { get => total; }
            [SerializeField]
            private Legalities legalities;
            public Legalities Legalities { get => legalities; }
            [SerializeField]
            private string ptcgoCode;
            public string PtcgoCode { get => ptcgoCode; }
            [SerializeField]
            private string releaseDate;
            public string ReleaseDate { get => releaseDate; }
            [SerializeField]
            private string updatedAt;
            public string UpdatedAt { get => updatedAt; }
            [SerializeField]
            private SetImages images;
            public SetImages Images { get => images; }
        }

        [System.Serializable]
        public class Legalities
        {
            [SerializeField]
            private string unlimited;
            [SerializeField]
            private string standard;
            [SerializeField]
            private string expanded;

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
            [SerializeField]
            private string symbol;
            public string Symbol { get => symbol; }
            [SerializeField]
            private string logo;
            public string Logo { get => logo; }
        }

        [System.Serializable]
        public class PokemonCard
        {
            [SerializeField]
            private string id;
            public string ID { get => id; }
            [SerializeField]
            private string name;
            public string Name { get => name; }
            [SerializeField]
            private string supertype;
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
                        return _supertype = CardHelper.GetSupertype(supertype);
                    }
                }
            }
            [SerializeField]
            private string[] subtypes;
            private CardSubtype[] _subtypes;
            public CardSubtype[] Subtypes
            {
                get
                {
                    if (_subtypes != null)
                    {
                        return _subtypes;
                    }
                    else
                    {
                        return _subtypes = CardHelper.GetSubtypes(subtypes);
                    }
                }
            }
            [SerializeField]
            private string hp;
            // TODO can this be an int?
            public string HP { get => hp; }
            [SerializeField]
            private string[] types;
            private PokemonType[] _types;
            public PokemonType[] Types
            {
                get
                {
                    if (_types != null)
                    {
                        return _types;
                    }
                    else
                    {
                        return _types = CardHelper.GetTypes(types);
                    }
                }
            }
            [SerializeField]
            private string evolvesFrom;
            public string EvolvesFrom { get => evolvesFrom; }
            [SerializeField]
            private string[] evolvesTo;
            public string[] EvolvesTo { get => evolvesTo; }
            [SerializeField]
            private string[] rules;
            public string[] Rules { get => rules; }
            [SerializeField]
            private PokemonAbility[] abilities;
            public PokemonAbility[] Abilities { get => abilities; }
            [SerializeField]
            private PokemonAttack[] attacks;
            public PokemonAttack[] Attacks { get => attacks; }
            [SerializeField]
            private WeaknessResistance[] weaknesses;
            public WeaknessResistance[] Weakness { get => weaknesses; }
            [SerializeField]
            private WeaknessResistance[] resistances;
            public WeaknessResistance[] Resistances { get => resistances; }
            [SerializeField]
            private int convertedRetreatCost;
            public int ConvertedRetreatCost { get => convertedRetreatCost; }
            [SerializeField]
            private int number;
            public int Number { get => number; }
            [SerializeField]
            private string artist;
            public string Artist { get => artist; }
            [SerializeField]
            private string rarity;
            // TODO can probably be an enum
            public string Rarity { get => rarity; }
            [SerializeField]
            private string flavorText;
            public string FlavorText { get => flavorText; }
            [SerializeField]
            private int[] nationalPokedexNumbers;
            public int[] NationalPokedexNumbers { get => nationalPokedexNumbers; }
            [SerializeField]
            private Legalities legalities;
            public Legalities Legalities { get => legalities; }
            [SerializeField]
            private CardImages images;
            public CardImages Images { get => images; }
        }

        [System.Serializable]
        public class CardImages
        {
            [SerializeField]
            private string small;
            public string Small { get => small; }
            [SerializeField]
            private string large;
            public string Large { get => large; }
        }

        [System.Serializable]
        public class WeaknessResistance
        {
            [SerializeField]
            private string type;
            public PokemonType Type
            {
                get
                {
                    return CardHelper.GetType(type);
                }
            }
            [SerializeField]
            private string value;
            // TODO can this be an int?
            public string Value { get => value; }
        }

        [System.Serializable]
        public class PokemonAttack
        {
            [SerializeField]
            private string name;
            public string Name { get => name; }
            [SerializeField]
            private string[] cost;
            private PokemonType[] _cost;
            public PokemonType[] Cost
            {
                get
                {
                    if (_cost != null)
                    {
                        return _cost;
                    }
                    else
                    {
                        return _cost = CardHelper.GetTypes(cost);
                    }
                }
            }
            [SerializeField]
            private int convertedEnergyCost;
            public int ConvertedEnergyCost { get => convertedEnergyCost; }
            [SerializeField]
            private string damage;
            public string Damage { get => damage; }
            [SerializeField]
            private string text;
            public string Text { get => text; }
        }

        [System.Serializable]
        public class PokemonAbility
        {
            [SerializeField]
            private string name;
            public string Name { get => name; }
            [SerializeField]
            private string text;
            public string Text { get => text; }
            [SerializeField]
            private string type;
            public string Type { get => type; }
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
