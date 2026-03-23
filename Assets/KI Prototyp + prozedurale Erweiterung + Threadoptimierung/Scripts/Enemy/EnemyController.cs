using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private DetectorPlayer detectorAura;
    public DetectorPlayer DetectorAura => detectorAura;

    [SerializeField]
    private DetectorAttackRange attackArea;
    public DetectorAttackRange AttackArea => attackArea;

    [SerializeField]
    private float rotationSpeed = 8f;
    public float RotationSpeed => rotationSpeed;

    [SerializeField]
    private float walkSpeed = 100f;
    public float WalkSpeed => walkSpeed;

    [SerializeField] private GameObject exclamationMark;
    [SerializeField] private GameObject[] arms;
    [SerializeField] private GameObject body;

    public Rigidbody RB => rb;
    private Rigidbody rb;
    private NavMeshAgent agent;

    public Quaternion TargetRotation => targetRotation;
    private Quaternion targetRotation;

    public WhatAmIDoingNext CurrentAction { get; set; }
    public float Countdown { get; set; } = 0;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        ShowExclamationMark(false);
    }

    /// <summary>
    /// Set the transform rotation.
    /// </summary>
    /// <param name="deltaTime"></param>
    public void Rotate(float deltaTime)
    {
        float angle = Quaternion.Angle(transform.rotation, targetRotation);

        if (angle < 0.1f)
        {
            CurrentAction = WhatAmIDoingNext.Nothing;
            return;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * deltaTime);
    }

    /// <summary>
    /// Set the rigidbody linear velocity.
    /// </summary>
    /// <param name="deltaTime"></param>
    public void MoveWithSpeed(float deltaTime)
    {
        rb.linearVelocity = transform.forward * walkSpeed * deltaTime;
    }

    /// <summary>
    /// MoveToPlayer is called to move the enemy towards the player or to stop.
    /// </summary>
    /// <param name="playerController"></param>
    public void MoveToPlayer(PlayerController playerController, bool value)
    {
        if (agent == null || playerController == null)
            return;

        if (value) // is moving
        {
            if (agent.isOnNavMesh)
            {
                agent.destination = playerController.transform.position;
                agent.isStopped = false;
            }
        }
        else // is stopping
        {
            agent.isStopped = true;
        }
    }

    /// <summary>
    /// Set the random target rotation.
    /// </summary>
    public void SetRandomRotation()
    {
        int rndAngle = Random.Range(-90, 90);
        float targetAngle = transform.eulerAngles.y + rndAngle;
        targetRotation = Quaternion.Euler(0, targetAngle, 0);
    }

    /// <summary>
    /// Show the exclamation mark, when player is detected.
    /// </summary>
    /// <param name="value"></param>
    public void ShowExclamationMark(bool value)
    {
        if (exclamationMark != null)
        {
            exclamationMark.SetActive(value);
        }
        else
        {
            Debug.LogWarning("Exclamation mark GameObject is not assigned.");
        }
    }

    /// <summary>
    /// Raise the arms, when player is detected.
    /// </summary>
    /// <param name="value"></param>
    public void RaiseArms(bool value)
    {
        foreach (GameObject arm in arms)
        {
            if (value) // raise arms
            {
                arm.transform.localRotation = Quaternion.Euler(90, 0, 0);
            }
            else // let arms down
            {
                arm.transform.localRotation = Quaternion.Euler(180, 0, 0);
            }
        }
    }

    /// <summary>
    /// Tilt the body to animate the attack.
    /// </summary>
    /// <param name="value"></param>
    public void TiltBodyWhileAttacking(bool value)
    {
        if (value) // tilt body
        {
            body.transform.localRotation = Quaternion.Euler(30, 0, 0);
        }
        else // stand straight
        {
            body.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

}

