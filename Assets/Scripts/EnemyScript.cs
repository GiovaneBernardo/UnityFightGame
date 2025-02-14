using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public float MaxHealth = 50;
    public float CurrentHealth = 50;
    public List<AttackData> EquippedAttacks = new List<AttackData>();

    public GameObject HealthCanvasObject;

    public EnemySpawner OwnerSpawner;

    public Vector2 BlendTreeMovement = Vector2.zero;
    private Animator _animator;
    UnityEngine.CharacterController CharacterControllerComponent;
    public Transform Target;
    public Dictionary<int, Transform> PlayersTargetingMe = new Dictionary<int, Transform>();

    public float SprintingBoost = 2.0f;
    public float Acceleration = 2.0f;
    public float SprintDeceleration = 4.0f;
    public float WalkDeceleration = 1.0f;

    public Transform FootLeftTransform;
    public Transform FootRightTransform;

    private float LastJumpTime;
    private float JumpCooldown = 1.0f; // Seconds

    public float JumpHeight = 2.0f;
    public float Gravity = -9.81f;
    public Vector3 RootMotion;

    void Start()
    {
        _animator = GetComponent<Animator>();
        CharacterControllerComponent = GetComponent<UnityEngine.CharacterController>();
    }

    void Update()
    {
        float closest = float.MaxValue;
        Transform closestTransform = null;
        foreach (KeyValuePair<int, Transform> pair in PlayersTargetingMe)
        {
            if (Vector3.Distance(transform.position, pair.Value.position) < closest)
            {
                closestTransform = pair.Value;
            }
        }
        if (closestTransform != null)
            Target = closestTransform;

        if (Target)
        {
            BlendTreeMovement.x = Mathf.Clamp(UpdateMovementAxis(BlendTreeMovement.x, KeyCode.A, -1.5f, 1.5f, 0.5f, GetAIMovements().x * Acceleration, true), -1.5f, 1.5f);
            BlendTreeMovement.y = Mathf.Clamp(UpdateMovementAxis(BlendTreeMovement.y, KeyCode.W, -1.5f, 1.5f, 0.5f, GetAIMovements().y * Acceleration, true), -1.5f, 1.5f);

            CalculateAttack();
        }
        else
        {
            BlendTreeMovement = Vector2.zero;
        }
        _animator.SetFloat("XBlendWalk", BlendTreeMovement.x);
        _animator.SetFloat("YBlendWalk", BlendTreeMovement.y);
    }

    private Vector2 GetAIMovements()
    {
        Vector3 difference = new Vector3(0.0f, 0.0f, Vector3.Distance(Target.position, transform.position) - 1.5f);
        return new Vector2(difference.x, difference.z * 3.0f);
    }

    float UpdateMovementAxis(float velocity, KeyCode keyToPress, float min, float max, float maxNonSprint, float movementAcceleration, bool negative)
    {
        if (movementAcceleration > 0)
            negative = false;
        else
            negative = true;

        bool sprinting = Input.GetKey(KeyCode.LeftShift);
        sprinting = true;
        bool walking = ((!negative && velocity <= maxNonSprint) || (negative && velocity >= -maxNonSprint)) && movementAcceleration != 0.0f;
        bool walkingButNeedsToDecelerate = ((!negative && velocity > maxNonSprint) || (negative && velocity < -maxNonSprint)) && movementAcceleration != 0.0f;
        if (sprinting && movementAcceleration != 0.0f)
            velocity = Mathf.Clamp(velocity + Time.deltaTime * movementAcceleration * SprintingBoost, -max, max);
        else if (walking)
            velocity = Mathf.Clamp(velocity + Time.deltaTime * movementAcceleration, -maxNonSprint, maxNonSprint);
        else if (walkingButNeedsToDecelerate)
        {
            float newDeceleration = negative ? (Time.deltaTime * SprintDeceleration) : -(Time.deltaTime * SprintDeceleration);
            if (negative)
                velocity = Mathf.Min(velocity + newDeceleration, maxNonSprint);
            else
                velocity = Mathf.Max(velocity + newDeceleration, maxNonSprint);
        }
        else if (movementAcceleration == 0.0f)
        {
            bool velocityIsNegative = velocity <= 0.0f;
            float newDeceleration = velocityIsNegative ? (Time.deltaTime * WalkDeceleration) : -(Time.deltaTime * WalkDeceleration);
            if (velocityIsNegative)
                velocity = Mathf.Min(velocity + newDeceleration, 0.0f);
            else
                velocity = Mathf.Max(velocity + newDeceleration, 0.0f);
        }
        return velocity;
    }

    private void OnAnimatorMove()
    {
        RootMotion += _animator.deltaPosition;
    }

    private void FixedUpdate()
    {
        CharacterControllerComponent.Move(RootMotion + new Vector3(0.0f, Gravity * Time.fixedDeltaTime, 0.0f));
        RootMotion = Vector3.zero;
        if (Target)
        {
            this.transform.LookAt(new Vector3(Target.position.x, transform.position.y, Target.position.z));
        }
    }

    public void AddPlayerTargetingMe(Transform player)
    {
        if (!PlayersTargetingMe.ContainsKey(player.GetInstanceID()))
            PlayersTargetingMe.Add(player.GetInstanceID(), player);
    }

    public void RemovePlayerTargetingMe(Transform player)
    {
        PlayersTargetingMe.Remove(player.GetInstanceID());
    }

    public void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        if (CurrentHealth <= 0)
        {
            Die();
            return;
        }
        HealthCanvasObject.GetComponent<RectTransform>().sizeDelta = new Vector2((float)(100 * (CurrentHealth / MaxHealth)), 64);
    }

    public void TakeKnockback(Vector3 knockback)
    {

    }

    public void Die()
    {
        OwnerSpawner.KillEnemy(gameObject);
    }

    public void CalculateAttack()
    {
        if (EquippedAttacks.Count <= 0)
            return;

        SummonAttack(EquippedAttacks[Random.Range(0, EquippedAttacks.Count)].AnimationName);
    }

    private Dictionary<string, GameObject> _loadedPrefabs = new Dictionary<string, GameObject>();
    private Dictionary<string, float> _attacksCooldown = new Dictionary<string, float>();
    void SummonAttack(string name)
    {
        GameObject prefab;
        if (_loadedPrefabs.ContainsKey("AttackPrefabs/" + name))
            prefab = _loadedPrefabs["AttackPrefabs/" + name];
        else
        {
            prefab = Resources.Load<GameObject>("AttackPrefabs/" + name);
            _loadedPrefabs.Add("AttackPrefabs/" + name, prefab);
        }
        if (prefab != null)
        {
            if (!_attacksCooldown.ContainsKey(name))
                _attacksCooldown.Add(name, 0);
            prefab.GetComponent<AttackMonoBehaviour>().Start();
            bool cooldownIsOver = Time.time - _attacksCooldown[name] > prefab.GetComponent<AttackMonoBehaviour>().Data.Cooldown;
            if (!cooldownIsOver)
                return;
            _attacksCooldown[name] = Time.time;
            GameObject obj = Instantiate(prefab, new Vector3(0.0f, 0.0f, 0.0f), transform.rotation);
            AttackData attackData = prefab.GetComponent<AttackMonoBehaviour>().Data;
            obj.GetComponent<AttackMonoBehaviour>().SetPosition(transform, transform);
            obj.GetComponent<AttackMonoBehaviour>().OwnerCharacter = gameObject;
        }
    }
}
