# Pokémon TCG Deck List Validator
Helps check that your decklist is playablein non-standard formats. Can be modified to support any formats you need (see below).

## What is a Format?
In trading card games like the Pokemon Trading Card Game (TCG) and Magic: The Gathering (MTG), a format defines what cards players can use and how they build their decks. The most common formats are *Standard* format which includes cards from the past ~2 years, and *Expanded* (Pokemon) or *Modern* (MTG) format which includes cards from the past ~10 years. Any cards that are older than the specified format are said to have "rotated" out and cannot be played in current tournaments. 
While this can be burdensome on players forcing them to buy new cards roughly each season, it also keeps the game fair by allowing problematic cards or mechanics to naturally expire and leave play without having to maintain a growing ban list.

## What does this do?
While the *Standard* and *Expanded* formats are well defined, recently there has been an increase in players looking for other formats and rulesets to play in. Notibly [Tricky Gym](https://www.youtube.com/c/TrickyGym), a Pokemon TCG youtuber launched the *Gym Leader Challenge* format. And Oregon Pokemon Professor [GoomyGumi](https://www.twitch.tv/goomygumi) runs a monthly Goomy Cup which changes formats each month. This project reads decklists from the Pokemon Trading Card Game Online and tests them against a custom ruleset to confirm that a deck is playable in these non-standard formats, and provides feedback for players if their deck does not meet the requirements.

## Adding a New Format
**This project uses Unity 2020.3.28f1**
A format is defined with a class extending `PokemonFormat` and should be put in the `/Scripts/Formats/` directory. To start, clone the project and checkout a new branch (`git checkout -b <your_branch_name>`). Delete the monobehaviour code and set your class to extend `PokemonFormat`. It may be beneficial to also add the using directive `using PKMN.Cards` which is the namespace for all card database code.
The class will requrie you to implement `protected override void CustomFormatRules(PokemonDeck deck)`, make a stub for this function, we will come back to it later.

There are existing functions that can be overriden by any format for basic functionality, those are:
- `bool RequireStandardLegal { get => false; }` Overriding and setting this to `true` allows you to expand your format from the existing *Standard* format.
- `bool RequireExpandedLegal { get => false; }` Like above but for the *Expanded* format.
- `bool RequireUnlimitedLegal { get => false; }` The card database represents legalities as *Standard*, *Expanded* or *Unlimited*, therefore this function is included for completeness.
- `int MaxDuplicates { get => 4; }` For formats like the *Gym Leader Challenge* this can be changed so that only 1 copy of a card is allowed. (Works with 2 and 3 copies too).
- `List<ShortCard> FormatBanList { get => null; }` A `ShortCard` is a class containing a string set code, and a string collectors id. Cards included in this list as well as **all reprints** of these cards will be marked as banned for the given format. Because of this, you only need to include a given card once.
- More to come

The last step is to complete the `CustomFormatRules` function. Here, you can use the deck however necessary (documentation to come) to ensure all format rules are enforced.
To apply a ruling, you can call `PokemonDeck.AddNote` or `CardInDeck.AddNote`, these functions take 2 arguments: `NoteType severity` and `string message`. The message can be anything you need, severity however follows the following rules:
- `NoteType.NOTE` Information for players, the card is still valid.
- `NoteType.ERROR` The code handling the card has thrown an error or card lookup did not work. The card may still be legal, it is up to the player to check.
- `NoteType.INVALID` The card is in violation of deck building rules and is not legal for the specified format.
- `NoteType.SUCCESS` Used to show a deck is valid, does not have any meaning for cards.

If any card in the deck has an `INVALID` note, or the deck itself has an `INVALID` note, the deck will be marked as invalid for the given format. Otherwise the deck will be marked legal and ready to play.

### Registering the Format
The last step in making a format is ensuring it will be visible to users in the dropdown menue. Go to `/Scripts/UI/FormatSelect.cs` and add an entry to `Dictionary<string, PokemonFormat> formats` for your format in the form `{"string name", new FormatInstance()}`. Note that the order the formats appear in the dictionary will be the order they appear to users.

You are done, make a Pull Request and assuming there are no major issues the format will be merged and included in the next update. Thank you for contributing!
