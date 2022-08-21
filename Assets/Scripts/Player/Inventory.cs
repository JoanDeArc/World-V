using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

// Hold inventory data
public class Inventory : MonoBehaviour
{
    public InventoryUI InventoryUI;

    public delegate void OnItemChanged(ItemData data, int indexSlot, bool adding);
    public OnItemChanged onItemChangedCallback;

    public int space;
    public ItemData[] items;

    void Start()
    {
        items = new ItemData[space];

        InventoryUI.ConnectToInventory();
    }

    public async Task<bool> Open()
    {
        InventoryUI.Open();

        while (InventoryUI.IsOpen)
            await Task.Yield();

        return true;
    }

    public void Close()
    {
        InventoryUI.Close();
    }

    public async Task<ItemData> OpenChooseType(System.Type type)
    {
        int index = await InventoryUI.ShowChooseType(type);

        if (items.Length > index)
            return items[index];
        return null;
    }

    /*public Item HasItem(int id)
    {
        foreach (ItemData item in items)
            if (item.Id == id)
                return new GameObject(item.Name).AddComponent<Item>();
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
    }*/

    public bool HasEmptySpace()
    {
        for (int i = 0; i < space; i++)
        {
            if (items[i] == null)
            {
                return true;
            }
        }
        return false;
    }

    public bool TryAddItem(ItemData item)
    {
        if (HasEmptySpace())
        {
            int index;
            for (index = 0; index < space; index++)
                if (items[index] == null)
                {
                    items[index] = item;
                    break;
                }

            if (onItemChangedCallback != null)
                onItemChangedCallback.Invoke(item, index, true);
            return true;
        }
        return false;
    }

    public void RemoveItem(ItemData item)
    {
        int index;
        for (index = 0; index < space; index++)
            if (items[index] == item)
            {
                items[index] = null;
                break;
            }

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke(item, index, false);
    }

    public void SetSpace(int amount)
    {
        space = amount;
    }

    public List<ItemData> GetItems()
    {
        return new List<ItemData>(items);
    }
}
