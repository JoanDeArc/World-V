using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : Attack
{
    public ParticleSystem VisualEffect;

    public override bool IsRanged()
    {
        return false;
    }
}
