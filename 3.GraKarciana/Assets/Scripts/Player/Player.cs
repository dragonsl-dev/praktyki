using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public abstract class Player : MonoBehaviour
{
    public TextMeshPro NameTagText;

    private string _name;
    public string Name {
        get => _name;
        set {
            _name = value;
            UpdateNameText();
        }
    }
    private void UpdateNameText() => NameTagText.text = $"{_name} ({_coins})";

    public Color ActiveColor;
    public Color FoldColor;
    public Renderer RendererComp;
    private bool isFold = false;
    public bool IsFold { get => isFold;
        set {
            isFold = value;
            if (RendererComp != null)
                RendererComp.material.color = value ? FoldColor : ActiveColor;
        }
    }
    public int index;
    public Player next;

    private int _coins;
    public int Coins {
        get => _coins;
        set {
            _coins = value;
            _ChipStack.StackCount = value;
            UpdateNameText();
        }
    }

    

    public Transform HoldCardsC;
    public Container<Card> HoldCards = new Container<Card>();

    public Transform[] CardPlacement = new Transform[2];
    public Transform DealerPlacement;

    public ChipStack _ChipStack;

    protected virtual void Start()
    {
        HoldCards.storage = HoldCardsC;

        HoldCards.OnModify += HoldCards_OnModify;

        Coins = 25;
    }

    public bool TryTakeCoins(int amount)
    {
        if (amount < Coins)
        {
        }
        print("IMPLEMENT HERE");
            return true;
    }

    public void FlipCards()
    {
        foreach(var card in CardPlacement)
            card.eulerAngles += new Vector3(0f, 180f, 0f);
    }

    private void HoldCards_OnModify(Container<Card> obj)
    {
        if (obj.Count == 1 || obj.Count == 2)
        {
            var card1 = obj.Get(0).transform;
            card1.position = CardPlacement[0].position;
            card1.eulerAngles = CardPlacement[0].eulerAngles;
        } 
        if (obj.Count == 2)
        {
            var card2 = obj.Get(1).transform;
            card2.position = CardPlacement[1].position;
            card2.eulerAngles = CardPlacement[1].eulerAngles;
        }
    }

    public abstract IEnumerator Prompt(Stage gameStage, System.Action<Action> result);

    private void OnDrawGizmos()
    {
        GizmosEx.DrawYAligment(CardPlacement[0].position, .1f);
        GizmosEx.DrawYAligment(CardPlacement[1].position, .1f);

    }
}
