using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall1Attack : AttackMonoBehaviour
{

    public override void Start()
    {
        Data.Damage = 5.0f;
        Data.AreaOfEffect = 1.0f;
        Data.MaxEnemiesToHit = 1;
        Data.Cooldown = 1.0f;
        Data.Speed = 16.0f;
        Data.TimeToDisappear = 5.0f;
        Data.SpawnMetersAhead = 1.0f;
        Data.SpawnTime = Time.time;
        Data.AnimationName = "SimpleAnimation";
    }

    public void StartAnimation()
    {
        //Data.Animator.Play(Data.AnimationName, 1);
    }

    public void Update()
    {
        transform.position += transform.forward * (Data.Speed * Time.deltaTime);

        AttackCommonUpdate();
    }
}
