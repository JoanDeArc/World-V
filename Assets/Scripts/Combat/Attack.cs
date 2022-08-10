using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Attack : MonoBehaviour
{
    public string Name;
    public int Damage;
    public float Cooldown;

    public abstract bool IsRanged();
}
