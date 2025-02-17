using UnityEngine;

public class AttackHandler : MonoBehaviour
{
    public AttackBase AttackBehaviour;
    public void Start()
    {
        AttackBehaviour = AttackBehaviour.Clone();
        AttackBehaviour.gameObject = gameObject;
        AttackBehaviour.transform = transform;
        AttackBehaviour.SpawnTime = Time.time;
        AttackBehaviour.Start();
    }

    void Update()
    {
        AttackBehaviour.Update();
    }

    private void OnTriggerEnter(Collider other)
    {
        AttackBehaviour.OnTriggerEnter(other);
    }

    private void OnDestroy()
    {
        AttackBehaviour.Destroy();
    }
}
