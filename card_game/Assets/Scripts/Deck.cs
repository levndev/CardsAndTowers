using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck
{
    private List<Card> deckList = new List<Card>();
  
    public Queue<Card> GetDeckQueue()
    {
        //var random = new System.Random();
        var generatedQueue = new Queue<Card>();
        foreach(var card in deckList)
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
        if (!CanAddToDeck(card))
        {
            deckList.Add(card);
        }
    }

    public bool CanAddToDeck(Card card)
    {
        return !deckList.Contains(card);
    }
}
