using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemData : ScriptableObject
{
    public string Name;
    public int Id;
    public ItemTag Tag;
    public Sprite Icon;
    public string About;

    public float FillAmount;

    public ItemData()
    {

    }

    public void Init(ItemData data)
    {
        Name = data.Name;
        Id = data.Id;
        Tag = data.Tag;
        Icon = data.Icon;
        About = data.About;

        FillAmount = data.FillAmount;
    }
}
