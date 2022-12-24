
using System.Collections.Generic;

namespace YG
{
    [System.Serializable]
    public class SavesYG
    {
        public bool isFirstSession = true;
        public string language = "ru";
        public bool feedbackDone;
        public bool promptDone;

        // Ваши сохранения
        public List<CardSaveData> Cards = new List<CardSaveData>
        {
            new CardSaveData
            {
                UID = "b60fd3e1-67d3-4e3e-bd76-e9d922d992c9",
                Amount = 1,
                Level = 1,
            },
            new CardSaveData
            {
                UID = "b5025eb8-1d44-44cc-8f80-002014ab15fc",
                Amount = 1,
                Level = 1,
            },
        };
        public List<DeckSaveData> Decks = new List<DeckSaveData>
        {
            new DeckSaveData
            {
                Name = "Default",
                Cards = new List<string>
                {
                    "ce08dc96-b9fb-4de7-9800-32fbcbd07558",
                    "44b4fab2-5d0c-482c-95d8-a7c60536473b",
                },
            },
        };
        public List<PackSaveData> Packs = new()
        {
            new PackSaveData
            {
                Name = "Classic",
                Amount = 1000
            }
        };
        public string LastUsedDeck;
    }
}
