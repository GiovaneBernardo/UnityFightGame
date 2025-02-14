using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public int Health = 50;
    public List<AttackData> EquippedAttacks = new List<AttackData>();

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
        if (Input.GetKey(KeyCode.G))
        {
            BlendTreeMovement.x = Mathf.Clamp(UpdateMovementAxis(BlendTreeMovement.x, KeyCode.A, -1.5f, 1.5f, 0.5f, GetAIMovements().x * Acceleration, true), -1.5f, 1.5f);
            BlendTreeMovement.y = Mathf.Clamp(UpdateMovementAxis(BlendTreeMovement.y, KeyCode.W, -1.5f, 1.5f, 0.5f, GetAIMovements().y * Acceleration, true), -1.5f, 1.5f);
        }

        _animator.SetFloat("XBlendWalk", BlendTreeMovement.x);
        _animator.SetFloat("YBlendWalk", BlendTreeMovement.y);
    }

    private Vector2 GetAIMovements()
    {
        Vector3 difference = new Vector3(0.0f, 0.0f, Vector3.Distance(Target.position, transform.position) - 0.5f);
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
        this.transform.LookAt(new Vector3(Target.position.x, transform.position.y, Target.position.z));
    }

    private void FixedUpdate()
    {
        CharacterControllerComponent.Move(RootMotion + new Vector3(0.0f, Gravity * Time.fixedDeltaTime, 0.0f));
        RootMotion = Vector3.zero;
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
}
