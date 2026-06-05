using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateParticles : MonoBehaviour
{
    [SerializeField]
    ParticleSystem[] particleSystems;
    private void OnEnable()
    {
        foreach (ParticleSystem particle in particleSystems)
        {
            particle.Play();
        }
    }
}
