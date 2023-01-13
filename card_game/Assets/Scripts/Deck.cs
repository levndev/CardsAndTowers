using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class Deck
{
    public string Name;
    public List<CardSO> deckList { get; private set; } = new List<CardSO>();

    public Deck(string name, List<CardSO> deckList)
    {
        this.Name = name;
        this.deckList = deckList;
    }

    public Deck()
    {

    }

    public static void SaveDeck(Deck deck)
    {
        using (StreamWriter sw = File.CreateText(Application.persistentDataPath + "/" + deck.Name + ".txt"))
        {
            foreach (var card in deck.deckList)
            {
                sw.WriteLine(card.name);
            }
        }
    }

    public Queue<CardSO> GetDeckQueue()
    {
        //var random = new System.Random();
        var generatedQueue = new Queue<CardSO>();
        foreach (var card in deckList)
        {
            generatedQueue.Enqueue(card);
        }
        return generatedQueue;
    }

    public List<CardSO> GetDeckList(bool sorted = false)
    {
        if (sorted)
        {
            return GetSortedDeckList();
        }
        return deckList;
    }

    private List<CardSO> GetSortedDeckList()
    {
        var sorted = deckList.ToList();
        sorted.Sort((card1, card2) => card1.Cost - card2.Cost);
        return sorted;
    }


    public void AddToList(CardSO card)
    {
        if (CanAddToDeck(card))
        {
            deckList.Add(card);
        }
    }

    public bool CanAddToDeck(CardSO card)
    {
        return deckList.Count < 10 && !deckList.Contains(card);
    }

    public void RemoveFromDeck(CardSO card)
    {
        deckList.Remove(card);
    }

    public Deck Clone()
    {
        var newDeck = new Deck();
        newDeck.Name = new string(Name);
        newDeck.deckList = new();
        foreach (var card in deckList)
        {
            newDeck.AddToList(card);
        }
        return newDeck;
    }
}
