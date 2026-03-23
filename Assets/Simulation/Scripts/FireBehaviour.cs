using System;
using System.Collections;
using UnityEngine;

public class FireBehaviour : MonoBehaviour
{
    [SerializeField] private ParticleSystem fireParticles;
    [SerializeField] private ParticleSystem smokeParticles;

    [SerializeField] 
    private int maxFireParticles = 10000;
    public int MaxFireParticles => maxFireParticles;

    [SerializeField] private int countDownRate = 10;
    [SerializeField] private float maxScale = 4f;
    [SerializeField] private float minScale = 1f;
    [SerializeField] private AnimationCurve animationCurve;

    [SerializeField] private int minParticles = 20;

    /// <summary>
    /// Initialize.
    /// </summary>
    public void Initialize()
    {
        SimulationController.Instance.SetMaxParticles(
                fireParticles, MaxFireParticles);
        
        SimulationController.Instance.SetMaxParticles(
                smokeParticles, 0);
    }

    /// <summary>
    /// Collision triggers.
    /// </summary>
    /// <param name="other"></param>
    private void OnParticleCollision(GameObject other)
    {
        if (other.tag == "Rain")
        {
            float currentSize = fireParticles.particleCount / (float)MaxFireParticles;
            float curve = animationCurve.Evaluate(currentSize);
            float scale = Mathf.Lerp(minScale, maxScale, curve);
            int rate = (int)(countDownRate * scale);

            int count = fireParticles.particleCount <= minParticles ?
                        0 :
                        fireParticles.particleCount - rate;

            SimulationController.Instance.SetMaxParticles(fireParticles, count);

            int smokeParticlesCount = count >= MaxFireParticles * 0.5f ?
                                    MaxFireParticles - fireParticles.particleCount :
                                    count;

            SimulationController.Instance.SetMaxParticles(
                smokeParticles, smokeParticlesCount);

        }
    }
}
