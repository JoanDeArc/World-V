using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string Name;
    public int Id;
    public ItemTag Tag;

    public Item Clone()
    {
        return (Item)MemberwiseClone();
    }
}
