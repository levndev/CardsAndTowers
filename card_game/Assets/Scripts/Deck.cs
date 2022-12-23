using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Deck
{
    public string Name;
    private List<Card> deckList = new List<Card>();

    public static Deck LoadFromFile(string name)
    {
        var deck = new Deck();
        try
        {
            var lines = File.ReadAllLines(Application.persistentDataPath + "/" + name + ".txt");
            deck.Name = name;
            foreach (var line in lines)
            {
                deck.AddToList(Card.LoadfromFile(line));
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

    public Queue<Card> GetDeckQueue()
    {
        //var random = new System.Random();
        var generatedQueue = new Queue<Card>();
        foreach (var card in deckList)
        {
            generatedQueue.Enqueue(card);
        }
        return generatedQueue;
    }

    public List<Card> GetDeckList()
    {
        return deckList;
    }

    public void AddToList(Card card)
    {
        if (CanAddToDeck(card))
        {
            deckList.Add(card);
        }
    }

    public bool CanAddToDeck(Card card)
    {
        return !deckList.Contains(card);
    }

    public void RemoveFromDeck(Card card)
    {
        deckList.Remove(card);
    }
}
