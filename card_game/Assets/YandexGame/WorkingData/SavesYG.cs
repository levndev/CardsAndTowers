
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
                UID = "40e8b3d7-3cf9-4de7-bfe8-b242a8809e27",
                Amount = 1,
                Level = 1,
            },
            new CardSaveData
            {
                UID = "86c24567-fc5f-4150-a8dd-652d8c254a09",
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
                    "40e8b3d7-3cf9-4de7-bfe8-b242a8809e27",
                    "86c24567-fc5f-4150-a8dd-652d8c254a09",
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
