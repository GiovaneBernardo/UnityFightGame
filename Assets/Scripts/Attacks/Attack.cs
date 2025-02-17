using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/AttackBase")]
public class AttackBase : ScriptableObject
{
    public float Damage;
    public float AreaOfEffect; // Meters
    public int MaxEnemiesToHit;
    public float Cooldown; // Seconds
    public float Speed; // Speed of the projectile, can be 0 if attack isnt a projectile
    public float TimeToDisappear; // Seconds
    public float SpawnMetersAhead;
    public string AnimationName;
    public GameObject Prefab;
    [HideInInspector]
    public Animator Animator;
    [HideInInspector]
    public float SpawnTime;

    [HideInInspector]
    public GameObject gameObject;
    [HideInInspector]
    public Transform transform;
    [HideInInspector]
    public GameObject CasterGameObject;
    [HideInInspector]
    public bool CasterIsPlayer = false;


    public AttackBase Clone()
    {
        return Instantiate(this);
    }

    //[SerializeField]
    //public AttackData Data = new AttackData();
    public virtual void Start()
    {
        SpawnTime = Time.time;
    }
    public virtual void Update() { }

    public void AttackCommonUpdate()
    {
        if (Time.time - SpawnTime > TimeToDisappear)
        {
            Destroy();
        }
    }

    public virtual void SetPosition(Transform centerTransform, Transform characterTransform)
    {
        transform.position = centerTransform.position + centerTransform.forward * SpawnMetersAhead;
    }

    public virtual void Destroy()
    {
        GameObject.Destroy(gameObject);
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (CompareTags(other.gameObject) && other.gameObject != CasterGameObject && !other.GetComponent<Transform>().IsChildOf(transform))
        {
            HitEnemy(other.transform);
        }
    }

    public virtual void HitEnemy(Transform enemy)
    {
        if (!enemy)
            return;

        if (enemy.tag == "AIEnemy")
        {
            EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
            if (!enemyScript)
                return;
            enemyScript.TakeDamage(Damage);
            if (enemy != null)
            {
                enemyScript.TakeKnockback(new Vector3(0.0f, 0.25f, -0.1f));
                AfterHit(enemy);

                enemyScript.Target = CasterGameObject.transform;
            }
        } else if(enemy.tag == "Player")
        {
            CharacterController enemyScript = enemy.GetComponent<CharacterController>();
            if (!enemyScript)
                return;
            enemyScript.TakeDamage(Damage);
            if (enemy != null)
            {
                enemyScript.TakeKnockback(new Vector3(0.0f, 0.25f, -0.1f));
                AfterHit(enemy);
            }
        }
    }

    public virtual bool CompareTags(GameObject obj)
    {
        if (CasterIsPlayer)
            return obj.tag == "AIEnemy";
        else
            return obj.tag == "AIEnemy" || obj.tag == "Player";
    }

    public virtual void AfterHit(Transform enemy) { }
}


public class AttacksManager
{
    public static Dictionary<string, AttackBase> AllAttacks = new Dictionary<string, AttackBase>();
}
