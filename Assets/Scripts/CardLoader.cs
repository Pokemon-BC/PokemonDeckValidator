using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardLoader : MonoBehaviour
{
    [SerializeField]
    public PokemonCard[] cards;
    // Start is called before the first frame update
    void Start()
    {
        TextAsset setJson = Resources.Load<TextAsset>("pokemon-tcg-data/cards/en/swsh8");
        Debug.Log(setJson);
        cards = JSonUtils.FromJson<PokemonCard>(JSonUtils.FixJson(setJson.text));
    }
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
}

[System.Serializable]
public class PokemonAttack
{
    public string name;
    public string[] cost;
    public int convertedEnergyCost;
    public string damage;
    public string text;
}
