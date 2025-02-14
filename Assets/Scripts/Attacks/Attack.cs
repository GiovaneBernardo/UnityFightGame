using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AttackMonoBehaviour : MonoBehaviour
{
    public GameObject OwnerCharacter;
    public AttackData Data = new AttackData();
    public virtual void Start() { }
    public void AttackCommonUpdate()
    {
        if (Time.time - Data.SpawnTime > Data.TimeToDisappear)
        {
            Destroy();
        }
    }

    public virtual void SetPosition(Transform centerTransform, Transform characterTransform)
    {
        transform.position = centerTransform.position + centerTransform.forward * Data.SpawnMetersAhead;
    }

    public virtual void Destroy()
    {
        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "AIEnemy")
        {
            HitEnemy(other.transform);
        }
    }

    public virtual void HitEnemy(Transform enemy)
    {
        if (!enemy)
            return;

        EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
        enemyScript.TakeDamage(Data.Damage);
        if (enemy != null)
        {
            enemyScript.TakeKnockback(new Vector3(0.0f, 0.25f, -0.1f));
            AfterHit(enemy);

            enemyScript.Target = OwnerCharacter.transform;
        }
    }

    public virtual void AfterHit(Transform enemy) { }
}


public class AttacksManager
{
    public static Dictionary<string, AttackMonoBehaviour> AllAttacks = new Dictionary<string, AttackMonoBehaviour>();
}
