using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AttackMonoBehaviour : MonoBehaviour
{
    public AttackData Data = new AttackData();
    public virtual void Start() { }
    public void AttackCommonUpdate()
    {
        if(Time.time - Data.SpawnTime > Data.TimeToDisappear)
        {
            Destroy(); 
        }
    }

    public virtual void SetPosition(Transform centerTransform, Transform characterTransform)
    {
        transform.position = centerTransform.position + centerTransform.forward * Data.SpawnMetersAhead;
    }

    public virtual void Destroy() {
        Destroy(gameObject);
    }

}


public class AttacksManager
{
    public static Dictionary<string, AttackMonoBehaviour> AllAttacks = new Dictionary<string, AttackMonoBehaviour>();
}
