using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for setting the units to white when hit by an attack, sets them to white for .15 seconds then sets them back to their orignal color
/// </summary>
public class TroopHit : MonoBehaviour
{
    public Material hitMaterial;

    public MeshRenderer[] meshComponents;
    public Dictionary<MeshRenderer, List<Material>> initialMaterials;

    public void Awake()
    {
        _InitializeMaterials();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        _InitializeMaterials();
    }
#endif

    public void _InitializeMaterials()
    {
        if (initialMaterials == null)
            initialMaterials = new Dictionary<MeshRenderer, List<Material>>();
        if (initialMaterials.Count > 0)
        {
            foreach (var l in initialMaterials) l.Value.Clear();
            initialMaterials.Clear();
        }

        foreach (MeshRenderer r in meshComponents)
        {
            initialMaterials[r] = new List<Material>(r.sharedMaterials);
        }
    }

    public void HitAnimation()
    {
        SetMaterialWhite();
        Invoke("SetMaterialInitial", 0.15f);
    }

    public void SetMaterialInitial()
    {

        foreach (MeshRenderer r in meshComponents)
            r.sharedMaterials = initialMaterials[r].ToArray();

    }

    public void SetMaterialWhite()
    {
        
            Material matToApply = hitMaterial;

            Material[] m; int nMaterials;
            foreach (MeshRenderer r in meshComponents)
            {
                nMaterials = initialMaterials[r].Count;
                m = new Material[nMaterials];
                for (int i = 0; i < nMaterials; i++)
                    m[i] = matToApply;
                r.sharedMaterials = m;
            }
        
    }
}
