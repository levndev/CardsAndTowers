
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
        public List<CardSaveData> Cards = new();
        public List<DeckSaveData> Decks = new();
        public List<PackSaveData> Packs = new();
        public string LastUsedDeck;
        public uint GoldAmount;
    }
}
