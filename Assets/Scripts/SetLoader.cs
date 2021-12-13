using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLoader : MonoBehaviour
{
    //[SerializeField]
    //public PokemonSet[] sets;

    // Start is called before the first frame update
    void Start()
    {
        //TextAsset setJson = Resources.Load<TextAsset>("pokemon-tcg-data/sets/en");
        //Debug.Log(setJson);
        //sets = JSonUtils.FromJson<PokemonSet>(JSonUtils.FixJson(setJson.text));
    }

    public static PokemonSet[] LoadAllSets()
    {
        TextAsset setJson = Resources.Load<TextAsset>("pokemon-tcg-data/sets/en");
        //Debug.Log(setJson);
        return JSonUtils.FromJson<PokemonSet>(JSonUtils.FixJson(setJson.text));
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
    public Images images;
}

[System.Serializable]
public class Legalities
{
    public string unlimited;
    public string standard;
    public string expanded;
}

[System.Serializable]
public class Images
{
    public string symbol;
    public string logo;
}