using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System;

namespace PKMN
{
    namespace Cards
    {
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

            public static Legality MapLegality(string legality)
            {
                if (legality == null || legality == "")
                {
                    return Legality.ILLEGAL;
                }
                else if (legality == "Banned")
                {
                    return Legality.BANNED;
                }
                else
                {
                    return Legality.LEGAL;
                }
            }

            public static bool ArraySameElements(object[] arr1, object[] arr2)
            {
                if(arr1 == null && arr2 == null)
                {
                    return true;
                }
                else if((arr1 == null && arr2 != null) || (arr1 != null && arr2 == null))
                {
                    return false;
                }
                HashSet<object> outstanding = new HashSet<object>();
                for (int i = 0, count = arr1.Length; i < count; i++)
                {
                    outstanding.Add(arr1[i]);
                }
                for (int i = 0, count = arr2.Length; i < count; i++)
                {
                    if (outstanding.Contains(arr2[i]))
                    {
                        outstanding.Remove(arr2[i]);
                    }
                }
                return outstanding.Count == 0;
            }

            public static int ExtractNumber(string textNumber)
            {
                string extractedNumber = Regex.Match(textNumber, @"\d+").Value;
                return int.Parse(extractedNumber);
            }
        }

         [System.Serializable]
        public class PokemonSet : IEquatable<PokemonSet>
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

            public override bool Equals(object obj)
            {
                return obj is PokemonSet set && Equals(set);
            }

            public bool Equals(PokemonSet set)
            {
                return id == set.id &&
                       name == set.name &&
                       series == set.series &&
                       printedTotal == set.printedTotal &&
                       total == set.total &&
                       EqualityComparer<Legalities>.Default.Equals(legalities, set.legalities) &&
                       ptcgoCode == set.ptcgoCode &&
                       releaseDate == set.releaseDate &&
                       updatedAt == set.updatedAt &&
                       EqualityComparer<SetImages>.Default.Equals(images, set.images);
            }

            public override int GetHashCode()
            {
                return id.GetHashCode() * name.GetHashCode() * series.GetHashCode() * printedTotal.GetHashCode() * 
                       total.GetHashCode() * legalities.GetHashCode() * ptcgoCode.GetHashCode() * releaseDate.GetHashCode() * 
                       updatedAt.GetHashCode() * images.GetHashCode();
            }

