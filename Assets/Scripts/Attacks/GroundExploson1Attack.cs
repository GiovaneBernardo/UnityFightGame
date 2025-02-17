using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/GroundExplosion1")]
public class GroundExplosion1Attack : AttackBase
{

    public void StartAnimation()
    {
        //Data.Animator.Play(Data.AnimationName, 1);
    }

    public override void Update()
    {
        if (Time.time - SpawnTime > TimeToDisappear)
        {
            this.Destroy();
        }
    }

    public override void SetPosition(Transform centerTransform, Transform characterTransform)
    {
        transform.position = characterTransform.position;
    }
}
