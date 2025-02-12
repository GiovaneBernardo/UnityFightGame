using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class GroundSpikes1Attack : AttackMonoBehaviour
{
    public int MaxSpikes = 8;
    public int SpikesToInstantiate = 4;
    public int CurrentSpikes = 0;
    public float TimeBetweenSpikes = 0.1f; // Seconds
    private float _lastSpikeTime;
    public bool Destroying = false;

    public List<GameObject> ObjectsToInstantiate = new List<GameObject>();

    public override void Start()
    {
        Data.Damage = 5.0f;
        Data.AreaOfEffect = 1.0f;
        Data.MaxEnemiesToHit = 1;
        Data.Cooldown = 1.0f;
        Data.Speed = 16.0f;
        Data.TimeToDisappear = 3.0f;
        Data.SpawnMetersAhead = 3.0f;
        Data.SpawnTime = Time.time;
        Data.AnimationName = "SimpleAnimation";
    }

    public void StartAnimation()
    {
        //Data.Animator.Play(Data.AnimationName, 1);
    }

    public void Update()
    {
        if (Destroying)
        {
            if(Time.time - _lastSpikeTime > TimeBetweenSpikes * 0.75f && CurrentSpikes >= 0)
            {
                Destroy(gameObject.transform.GetChild(CurrentSpikes).gameObject);
                CurrentSpikes--;
                _lastSpikeTime = Time.time;
            } else if(CurrentSpikes < 0)
            {
                base.Destroy();
            }
            return;
        }
        else if (Time.time - _lastSpikeTime > TimeBetweenSpikes)
        {
            SummonSpike();
            CurrentSpikes++;
            _lastSpikeTime = Time.time;
        }
        AttackCommonUpdate();
    }

    public void SummonSpike()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position + new Vector3(0.0f, 5.0f) + (transform.forward * CurrentSpikes * 3), new Vector3(0.0f, -1.0f, 0.0f), out hit, 6.0f, LayerMask.GetMask("Terrain"));
        GameObject newSpike = Instantiate(ObjectsToInstantiate[Random.Range(0, ObjectsToInstantiate.Count)], this.transform);
        newSpike.transform.position = hit.point + new Vector3(0.0f, 0.3f, 0.0f);
        newSpike.transform.eulerAngles = hit.normal;
    }

    public override void Destroy()
    {
        Destroying = true;
    }
}
