using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundExplosion1Attack : AttackMonoBehaviour
{
    public override void Start()
    {
        Data.Damage = 5.0f;
        Data.AreaOfEffect = 1.0f;
        Data.MaxEnemiesToHit = 1;
        Data.Cooldown = 1.0f;
        Data.Speed = 16.0f;
        Data.TimeToDisappear = 1.0f;
        Data.SpawnMetersAhead = 3.0f;
        Data.SpawnTime = Time.time;
        Data.AnimationName = "GroundExplosion1Attack";
    }

    public void StartAnimation()
    {
        //Data.Animator.Play(Data.AnimationName, 1);
    }

    public void Update()
    {
        if (Time.time - Data.SpawnTime > Data.TimeToDisappear)
        {
            this.Destroy();
        }
    }

    public override void SetPosition(Transform centerTransform, Transform characterTransform)
    {
        transform.position = characterTransform.position;
    }
}
