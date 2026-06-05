using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class DissolvingController : MonoBehaviour
{
    public MeshRenderer MeshRenderer;
    public VisualEffect VFXGraph;
    public float dissolveRate = 0.05f;
    public float refreshRate = 0.05f;
    public float dieDelay = 0.25f;

    [SerializeField]
    private Material[] dissolveMaterials;

    void Awake()
    {
        if (VFXGraph != null)
        {
            VFXGraph.Stop();
            VFXGraph.gameObject.SetActive(false);
        }

        if (MeshRenderer != null)
            dissolveMaterials = MeshRenderer.materials;
    }

    public void Create()
    {
        StartCoroutine(CreateCo());
    }

    public void Dissolve()
    {
        StartCoroutine(DissolveCo());
    }
    IEnumerator DissolveCo()
    {
        if (VFXGraph != null)
        {
            VFXGraph.gameObject.SetActive(true);
            VFXGraph.Play();
        }
        float counter = 0;

        if (dissolveMaterials.Length > 0)
        {
            while (dissolveMaterials[0].GetFloat("DissolveAmount_") < 1)
            {
                counter += dissolveRate;
                for (int i = 0; i < dissolveMaterials.Length; i++)
                    dissolveMaterials[i].SetFloat("DissolveAmount_", counter);
                yield return new WaitForSeconds(refreshRate);
            }
        }
        else
        {
            //Debug.Log("No Dissolving Material assigned to model.");
            yield break;
        }
        gameObject.SetActive(false);
    }

    IEnumerator CreateCo()
    {

        if (VFXGraph != null)
        {
            VFXGraph.gameObject.SetActive(true);
            VFXGraph.Play();
        }

        float counter = 0;

        if (dissolveMaterials.Length > 0)
        {
            while (dissolveMaterials[0].GetFloat("DissolveAmount_") >= 0)
            {
                counter += dissolveRate;
                for (int i = 0; i < dissolveMaterials.Length; i++)
                    dissolveMaterials[i].SetFloat("DissolveAmount_", 1 - counter);
                yield return new WaitForSeconds(refreshRate);
            }
        }
        else
        {
            //Debug.Log("No Dissolving Material assigned to model.");
            yield break;
        }
        for (int i = 0; i < dissolveMaterials.Length; i++)
            dissolveMaterials[i].SetFloat("DissolveAmount_", 0);
    }

    public void Revive()
    {
        if (dissolveMaterials.Length > 0)
        {
            for (int i = 0; i < dissolveMaterials.Length; i++)
                dissolveMaterials[i].SetFloat("DissolveAmount_", 0);
        }
    }
}
