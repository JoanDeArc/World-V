using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string Name;
    public int Id;
    public ItemTag Tag;
    public Sprite Icon;
    public string About;

    public virtual ItemData GetData()
    {
        ItemData itemData = ScriptableObject.CreateInstance<ItemData>();
        itemData.Name = Name;
        itemData.Id = Id;
        itemData.Tag = Tag;
        itemData.Icon = Icon;
        itemData.About = About;

        return itemData;
    }

    public Item(ItemData data)
    {
        Name = data.Name;
        Id = data.Id;
        Tag = data.Tag;
        Icon = data.Icon;
        About = data.About;
    }
}
