
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
                UID = "b9cfe01a-2434-4980-b024-3d53a0f83b40",
                Amount = 1,
                Level = 1,
            },
            new CardSaveData
            {
                UID = "8d99d6de-ad93-4c41-8694-62a3039230d3",
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
                    "b5025eb8-1d44-44cc-8f80-002014ab15fc",
                    "58afd0e9-c88c-4092-baf3-f57d9a8c647f",
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
        public string LastUsedDeck = "Default";
    }
}
