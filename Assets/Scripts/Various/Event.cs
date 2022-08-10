using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Event
{
    public List<GameObject> Characters;

    [TextArea]
    public string Commands;
}
