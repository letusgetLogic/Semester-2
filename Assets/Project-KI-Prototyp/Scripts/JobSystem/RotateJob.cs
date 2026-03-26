using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;

[BurstCompile]
public struct RotateJob : IJobParallelForTransform
{
    public NativeArray<WhatAmIDoingNext> CurrentActions;
    [ReadOnly] public NativeArray<Quaternion> TargetRotations;
    [ReadOnly] public NativeArray<float> Speeds;
    [ReadOnly] public float DeltaTime;

    public void Execute(int index, TransformAccess transform)
    {
        if (CurrentActions[index] != WhatAmIDoingNext.LookAround)
        {
            return;
        }
        float angle = Quaternion.Angle(transform.rotation, TargetRotations[index]);

        if (angle < 0.1f)
        {
            CurrentActions[index] = WhatAmIDoingNext.Nothing;
            return;
        }
        transform.rotation =
            Quaternion.Slerp(transform.rotation, TargetRotations[index], Speeds[index] * DeltaTime);
    }
}
