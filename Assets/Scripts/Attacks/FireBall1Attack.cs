using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/FireBall1")]
public class FireBall1Attack : AttackBase
{

    public override void Start()
    {

    }

    public void StartAnimation()
    {
        //Data.Animator.Play(Data.AnimationName, 1);
    }

    public override void Update()
    {
        transform.position += transform.forward * (Speed * Time.deltaTime);

        AttackCommonUpdate();
    }

    public override void AfterHit(Transform enemy)
    {
        Destroy(gameObject);
    }
}
