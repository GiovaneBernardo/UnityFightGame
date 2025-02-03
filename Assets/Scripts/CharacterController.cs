using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public Transform CharacterFollowerTransform;
    private Animator animator;
    float velocity = 0.0f;
    public float acceleration = 0.1f;
    public float deceleration = 0.8f;
    int VelocityHash;

    void Start()
    {
        animator = GetComponent<Animator>();
        VelocityHash = Animator.StringToHash("Velocity");
    }


    void Update()
    {
        //if (Input.GetKey(KeyCode.W))
        //{
        //    if (Input.GetKey(KeyCode.LeftShift))
        //        animator.SetBool("IsRunning", true);
        //    else
        //    {
        //        animator.SetBool("IsRunning", false);
        //        animator.SetBool("IsWalking", true);
        //    }
        //}
        //else
        //{
        //    animator.SetBool("IsWalking", false);
        //}
        //
        //if (Input.GetKey(KeyCode.Space))
        //    animator.SetBool("IsJumping", true);
        //else
        //    animator.SetBool("IsJumping", false);

        bool pressingAnyMovementKey = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
        if (velocity < 1.0f && pressingAnyMovementKey)
        {
            velocity += Time.deltaTime * acceleration;
        }
        else if (velocity > 0.0f)
        {
            velocity -= Time.deltaTime * deceleration;
        }
        animator.SetFloat(VelocityHash, velocity);

        Vector3 direction = new Vector3(0.0f, 0.0f, 0.0f);

        int countActivated = 0;
        if (Input.GetKey(KeyCode.W))
        {
            direction.y += 0.0f;
            countActivated++;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            direction.y += -1.0f;
            countActivated++;
        }

        if (Input.GetKey(KeyCode.A))
        {
            direction.y -= 0.5f;
            countActivated++;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            if (direction.y < 0.0f)
                direction.y += -1.5f;
            else
                direction.y += 0.5f;
            countActivated++;
        }
        if (countActivated == 0)
            countActivated = 1;
        direction.y = direction.y / countActivated * 180.0f;

        this.transform.rotation = CharacterFollowerTransform.rotation * Quaternion.Euler(direction);

        if (!pressingAnyMovementKey)
        {
            this.transform.rotation = CharacterFollowerTransform.rotation;
        }

    }
}
