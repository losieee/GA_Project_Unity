using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreedyCoinSample : MonoBehaviour
{
    // Start is called before the first frame update
    int[] coinType = { 500, 100, 50, 10 };
    void Start()
    {
        Debug.Log(CountCoins(1260));
    }

    int CountCoins(int amount)
    {
        int count = 0;

        foreach(int c in coinType)
        {
            int use = amount / c;
            count += use;
            amount -= use * c;
        }

        return count;
    }
}
