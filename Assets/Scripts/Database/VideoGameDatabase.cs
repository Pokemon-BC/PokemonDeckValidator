using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PKMN
{
    public static class VideoGameDatabase
    {
        public static Dictionary<string, VGPokemon> pokemonNameToPokedex;

        private static bool initialized = false;
        public static bool Initialized { get => initialized; }

        public static void Init()
        {
            pokemonNameToPokedex = new Dictionary<string, VGPokemon>();

            TextAsset pokedex = Resources.Load<TextAsset>("pokemon-vg-data/vg-pokedex");
            VGData data = JsonUtility.FromJson<VGData>(pokedex.text);

            for (int i = 0, count = data.data.Length; i < count; i++)
            {
                VGPokemon current = data.data[i];
                if (!pokemonNameToPokedex.TryAdd(current.Name, current))
                {
                    Debug.LogWarning($"Failed to add {current.Name}, hopefully it isn't a problem");
                }
            }

            initialized = true;
        }

        public static VGPokemon LookupPokemon(string name)
        {
            if (!initialized) Init();
            if (pokemonNameToPokedex.TryGetValue(name, out VGPokemon vgp))
            {
                return vgp;
            }
            else
            {
                return null;
            }
        }
    }

    [System.Serializable]
    public class VGData
    {
        [SerializeField]
        public VGPokemon[] data;
    }

    // Not as optimized as cards because they aren't as important
    [System.Serializable]
    public class VGPokemon
    {
        [SerializeField]
        public string No;
        [SerializeField]
        public string Original_Name;
        [SerializeField]
        public string Name;
        [SerializeField]
        public string Generation;
        [SerializeField]
        public string Height;
        [SerializeField]
        public string Weight;
        [SerializeField]
        public string Type1;
        [SerializeField]
        public string Type2;
        [SerializeField]
        public string Ability1;
        [SerializeField]
        public string Ability2;
        [SerializeField]
        public string Ability_Hidden;
        [SerializeField]
        public string Color;
        [SerializeField]
        public string Gender_Male;
        [SerializeField]
        public string Gender_Female;
        [SerializeField]
        public string Gender_Unknown;
        [SerializeField]
        public string Egg_Steps;
        [SerializeField]
        public string Egg_Group1;
        [SerializeField]
        public string Egg_Group2;
        [SerializeField]
        public string Capture_Rate;
        [SerializeField]
        public string Base_EXP;
        [SerializeField]
        public string Final_EXP;
        [SerializeField]
        public string Category;
        [SerializeField]
        public string Mega_Evolution_Flag;
        [SerializeField]
        public string Regional_Forme;
        [SerializeField]
        public string HP;
        [SerializeField]
        public string Attack;
        [SerializeField]
        public string Defense;
        [SerializeField]
        public string SP_Attack;
        [SerializeField]
        public string SP_Defense;
        [SerializeField]
        public string Speed;
        [SerializeField]
        public string Total;
        [SerializeField]
        public string Ef_HP;
        [SerializeField]
        public string EF_Attack;
        [SerializeField]
        public string Ef_Defense;
        [SerializeField]
        public string Ef_SP_Attack;
        [SerializeField]
        public string Ef_SP_Defense;
        [SerializeField]
        public string Ef_Speed;

        public override bool Equals(object obj)
        {
            return obj is VGPokemon dex && Equals(dex);
        }

        public bool Equals(VGPokemon dex)
        {
            return No.Equals(dex.No) && Name.Equals(dex.Name);
        }

        public override int GetHashCode()
        {
            return No.GetHashCode() * Name.GetHashCode();
        }

        public override string ToString()
        {
            return JsonUtility.ToJson(this);
        }
    }
}
