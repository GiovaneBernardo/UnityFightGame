using UnityEngine;

[System.Serializable]
public class AttackData
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