using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttack : Attack
{

    public GameObject Projectile;
    public float Speed;
    public float Range;

    public override bool IsRanged()
    {
        return true;
    }
}
