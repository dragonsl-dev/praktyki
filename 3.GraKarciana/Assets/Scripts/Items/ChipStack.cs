using UnityEngine;
using System.Collections;

public class ChipStack : MonoBehaviour
{
    public Transform ChipTemplate;
    public Transform[] Stacks;
    public int MaxChipsPerStack;

    private int stackCount = 0;

    public int StackCount { get => stackCount; set => UpdateStack(value); }

    private float chipHeight;
    private void Start()
    {
        chipHeight = ChipTemplate.GetComponent<Renderer>().bounds.size.y;
    }

    private void UpdateStack(int amount)
    {
        int oldAmount = stackCount;
        stackCount = amount;

        int c = 0;
        foreach (var i in Stacks)
        {
            foreach(Transform j in i)
            {
                if (c < oldAmount)
                {
                    c += 1;
                    continue;
                } else
                {
                    Destroy(j.gameObject);
                    c += 1;
                }
            }
        }

        c = 0;
        int n = 0;
        for (var i = 0; i < amount; i++)
        {
            if (n > Stacks.Length-1)
            {
                break;
            }

            var currentStack = Stacks[n];

            var chip = Instantiate(ChipTemplate, currentStack);

            var newPos = currentStack.transform.position;
            newPos.y += chipHeight * c;
            chip.transform.position = newPos;

            c += 1;
            if (c >= MaxChipsPerStack)
            {
                c = 0;
                n += 1;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var stack in Stacks)
        {
            GizmosEx.DrawYAligment(stack.position, 0.025f);
        }
    }
}
