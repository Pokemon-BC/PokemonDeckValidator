using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class FormatSelect : MonoBehaviour
{
    public Dropdown selector;

    public static Dictionary<string, PokemonFormat> formats = new Dictionary<string, PokemonFormat>()
    {
        {"GC: Spring Fling", new GCSpringFling()},
        {"TJ: Gym Leader Challenge", new TrickyGymGLC()},
        {"Standard", new StandardFormat()},
        {"Expanded", new ExpandedFormat()},
        {"GC: New Start", new GCNewStart()},
        {"GC: Expanded Limited", new GCExpandedLimited()}
    };

    protected List<Dropdown.OptionData> options;

    // Start is called before the first frame update
    void Start()
    {
        if(selector == null)
        {
            selector = GetComponent<Dropdown>();
        }
        selector.ClearOptions();
        options = new List<Dropdown.OptionData>();
        foreach(string formatName in formats.Keys)
        {
            options.Add(new Dropdown.OptionData(formatName));
        }
        selector.AddOptions(options);
    }

    public PokemonFormat GetSelectedFormat()
    {
        return formats[options[selector.value].text];
    }
}
