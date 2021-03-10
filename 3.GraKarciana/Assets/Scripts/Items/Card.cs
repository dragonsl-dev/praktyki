using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class Card : MonoBehaviour
{
    public enum CardSuit
    {
        Spades, //pik (strzala)
        Hearts, //kier (serce)
        Clubs, //trefl (3 kolka)
        Diamonds //karo (romb)
    }

    public enum CardRank
    {
        L2,
        L3,
        L4,
        L5,
        L6,
        L7,
        L8,
        L9,
        L10,
        J,
        Q,
        K,
        A
    }

    public CardRank Rank;
    public CardSuit Suit;

    public Texture[] RankTextures;
    public Texture[] SuitTextures;

    public new Renderer renderer;

    public static bool IsSequence(Card[] c)
    {
        for (int i = 0; i < 4; i++)
        {
            if (c[i].Rank + 1 != c[i + 1].Rank)
                return false;
        }
        return true;
    }

    public static bool IsSameSuit(Card[] c)
    {
        for (int i = 0; i < 4; i++)
        {
            if (c[i].Suit != c[i + 1].Suit)
                return false;
        }
        return true;
    }



    public static (int, string, int) GetLevel(Card[] cards)
    {
        Debug.Assert(cards.Length == 5, "(getlvl) Card count is not 5!");
        // 10, 9
        if (IsSequence(cards))
        {
            if (IsSameSuit(cards))
            {
                if (cards[0].Rank == CardRank.L10)
                    return (10, "Royal Flush", 100);
                return (9, "Straight Flush", (int)cards[0].Rank);
            }
        }

        // 8 - 4 of a kind
        for (int i = 0; i < 2; i++)
        {
            if (cards[i].Rank == cards[i + 1].Rank && cards[i + 1].Rank == cards[i + 2].Rank && cards[i + 2].Rank == cards[i + 3].Rank)
                return (8, "4 of a kind", 0);
        }

        // 7 - full house
        if (cards[0].Rank == cards[1].Rank)
        {
            if (cards[2].Rank == cards[3].Rank && cards[3].Rank == cards[4].Rank)
                return (7, "Full House", 0);
        }

        // 7 - full house
        if (cards[3].Rank == cards[4].Rank)
        {
            if (cards[1].Rank == cards[2].Rank && cards[2].Rank == cards[3].Rank)
                return (7, "Full House", 0);
        }

        // 6  - flush
        if (IsSameSuit(cards))
            return (6, "Flush", (int)cards[0].Rank);

        // 5 - straight
        if (IsSequence(cards))
            return (5, "Straight", (int)cards[0].Rank);

        // 4 - three of a kind
        for (int i = 0; i < 3; i++)
        {
            if (cards[i].Rank == cards[i + 1].Rank && cards[i + 1].Rank == cards[i+2].Rank)
            {
                return (4, "Three of a kind", (int)cards[i].Rank); // todo: 2 wartosci
            }

        }

        // 3 - two pair
        for (int i = 0; i < 2; i++)
        {
            if (cards[i].Rank == cards[i+1].Rank)
                if (cards[i+2].Rank == cards[i+3].Rank)
                    return (3, "2 Pair", 0);
        }

        // 2 - one pair
        for (int i = 0; i < 4; i++)
        {
            if (cards[i].Rank == cards[i + 1].Rank)
                return (2, "Pair", (int)cards[i].Rank);
        }

        return (1, "High card", 0); //todo (0)
    }
    class m
    {
        public CardRank rank;
        public CardSuit suit;
        public m(CardRank r, CardSuit s)
        {
            rank = r;
            suit = s;
        }

        public m(CardRank r)
        {
            rank = r;
            suit = CardSuit.Clubs;
        }
    }
    private void OnValidate()
    {
        renderer = GetComponent<Renderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        var rankMat = renderer.materials[1];
        var suitMat = renderer.materials[2];
        rankMat.EnableKeyword("_MainTex");
        suitMat.EnableKeyword("_MainTex");

        rankMat.SetTexture("_MainTex", RankTextures[(int)Rank]);
        suitMat.SetTexture("_MainTex", SuitTextures[(int)Suit]);
        
    }
}