            public override string ToString()
            {
                return JsonUtility.ToJson(this);
            }
        }

        [System.Serializable]
        public class Legalities : IEquatable<Legalities>
        {
            [SerializeField]
            private string unlimited;
            public Legality Unlimited { get => CardHelper.MapLegality(unlimited); }
            public bool IsUnlimitedLegal
            {
                get
                {
                    return unlimited != null && unlimited != "" && unlimited != "Banned";
                }
            }
            [SerializeField]
            private string standard;
            public Legality Standard { get => CardHelper.MapLegality(standard); }
            public bool IsExpandedLegal
            {
                get
                {
                    return expanded != null && expanded != "" && expanded != "Banned";
                }
            }
            [SerializeField]
            private string expanded;
            public Legality Expanded { get => CardHelper.MapLegality(expanded); }
            public bool IsStandardLegal
            {
                get
                {
                    return standard != null && standard != "" && standard != "Banned";
                }
            }

            public override bool Equals(object obj)
            {
                return obj is Legalities legalities && Equals(legalities);
            }

            public bool Equals(Legalities other)
            {
                return unlimited.Equals(other.unlimited) &&
                       standard.Equals(other.standard) &&
                       expanded.Equals(other.standard);
            }

            public override int GetHashCode()
            {
                return unlimited.GetHashCode() * standard.GetHashCode() * expanded.GetHashCode();
            }

            public override string ToString()
            {
                return JsonUtility.ToJson(this);
            }
        }

        [System.Serializable]
        public class SetImages : IEquatable<SetImages>
        {
            [SerializeField]
            private string symbol;
            public string Symbol { get => symbol; }
            [SerializeField]
            private string logo;
            public string Logo { get => logo; }

            public override bool Equals(object obj)
            {
                return obj is SetImages images && Equals(images);
            }

            public bool Equals(SetImages images)
            {
                return symbol == images.symbol &&
                       logo == images.logo;
            }

            public override int GetHashCode()
            {
                return symbol.GetHashCode() * logo.GetHashCode();
            }

            public override string ToString()
            {
                return JsonUtility.ToJson(this);
            }
        }

        [System.Serializable]
        public class PokemonCard : IEquatable<PokemonCard>
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
            private string number;
            public int Number { get => CardHelper.ExtractNumber(number); }
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
            private string regulationMark;
            public string RegulationMark { get => regulationMark; }
            [SerializeField]
            private CardImages images;
            public CardImages Images { get => images; }

            public bool IsReprintOf(PokemonCard other)
            {
                return other.name == name && 
                       other.supertype == supertype && 
                       CardHelper.ArraySameElements(other.subtypes, subtypes) &&
                       other.hp == hp &&
                       CardHelper.ArraySameElements(other.types, types) &&
                       other.evolvesFrom == evolvesFrom &&
                       CardHelper.ArraySameElements(other.evolvesTo, evolvesTo) &&
                       CardHelper.ArraySameElements(other.rules, rules) &&
                       CardHelper.ArraySameElements(other.abilities, abilities) &&
                       CardHelper.ArraySameElements(other.attacks, attacks) &&
                       CardHelper.ArraySameElements(other.weaknesses, weaknesses) &&
                       CardHelper.ArraySameElements(other.resistances, resistances) &&
                       other.convertedRetreatCost == convertedRetreatCost;
            }

            public override bool Equals(object obj)
            {
                return obj is PokemonCard card && Equals(card);
            }

            public bool Equals(PokemonCard card)
            {
                return id == card.id &&
                       name == card.name &&
                       supertype == card.supertype &&
                       EqualityComparer<string[]>.Default.Equals(subtypes, card.subtypes) &&
                       hp == card.hp &&
                       EqualityComparer<string[]>.Default.Equals(types, card.types) &&
                       evolvesFrom == card.evolvesFrom &&
                       EqualityComparer<string[]>.Default.Equals(evolvesTo, card.evolvesTo) &&
                       EqualityComparer<string[]>.Default.Equals(rules, card.rules) &&
                       EqualityComparer<PokemonAbility[]>.Default.Equals(abilities, card.abilities) &&
                       EqualityComparer<PokemonAttack[]>.Default.Equals(attacks, card.attacks) &&
                       EqualityComparer<WeaknessResistance[]>.Default.Equals(weaknesses, card.weaknesses) &&
                       EqualityComparer<WeaknessResistance[]>.Default.Equals(resistances, card.resistances) &&
                       convertedRetreatCost == card.convertedRetreatCost &&
                       number == card.number &&
                       artist == card.artist &&
                       rarity == card.rarity &&
                       flavorText == card.flavorText &&
                       EqualityComparer<int[]>.Default.Equals(nationalPokedexNumbers, card.nationalPokedexNumbers) &&
                       EqualityComparer<Legalities>.Default.Equals(legalities, card.legalities) &&
                       EqualityComparer<CardImages>.Default.Equals(images, card.images);
            }

            public override int GetHashCode()
            {
                return id.GetHashCode() * name.GetHashCode() * supertype.GetHashCode() * hp.GetHashCode() * evolvesFrom.GetHashCode() * convertedRetreatCost.GetHashCode() * 
                       number.GetHashCode() * artist.GetHashCode() * rarity.GetHashCode() * flavorText.GetHashCode() * legalities.GetHashCode() * images.GetHashCode();
            }

            public override string ToString()
            {
                return JsonUtility.ToJson(this);
            }

            public static PokemonCard GenerateErrorCard(string name, int id)
            {
                PokemonCard output = new PokemonCard();
                output.name = name;
                output.id = id.ToString();
                output.supertype = "";
                output._supertype = CardSupertype.UNKNOWN;
                return output;
            }
        }

        [System.Serializable]
        public class CardImages : IEquatable<CardImages>
        {
            [SerializeField]
            private string small;
            public string Small { get => small; }
            [SerializeField]
            private string large;
            public string Large { get => large; }

            public override bool Equals(object obj)
            {
                return obj is CardImages images && Equals(images);
            }

            public bool Equals(CardImages images)
            {
                return small == images.small &&
                       large == images.large;
            }

            public override int GetHashCode()
            {
                return small.GetHashCode() * large.GetHashCode();
            }

            public override string ToString()
            {
                return JsonUtility.ToJson(this);
            }
        }

        [System.Serializable]
        public class WeaknessResistance : IEquatable<WeaknessResistance>
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

            public override bool Equals(object obj)
            {
                return obj is WeaknessResistance weeakResist && Equals(weeakResist);
            }

            public bool Equals(WeaknessResistance weakResist)
            {
                return type == weakResist.type &&
                       value == weakResist.value;
            }

            public override int GetHashCode()
            {
                return type.GetHashCode() * value.GetHashCode();
            }

            public override string ToString()
            {
                return JsonUtility.ToJson(this);
            }
        }

        [System.Serializable]
        public class PokemonAttack : IEquatable<PokemonAttack>
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

            public override bool Equals(object obj)
            {
                return obj is PokemonAttack attack && Equals(attack);
            }

            public bool Equals(PokemonAttack attack)
            {
                return name == attack.name &&
                       CardHelper.ArraySameElements(cost, attack.cost) &&
                       convertedEnergyCost == attack.convertedEnergyCost &&
                       damage == attack.damage &&
                       text == attack.text;
            }

            public override int GetHashCode()
            {
                return name.GetHashCode() * 
                       convertedEnergyCost.GetHashCode() * 
                       damage.GetHashCode() * 
                       text.GetHashCode();
            }

            public override string ToString()
            {
                return JsonUtility.ToJson(this);
            }
        }

        [System.Serializable]
        public class PokemonAbility : IEquatable<PokemonAbility>
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

            public override bool Equals(object obj)
            {
                return obj is PokemonAbility ability && Equals(ability);
            }

            public bool Equals(PokemonAbility ability)
            {
                return name == ability.name &&
                       text == ability.text &&
                       type == ability.type;
            }

            public override int GetHashCode()
            {
                return name.GetHashCode() * text.GetHashCode() * type.GetHashCode();
            }

            public override string ToString()
            {
                return JsonUtility.ToJson(this);
            }
        }


    }
}