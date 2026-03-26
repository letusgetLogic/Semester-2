using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

[BurstCompile]
public struct CountdownJob : IJobParallelFor
{
    public NativeArray<float> Countdowns;
    public float DeltaTime;

    public void Execute(int index)
    {
        Countdowns[index] -= DeltaTime;
    }
}

