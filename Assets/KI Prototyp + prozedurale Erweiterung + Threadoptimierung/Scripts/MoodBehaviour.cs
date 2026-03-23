using System;
using UnityEngine;

public class MoodBehaviour : MonoBehaviour
{
    public enum MoodType
    {
        ChaseWhenBeingAttacked,
        ChaseWhenPlayerDetected,
    }

    public MoodType CurrentMood;

    private void Start()
    {
        CurrentMood =  GetMood();
    }

    /// <summary>
    /// Gets a random mood for the chase system of enemy.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private MoodType GetMood()
    {
        int rnd = UnityEngine.Random.Range(0, Enum.GetNames(typeof(MoodType)).Length);

        switch (rnd)
        {
            case 0:
                return MoodType.ChaseWhenBeingAttacked;
            case 1:
                return MoodType.ChaseWhenPlayerDetected;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

}

