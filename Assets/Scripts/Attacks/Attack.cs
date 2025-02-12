using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public struct AttackData
{
    public float Damage;
    public float AreaOfEffect; // Meters
    public int MaxEnemiesToHit;
    public float Cooldown; // Seconds
    public float Speed; // Speed of the projectile, can be 0 if attack isnt a projectile
    public float TimeToDisappear; // Seconds
    public float SpawnTime;
    public float SpawnMetersAhead;
    public string AnimationName;
    public Animator Animator;
}

public class AttackMonoBehaviour : MonoBehaviour
{
    public AttackData Data;
    public virtual void Start() { }
    public void AttackCommonUpdate()
    {
        if(Time.time - Data.SpawnTime > Data.TimeToDisappear)
        {
            Destroy(); 
        }
    }

    public virtual void Destroy() {
        Destroy(gameObject);
    }
}


public class AttacksManager
{
    public static Dictionary<string, AttackMonoBehaviour> AllAttacks = new Dictionary<string, AttackMonoBehaviour>();
}
