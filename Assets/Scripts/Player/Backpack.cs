using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backpack
{
    private List<Item> items;
    private int space;

    public Backpack(int initialSpace)
    {
        space = initialSpace;
        items = new List<Item>(initialSpace);
    }

    public Item HasItem(int id)
    {
        foreach (Item item in items)
            if (item.Id == id)
                return item;
        return null;
    }
    public Item HasItem(string name)
    {
        foreach (Item item in items)
            if (item.Name == name)
                return item;
        return null;
    }
    public Item HasItem(ItemTag tag)
    {
        foreach (Item item in items)
            if (item.Tag == tag)
            {
                Debug.Log(item + "  " + item.Name);
                return item;
            }
        return null;
    }

    public bool TryAddItem(Item item)
    {
        if (items.Count < space)
        {
            items.Add(item.Clone());
            return true;
        }
        return false;
    }

    public void RemoveItem(Item item)
    {
        items.Remove(item);
    }

    public void SetSpace(int amount)
    {
        space = amount;
    }

    public List<Item> GetItems()
    {
        return items;
    }
}
