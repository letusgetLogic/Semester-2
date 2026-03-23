using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

[BurstCompile]
public struct VelocityJob : IJobParallelFor
{
    public NativeArray<Vector3> Velocities;
    public NativeArray<Vector3> Directions;
    public NativeArray<float> Speeds;
    public float DeltaTime;

    public void Execute(int index)
    {
        Velocities[index] = Directions[index] * Speeds[index] * DeltaTime;
    }
}
