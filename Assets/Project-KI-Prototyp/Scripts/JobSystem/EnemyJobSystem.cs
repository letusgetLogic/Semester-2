using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Jobs;

[BurstCompile]
public class EnemyJobSystem : MonoBehaviour
{
    public static EnemyJobSystem Instance { get; private set; }

    [SerializeField]
    private bool useCountdownJob = true;
    public bool UseCountdownJob => useCountdownJob;
    public List<EnemyController> CountdownEnemyControllers => countdownEnemyControllers;
    private List<EnemyController> countdownEnemyControllers = new List<EnemyController>();
    private NativeArray<float> countdowns;
    private JobHandle countdownJobHandle;

    [SerializeField]
    private bool useRotationJob = true;
    public bool UseRotationJob => useRotationJob;
    public List<EnemyController> RotateEnemyControllers => rotateEnemyControllers;
    private List<EnemyController> rotateEnemyControllers = new List<EnemyController>();
    private NativeArray<Quaternion> targetRotations;
    private NativeArray<float> rotateSpeeds;
    private NativeArray<WhatAmIDoingNext> currentActions;
    private TransformAccessArray rotateTransformAccessArray;
    private JobHandle rotateJobHandle;

    [SerializeField]
    private bool useForwardJob = true;
    public bool UseForwardJob => useForwardJob;
    public List<EnemyController> ForwardEnemyControllers => forwardEnemyControllers;
    private List<EnemyController> forwardEnemyControllers = new List<EnemyController>();
    private NativeArray<Vector3> velocities;
    private NativeArray<Vector3> directions;
    private NativeArray<float> walkSpeeds;
    private JobHandle velocityJobHandle;

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);

        Instance = this;

        int maxWorkers = JobsUtility.JobWorkerMaximumCount;
        JobsUtility.JobWorkerCount = maxWorkers -1;
    }
    private void Update()
    {
        if (useRotationJob)
            UpdateRotateJob();

        if (useForwardJob)
            UpdateForwardJob();

        if (useCountdownJob)
            UpdateCountdownJob();
    }

    private void LateUpdate()
    {
        if (useRotationJob)
        {
            rotateJobHandle.Complete();
            SetCurrentAction();
        }

        if (useForwardJob)
        {
            velocityJobHandle.Complete();
            SetRigidbodyVelocity();
        }

        if (useCountdownJob)
        {
            countdownJobHandle.Complete();
            SetCountdown();
        }
    }

    private void OnDestroy()
    {
        if (countdowns.IsCreated)
            countdowns.Dispose();

        if (targetRotations.IsCreated)
            targetRotations.Dispose();
        if (rotateSpeeds.IsCreated)
            rotateSpeeds.Dispose();
        if (currentActions.IsCreated)
            currentActions.Dispose();
        if (rotateTransformAccessArray.isCreated)
            rotateTransformAccessArray.Dispose();

        if (velocities.IsCreated)
            velocities.Dispose();
        if (walkSpeeds.IsCreated)
            walkSpeeds.Dispose();
        if (directions.IsCreated)
            directions.Dispose();

    }

    /// <summary>
    /// Add a enemy controller to the list and assign a new list.
    /// </summary>
    /// <param name="controllers"></param>
    /// <param name="controller"></param>
    public void AddEnemyToList(List<EnemyController> controllers, EnemyController controller)
    {
        controllers.Add(controller);
        switch (controllers)
        {
            case var a when a == rotateEnemyControllers:
                NewRotateList();
                break;

            case var b when b == forwardEnemyControllers:
                NewForwardList();
                break;

            case var c when c == countdownEnemyControllers:
                NewCountdownList();
                break;
        }
    }

    /// <summary>
    /// Remove a enemy controller from the list and assign a new list.
    /// </summary>
    /// <param name="controllers"></param>
    /// <param name="controller"></param>
    public void RemoveEnemyFromList(List<EnemyController> controllers, EnemyController controller)
    {
        switch (controllers)
        {
            case var a when a == rotateEnemyControllers:
                var rotatedEnemy = rotateEnemyControllers.Find(z => z == controller);
                if (rotatedEnemy == null)
                    break;
                rotateEnemyControllers.Remove(rotatedEnemy);
                NewRotateList();
                break;

            case var b when b == forwardEnemyControllers:
                var forwardEnemy = forwardEnemyControllers.Find(z => z == controller);
                if (forwardEnemy == null)
                    break;
                forwardEnemyControllers.Remove(forwardEnemy);
                NewForwardList();
                break;

            case var c when c == countdownEnemyControllers:
                var countdownEnemy = countdownEnemyControllers.Find(z => z == controller);
                if (countdownEnemy == null)
                    break;
                countdownEnemyControllers.Remove(countdownEnemy);
                NewCountdownList();
                break;
        }
    }

    /// <summary>
    /// Assign a new list, which contains to be rotated enemy controllers.
    /// </summary>
    private void NewRotateList()
    {
        int amount = rotateEnemyControllers.Count;
        targetRotations = new NativeArray<Quaternion>(amount, Allocator.Persistent);
        rotateSpeeds = new NativeArray<float>(amount, Allocator.Persistent);
        currentActions = new NativeArray<WhatAmIDoingNext>(amount, Allocator.Persistent);
        rotateTransformAccessArray = new TransformAccessArray(amount);

        for (int i = 0; i < amount; i++)
        {
            targetRotations[i] = rotateEnemyControllers[i].TargetRotation;
            rotateSpeeds[i] = rotateEnemyControllers[i].RotationSpeed;
            currentActions[i] = rotateEnemyControllers[i].CurrentAction;
            rotateTransformAccessArray.Add(rotateEnemyControllers[i].transform);
        }
    }

    /// <summary>
    /// Assign a new list, which contains to be moved forward enemy controllers.
    /// </summary>
    private void NewForwardList()
    {
        int amount = forwardEnemyControllers.Count;
        velocities = new NativeArray<Vector3>(amount, Allocator.Persistent);
        directions = new NativeArray<Vector3>(amount, Allocator.Persistent);
        walkSpeeds = new NativeArray<float>(amount, Allocator.Persistent);

        for (int i = 0; i < amount; i++)
        {
            directions[i] = forwardEnemyControllers[i].transform.forward;
            walkSpeeds[i] = forwardEnemyControllers[i].WalkSpeed;
        }
    }

    /// <summary>
    /// Assign a new list, which contains to be counted down enemy controllers.
    /// </summary>
    private void NewCountdownList()
    {
        int amount = countdownEnemyControllers.Count;
        countdowns = new NativeArray<float>(amount, Allocator.Persistent);
    }

    /// <summary>
    /// Update the rotate job.
    /// </summary>
    private void UpdateRotateJob()
    {
        if (rotateEnemyControllers.Count == 0)
            return;

        RotateJob rotateJob = new RotateJob
        {
            CurrentActions = currentActions,
            TargetRotations = targetRotations,
            Speeds = rotateSpeeds,
            DeltaTime = Time.deltaTime,
        };
        rotateJobHandle = rotateJob.Schedule(rotateTransformAccessArray);
    }

    /// <summary>
    /// Update the move forward job.
    /// </summary>
    private void UpdateForwardJob()
    {
        if (forwardEnemyControllers.Count == 0)
            return;

        VelocityJob velocityJob = new VelocityJob
        {
            Velocities = velocities,
            Directions = directions,
            Speeds = walkSpeeds,
            DeltaTime = Time.deltaTime,
        };
        velocityJobHandle = velocityJob.Schedule(forwardEnemyControllers.Count, 64);
    }

    /// <summary>
    /// Update the countdown job.
    /// </summary>
    private void UpdateCountdownJob()
    {
        if (countdownEnemyControllers.Count == 0)
            return;

        for (int i = 0; i < countdownEnemyControllers.Count; i++)
        {
            countdowns[i] = countdownEnemyControllers[i].Countdown;
        }

        CountdownJob countdownJob = new CountdownJob
        {
            Countdowns = countdowns,
            DeltaTime = Time.deltaTime
        };

        countdownJobHandle = countdownJob.Schedule(countdownEnemyControllers.Count, 64);
    }

    /// <summary>
    /// Update the current action of enemy controller.
    /// </summary>
    private void SetCurrentAction()
    {
        for (int i = 0; i < rotateEnemyControllers.Count; i++)
        {
            rotateEnemyControllers[i].CurrentAction = currentActions[i];
        }
    }

    /// <summary>
    /// Update the countdown of enemy controller.
    /// </summary>
    private void SetCountdown()
    {
        for (int i = 0; i < countdownEnemyControllers.Count; i++)
        {
            countdownEnemyControllers[i].Countdown = countdowns[i];
        }
    }

    /// <summary>
    /// Move enemies with the rigidbody linear velocitites.
    /// </summary>
    private void SetRigidbodyVelocity()
    {
        for (int i = 0; i < forwardEnemyControllers.Count; i++)
        {
            forwardEnemyControllers[i].RB.linearVelocity = velocities[i];
        }
    }
}
