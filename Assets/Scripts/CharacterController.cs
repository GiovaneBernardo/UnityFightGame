using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    public Transform CameraCenterTargetTransform;
    public Transform characterFollowerTransform;
    private Animator animator;
    public float sprintingBoost = 2.0f;
    public float acceleration = 2.0f;
    public float sprintDeceleration = 4.0f;
    public float walkDeceleration = 1.0f;
    int velocityHash;
    int blendYHash;

    public Transform footLeftTransform;
    public Transform footRightTransform;

    public Vector2 blendTreeMovement = Vector2.zero;

    private float lastJumpTime;
    private float jumpCooldown = 1.0f; // Seconds

    public float jumpHeight = 2.0f;
    public float gravity = -9.81f;
    Vector3 rootMotion;
    Vector3 velocity;

    Dictionary<string, GameObject> loadedPrefabs = new Dictionary<string, GameObject>();

    Dictionary<string, GameObject> possibleAttacks = new Dictionary<string, GameObject>();
    Dictionary<string, float> attacksCooldown = new Dictionary<string, float>();
    List<GameObject> activeAttacks = new List<GameObject>();

    bool lockMovement;

    UnityEngine.CharacterController characterControllerComponent;

    void Start()
    {
        GameObject[] attacks = Resources.LoadAll<GameObject>("Attacks");

        foreach (GameObject attack in attacks)
        {
            possibleAttacks.Add(attack.GetComponent<AttackMonoBehaviour>().Data.AnimationName, attack);
        }

        animator = GetComponent<Animator>();
        characterControllerComponent = GetComponent<UnityEngine.CharacterController>();

        velocityHash = Animator.StringToHash("Velocity");
        blendYHash = Animator.StringToHash("BlendY");
    }

    RaycastHit[] GetFootsRaycast()
    {
        RaycastHit hit1;
        Physics.Raycast(footLeftTransform.position, new Vector3(0.0f, -1.0f, 0.0f), out hit1, 0.3f);
        RaycastHit hit2;
        Physics.Raycast(footRightTransform.position, new Vector3(0.0f, -1.0f, 0.0f), out hit2, 0.3f);
        return new RaycastHit[] { hit1, hit2 };
    }

    bool IsAnyFootInGround()
    {
        bool hit1 = Physics.Raycast(footLeftTransform.position, new Vector3(0.0f, -1.0f, 0.0f), 1.5f);
        bool hit2 = Physics.Raycast(footRightTransform.position, new Vector3(0.0f, -1.0f, 0.0f), 1.5f);
        return hit1 || hit2;
    }

    void Update()
    {
        HandleInputs();
        UpdateAttacks();
    }

    void LateUpdate()
    {
        UpdateMovement();
    }

    void HandleInputs()
    {
        if (Input.GetMouseButton(0))
        {
            animator.SetTrigger("Attack1");
        }
        if (Input.GetKey(KeyCode.E))
        {
            SummonAttack("FireBall1");
        }
        if (Input.GetKey(KeyCode.Q))
        {
            SummonAttack("GroundSpikes1");
        }
    }

    void SummonAttack(string name)
    {
        GameObject prefab;
        if (loadedPrefabs.ContainsKey("AttackPrefabs/" + name))
            prefab = loadedPrefabs["AttackPrefabs/" + name];
        else
        {
            prefab = Resources.Load<GameObject>("AttackPrefabs/" + name);
            loadedPrefabs.Add("AttackPrefabs/" + name, prefab);
        }
        if (prefab != null)
        {
            if (!attacksCooldown.ContainsKey(name))
                attacksCooldown.Add(name, 0);
            prefab.GetComponent<AttackMonoBehaviour>().Start();
            bool cooldownIsOver = Time.time - attacksCooldown[name] > prefab.GetComponent<AttackMonoBehaviour>().Data.Cooldown;
            if (!cooldownIsOver)
                return;
            attacksCooldown[name] = Time.time;
            GameObject obj = Instantiate(prefab, new Vector3(0.0f, 0.0f, 0.0f), CameraCenterTargetTransform.rotation);
            AttackData attackData = prefab.GetComponent<AttackMonoBehaviour>().Data;
            Debug.Log(attackData.SpawnMetersAhead);
            obj.transform.position = CameraCenterTargetTransform.position + CameraCenterTargetTransform.forward * attackData.SpawnMetersAhead;
            if (obj)
            {
                //obj.AddComponent<FireBall1Attack>();
                //obj.GetComponent<FireBall1Attack>().StartAnimation();
            }
        }
    }

    private void UpdateAttacks()
    {

    }

    private void UpdateMovement()
    {
        if (IsAnyFootInGround() && !animator.GetCurrentAnimatorStateInfo(0).IsName("jump"))
        {
            animator.SetBool("IsGrounded", true);
            animator.SetBool("IsFalling", false);
        }
        else
        {
            animator.SetBool("IsGrounded", false);
            animator.SetBool("IsFalling", true);
        }

        bool isJumpCooldownOver = Time.time - lastJumpTime > jumpCooldown;
        if (Input.GetButtonDown("Jump") && isJumpCooldownOver)
        {
            lastJumpTime = Time.time;
            characterControllerComponent.Move(new Vector3(0.0f, 5.0f, 0.0f));
            animator.SetBool("IsJumping", true);
        }
        else
        {
            animator.SetBool("IsJumping", false);
        }

        bool pressingAnyMovementKey = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
        if (pressingAnyMovementKey)
        {
            animator.SetBool("RunMovementBlendTree", true);
        }
        else
        {
            animator.SetBool("RunMovementBlendTree", false);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            animator.SetTrigger("Jump");
            animator.ResetTrigger("Landed");
        }
        else
        {
            animator.ResetTrigger("Jump");
            animator.SetTrigger("Landed");
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            lockMovement = !lockMovement;
        }

        if (lockMovement)
        {
            animator.SetBool("RunMovementBlendTree", true);
        }
        else
        {
            blendTreeMovement.x = Mathf.Clamp(UpdateMovementAxis(blendTreeMovement.x, KeyCode.A, -1.5f, 1.5f, 0.5f, Input.GetAxis("Horizontal") * acceleration, true), -1.5f, 1.5f);
            blendTreeMovement.y = Mathf.Clamp(UpdateMovementAxis(blendTreeMovement.y, KeyCode.W, -1.5f, 1.5f, 0.5f, Input.GetAxis("Vertical") * acceleration, true), -1.5f, 1.5f);
        }
        animator.SetFloat("XBlendWalk", blendTreeMovement.x);
        animator.SetFloat("YBlendWalk", blendTreeMovement.y);
        Vector3 direction = new Vector3(0.0f, 0.0f, 0.0f);

        this.transform.rotation = characterFollowerTransform.rotation;
    }

    float UpdateMovementAxis(float velocity, KeyCode keyToPress, float min, float max, float maxNonSprint, float movementAcceleration, bool negative)
    {
        if (movementAcceleration > 0)
            negative = false;
        else
            negative = true;

        bool sprinting = Input.GetKey(KeyCode.LeftShift);
        bool walking = ((!negative && velocity <= maxNonSprint) || (negative && velocity >= -maxNonSprint)) && movementAcceleration != 0.0f;
        bool walkingButNeedsToDecelerate = ((!negative && velocity > maxNonSprint) || (negative && velocity < -maxNonSprint)) && movementAcceleration != 0.0f;
        if (sprinting && movementAcceleration != 0.0f)
            velocity = Mathf.Clamp(velocity + Time.deltaTime * movementAcceleration * sprintingBoost, -max, max);
        else if (walking)
            velocity = Mathf.Clamp(velocity + Time.deltaTime * movementAcceleration, -maxNonSprint, maxNonSprint);
        else if (walkingButNeedsToDecelerate)
        {
            float newDeceleration = negative ? (Time.deltaTime * sprintDeceleration) : -(Time.deltaTime * sprintDeceleration);
            if (negative)
                velocity = Mathf.Min(velocity + newDeceleration, maxNonSprint);
            else
                velocity = Mathf.Max(velocity + newDeceleration, maxNonSprint);
        }
        else if (movementAcceleration == 0.0f)
        {
            bool velocityIsNegative = velocity <= 0.0f;
            float newDeceleration = velocityIsNegative ? (Time.deltaTime * walkDeceleration) : -(Time.deltaTime * walkDeceleration);
            if (velocityIsNegative)
                velocity = Mathf.Min(velocity + newDeceleration, 0.0f);
            else
                velocity = Mathf.Max(velocity + newDeceleration, 0.0f);
        }
        return velocity;
    }

    private void OnAnimatorMove()
    {
        rootMotion += animator.deltaPosition;
    }

    private void FixedUpdate()
    {
        characterControllerComponent.Move(rootMotion + new Vector3(0.0f, gravity * Time.fixedDeltaTime, 0.0f));
        rootMotion = Vector3.zero;
    }
}
