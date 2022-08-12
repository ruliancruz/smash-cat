using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockbackBehavior : MonoBehaviour
{
    //OnDamage Behavior
    [Header("Knockback Settings")]
    public byte knockDownResist;
    private byte currentKnockDownResist;
    public float fallSpeed;
}
