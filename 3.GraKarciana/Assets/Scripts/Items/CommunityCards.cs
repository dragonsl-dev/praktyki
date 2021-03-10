using UnityEngine;
using System.Collections;

public class CommunityCards : MonoBehaviour
{
    public Transform CommunityCardsTransf;
    public Container<Card> CommunityCardContainer = new Container<Card>();
    public Transform[] Placement;

    private void Start()
    {
        CommunityCardContainer.storage = CommunityCardsTransf;
        CommunityCardContainer.OnModify += CommunityCardContainer_OnModify;
    }

    private void CommunityCardContainer_OnModify(Container<Card> obj)
    {
        int c = 0;
        foreach(var card in obj.GetArray())
        {
            card.transform.position = Placement[c].position;
            c += 1;
        }
    }
}
