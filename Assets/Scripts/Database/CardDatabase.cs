using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace PKMN
{
    namespace Cards
    {
        public static class PokemonLoader
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

            public static PokemonDeck LoadDeck(string decklist)
            {
                if (!CardDatabase.Initialized) CardDatabase.Init();
                PokemonDeck result = new PokemonDeck();

                string[] lines = decklist.Split('\n');
                for (int i = 0, count = lines.Length; i < count; i++)
                {
                    string line = lines[i];
                    if (line.StartsWith("******"))
                    {
                        // Skip
                    }
                    else if (line.StartsWith("##"))
                    {
                        // Quantity Count for card type
                        string[] subsets = line.Split(' ');
                        string identifier = subsets[0];
                        if (identifier.Contains("Pokémon"))
                        {
                            result.pokemon = int.Parse(subsets[2]);
                        }
                        else if (identifier.Contains("Trainer"))
                        {
                            result.trainers = int.Parse(subsets[3]);
                        }
                        else if (identifier.Contains("Energy"))
                        {
                            result.energies = int.Parse(subsets[2]);
                        }
                    }
                    else if (line.StartsWith("* "))
                    {
                        string[] subsets = line.Split(' ');
                        int quantity = int.Parse(subsets[1]);
                        int collId = int.Parse(subsets[subsets.Length - 1]);
                        string setId = subsets[subsets.Length - 2];
                        string name = "";
                        for (int subi = 2, subcount = subsets.Length - 2; subi < subcount; subi++)
                        {
                            name += " " + subsets[subi];
                        }

                        PokemonCard cardRef = CardDatabase.LookupCard(setId, collId);
                        if(cardRef == null)
                        {
                            cardRef = PokemonCard.GenerateErrorCard(name, collId);
                        }
                        result.deckCards.Add(new CardInDeck(cardRef, setId, quantity));
                    }
                    else if (line.StartsWith("Total Cards -"))
                    {
                        // Final Card Count
                        string[] subsets = line.Split(' ');
                        result.totalCards = int.Parse(subsets[3]);
                    }
                }
                return result;
            }
        }

        public static class CardDatabase
        {
            public static Dictionary<string, PokemonSet> tcgoIdToSet;
            public static Dictionary<string, LoadedSet> internalIdToSet;

            private static bool initialized = false;
            public static bool Initialized { get => initialized; }

            public static void Init()
            {
                tcgoIdToSet = new Dictionary<string, PokemonSet>();
                internalIdToSet = new Dictionary<string, LoadedSet>();
                initialized = true;
                PokemonSet[] sets = PokemonLoader.LoadAllSets();
                for (int i = 0, count = sets.Length; i < count; i++)
                {
                    PokemonSet current = sets[i];
                    string key = current.PtcgoCode;
                    if (current.PtcgoCode == null)
                    {
                        key = current.ID;
                    }
                    if (current.Legalities.IsStandardLegal || current.Legalities.IsExpandedLegal)
                    {
                        if (tcgoIdToSet.ContainsKey(key))
                        {
                            Debug.Log("Conflict between new set " + current.Name + " and old set " + tcgoIdToSet[key].Name);
                        }
                        else
                        {
                            tcgoIdToSet.Add(key, current);
                        }
                    }
                }
            }

            public static PokemonCard LookupCard(string ptcgoId, int collId)
            {
                if (!initialized) Init();
                if (tcgoIdToSet.TryGetValue(ptcgoId, out PokemonSet pokemonSet))
                {
                    LoadedSet ls;
                    if (internalIdToSet.ContainsKey(pokemonSet.ID))
                    {
                        ls = internalIdToSet[pokemonSet.ID];
                    }
                    else
                    {
                        PokemonCard[] cards = PokemonLoader.LoadCardsInSet(pokemonSet.ID);
                        internalIdToSet.Add(pokemonSet.ID, new LoadedSet(pokemonSet.ID, ptcgoId, pokemonSet, cards));
                        ls = internalIdToSet[pokemonSet.ID];
                    }
                    if (collId <= ls.setCards.Count)
                    {
                        return ls.setCards[collId - 1]; // -1 because arrays
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    Debug.Log("Card from problematic set " + ptcgoId + " with col ID " + collId);
                    return null;
                }
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
        }

        public class LoadedSet
        {
            public string ptcgoId;
            public string setId;
            public PokemonSet setData;
            public List<PokemonCard> setCards;

            public LoadedSet(string id, string ptcgo, PokemonSet data, PokemonCard[] cards)
            {
                setId = id;
                ptcgoId = ptcgo;
                setData = data;
                setCards = new List<PokemonCard>(cards);
            }

            public override bool Equals(object obj)
            {
                return obj is LoadedSet set &&
                       ptcgoId == set.ptcgoId &&
                       setId == set.setId &&
                       EqualityComparer<PokemonSet>.Default.Equals(setData, set.setData) &&
                       EqualityComparer<List<PokemonCard>>.Default.Equals(setCards, set.setCards);
            }

            public override int GetHashCode()
            {
                int hashCode = 738309204;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ptcgoId);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(setId);
                hashCode = hashCode * -1521134295 + EqualityComparer<PokemonSet>.Default.GetHashCode(setData);
                hashCode = hashCode * -1521134295 + EqualityComparer<List<PokemonCard>>.Default.GetHashCode(setCards);
                return hashCode;
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

            public override bool Equals(object obj)
            {
                return obj is PokemonSet set &&
                       id == set.id &&
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
                int hashCode = -340245959;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(id);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(series);
                hashCode = hashCode * -1521134295 + printedTotal.GetHashCode();
                hashCode = hashCode * -1521134295 + total.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<Legalities>.Default.GetHashCode(legalities);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ptcgoCode);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(releaseDate);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(updatedAt);
                hashCode = hashCode * -1521134295 + EqualityComparer<SetImages>.Default.GetHashCode(images);
                return hashCode;
            }
        }

        [System.Serializable]
        public class Legalities
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

            public override bool Equals(object obj)
            {
                return obj is SetImages images &&
                       symbol == images.symbol &&
                       logo == images.logo;
            }

            public override int GetHashCode()
            {
                int hashCode = 1339188243;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(symbol);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(logo);
                return hashCode;
            }
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
                return obj is PokemonCard card &&
                       id == card.id &&
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
                int hashCode = 390571997;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(id);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(supertype);
                hashCode = hashCode * -1521134295 + EqualityComparer<string[]>.Default.GetHashCode(subtypes);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(hp);
                hashCode = hashCode * -1521134295 + EqualityComparer<string[]>.Default.GetHashCode(types);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(evolvesFrom);
                hashCode = hashCode * -1521134295 + EqualityComparer<string[]>.Default.GetHashCode(evolvesTo);
                hashCode = hashCode * -1521134295 + EqualityComparer<string[]>.Default.GetHashCode(rules);
                hashCode = hashCode * -1521134295 + EqualityComparer<PokemonAbility[]>.Default.GetHashCode(abilities);
                hashCode = hashCode * -1521134295 + EqualityComparer<PokemonAttack[]>.Default.GetHashCode(attacks);
                hashCode = hashCode * -1521134295 + EqualityComparer<WeaknessResistance[]>.Default.GetHashCode(weaknesses);
                hashCode = hashCode * -1521134295 + EqualityComparer<WeaknessResistance[]>.Default.GetHashCode(resistances);
                hashCode = hashCode * -1521134295 + convertedRetreatCost.GetHashCode();
                hashCode = hashCode * -1521134295 + number.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(artist);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(rarity);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(flavorText);
                hashCode = hashCode * -1521134295 + EqualityComparer<int[]>.Default.GetHashCode(nationalPokedexNumbers);
                hashCode = hashCode * -1521134295 + EqualityComparer<Legalities>.Default.GetHashCode(legalities);
                hashCode = hashCode * -1521134295 + EqualityComparer<CardImages>.Default.GetHashCode(images);
                return hashCode;
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
        public class CardImages
        {
            [SerializeField]
            private string small;
            public string Small { get => small; }
            [SerializeField]
            private string large;
            public string Large { get => large; }

            public override bool Equals(object obj)
            {
                return obj is CardImages images &&
                       small == images.small &&
                       large == images.large;
            }

            public override int GetHashCode()
            {
                int hashCode = 822727712;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(small);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(large);
                return hashCode;
            }
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

            public override bool Equals(object obj)
            {
                return obj is WeaknessResistance resistance &&
                       type == resistance.type &&
                       value == resistance.value;
            }

            public override int GetHashCode()
            {
                int hashCode = 1148455455;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(type);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(value);
                return hashCode;
            }
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

            public override bool Equals(object obj)
            {
                return obj is PokemonAttack attack &&
                       name == attack.name &&
                       EqualityComparer<string[]>.Default.Equals(cost, attack.cost) &&
                       convertedEnergyCost == attack.convertedEnergyCost &&
                       damage == attack.damage &&
                       text == attack.text;
            }

            public override int GetHashCode()
            {
                int hashCode = 1865949528;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
                hashCode = hashCode * -1521134295 + EqualityComparer<string[]>.Default.GetHashCode(cost);
                hashCode = hashCode * -1521134295 + convertedEnergyCost.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(damage);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(text);
                return hashCode;
            }
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

            public override bool Equals(object obj)
            {
                return obj is PokemonAbility ability &&
                       name == ability.name &&
                       text == ability.text &&
                       type == ability.type;
            }

            public override int GetHashCode()
            {
                int hashCode = -1738727807;
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(text);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(type);
                return hashCode;
            }
        }

        public class PokemonDeck
        {
            public List<CardInDeck> deckCards;
            public int pokemon, trainers, energies;
            public int totalCards;

            public List<FormatedNote> deckNotes;

            public PokemonDeck()
            {
                deckCards = new List<CardInDeck>();
                deckNotes = new List<FormatedNote>();
            }

            public void AddNote(NoteType severity, string message)
            {
                deckNotes.Add(new FormatedNote(severity, message));
            }
        }

        public class CardInDeck
        {
            public PokemonCard reference;
            public string setId;
            public int quantity;
            public List<FormatedNote> notes;

            public CardInDeck(PokemonCard pokemonCard, string setId, int quantity)
            {
                this.reference = pokemonCard;
                this.setId = setId;
                this.quantity = quantity;
                notes = new List<FormatedNote>();
            }

            public void AddNote(NoteType severity, string message)
            {
                notes.Add(new FormatedNote(severity, message));
            }
        }

        public class FormatedNote
        {
            public NoteType severity;
            public string text;

            public FormatedNote(NoteType severity, string text)
            {
                this.severity = severity;
                this.text = text;
            }

            public override bool Equals(object obj)
            {
                return obj is FormatedNote note &&
                       severity == note.severity &&
                       text == note.text;
            }

            public override int GetHashCode()
            {
                int hashCode = -1008784448;
                hashCode = hashCode * -1521134295 + severity.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(text);
                return hashCode;
            }
        }

        public enum NoteType
        {
            // Note is just info for the player.
            // Error means a C# error occured but the deck is not necessarly invalid.
            // Invalid means the deck is invalid.
            // Success is used to display a validated deck.
            NOTE, ERROR, INVALID, SUCCESS
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

        public enum Legality
        {
            ILLEGAL, LEGAL, BANNED
        }
    }
}
