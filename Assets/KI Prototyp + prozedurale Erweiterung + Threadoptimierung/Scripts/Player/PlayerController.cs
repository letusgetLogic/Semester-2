using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using System;

public class PlayerController : MonoBehaviour
{
    private PlayerAction input;
    private NavMeshAgent agent;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] ParticleSystem clickEffect;

    private float lookRotationSpeed = 8f;
    private ParticleSystem currentClickEffect = null;
    private bool isMoving = false;

    private void Awake()
    {
        input = new PlayerAction();
        agent = GetComponent<NavMeshAgent>();
        AssignInput();
    }

    private void OnEnable()
    {
        input.Enable();
    }
    private void Update()
    {
        if (!isMoving)
            return;

        FaceTarget();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    /// <summary>
    /// Assigns the input actions to the player controller.
    /// </summary>
    private void AssignInput()
    {
        input.Main.Move.performed += ctx => ClickToMove();
    }

    /// <summary>
    /// Calls the NavMeshAgent to move to the clicked position on the ground.
    /// </summary>
    private void ClickToMove()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(
            Mouse.current.position.ReadValue()), out hit, 100f, groundLayer))
        {
            agent.destination = hit.point;
            isMoving = true;

            if (clickEffect != null)
            {
                if (currentClickEffect != null)
                {
                    Destroy(currentClickEffect.gameObject);
                }

                currentClickEffect = Instantiate(clickEffect, hit.point + new Vector3(0, 0.1f, 0),
                    clickEffect.transform.rotation);
            }
        }
    }

    /// <summary>
    /// Reaches the target destination of the NavMeshAgent and destroys the click effect if it exists.
    /// </summary>
    private void ReachTarget()
    {
        if (currentClickEffect != null)
        {
            Destroy(currentClickEffect.gameObject);
            currentClickEffect = null; // Reset the effect after reaching the destination
        }
    }

    /// <summary>
    /// Faces the player towards the target destination of the NavMeshAgent.
    /// </summary>
    private void FaceTarget()
    {
        Vector3 moveDirection = (agent.destination - transform.position).normalized;
        
        if ((agent.destination - transform.position).magnitude < agent.baseOffset + 0.1f)
        {
            ReachTarget();
            isMoving = false;
            return; // Do not rotate if the agent is close to the destination
        }

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0, moveDirection.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, lookRotationSpeed * Time.deltaTime);
    }
}
