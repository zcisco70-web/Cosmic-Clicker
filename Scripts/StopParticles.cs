using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopParticles : MonoBehaviour
{
    [SerializeField]
    private List<ParticleSystem> particles;

    public void StopCreatingParticles()
    {
        foreach (ParticleSystem p in particles)
        {
            var mainModule = p.main;
            mainModule.loop = false;
        }
    }
}

