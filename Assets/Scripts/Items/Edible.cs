using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edible : Item
{
    public float FillAmount;

    public override ItemData GetData()
    {
        ItemData itemData = base.GetData();
        itemData.FillAmount = FillAmount;
        return itemData;
    }

    public Edible(ItemData data) : base(data)
    {
        FillAmount = data.FillAmount;
    }
}
