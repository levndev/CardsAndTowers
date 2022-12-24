using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    public static Deck LoadFromFile(string name)
    {
        var deck = new Deck();
        try
        {
            var lines = File.ReadAllLines(Application.persistentDataPath + "/" + name + ".txt");
            deck.Name = name;
            foreach (var line in lines)
            {
                deck.AddToList(CardSO.LoadfromFile(line));
            }
        }
        catch
        {
            Debug.Log($"Can not load deck \"{name}\" from file");
            return null;
        }
        return deck;
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

    public List<CardSO> GetDeckList()
    {
        return deckList;
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
        return !deckList.Contains(card);
    }

    public void RemoveFromDeck(CardSO card)
    {
        deckList.Remove(card);
    }
}
