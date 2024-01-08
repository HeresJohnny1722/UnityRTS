using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enumeration representing different placement modes for a building.
/// </summary>
public enum PlacementMode
{
    Fixed,
    Valid,
    Invalid
}

/// <summary>
/// Handles the placement mode and material of a building, indicating whether the placement is valid, invalid, or fixed.
/// </summary>
public class BuildingManager : MonoBehaviour
{
    public Material validPlacementMaterial;
    public Material invalidPlacementMaterial;

    public MeshRenderer[] meshComponents;

    public Dictionary<MeshRenderer, List<Material>> initialMaterials;

    [HideInInspector] public bool hasValidPlacement;
    [HideInInspector] public bool isFixed;

    private int _nObstacles;

    public bool isWallPlacementValid = true;

    /// <summary>
    /// Initializes the building manager's state and material dictionary.
    /// </summary>
    private void Awake()
    {
        hasValidPlacement = true;
        isFixed = true;
        _nObstacles = 0;
        _InitializeMaterials();
    }

    /// <summary>
    /// Handles triggering events when another collider enters the building's placement area.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (isFixed) return;
        if (_IsGround(other.gameObject)) return;

        _nObstacles++;
        SetPlacementMode(PlacementMode.Invalid);
    }

    /// <summary>
    /// Handles triggering events when another collider exits the building's placement area.
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (isFixed) return;
        if (_IsGround(other.gameObject)) return;

        _nObstacles--;

        if (_nObstacles == 0)
            SetPlacementMode(PlacementMode.Valid);
    }

    /// <summary>
    /// Validates the placement mode and sets the appropriate material for visualization.
    /// </summary>
    /// <param name="mode">The desired placement mode (Fixed, Valid, or Invalid).</param>
    public void SetPlacementMode(PlacementMode mode)
    {
        if (mode == PlacementMode.Fixed)
        {
            isFixed = true;
            hasValidPlacement = true;
        }
        else if (mode == PlacementMode.Valid)
        {
            hasValidPlacement = true;
        }
        else
        {
            hasValidPlacement = false;
        }
        SetMaterial(mode);
    }

    /// <summary>
    /// Sets the material for the building based on the specified placement mode.
    /// </summary>
    /// <param name="mode">The placement mode (Fixed, Valid, or Invalid).</param>
    public void SetMaterial(PlacementMode mode)
    {
        if (mode == PlacementMode.Fixed)
        {
            foreach (MeshRenderer r in meshComponents)
                r.sharedMaterials = initialMaterials[r].ToArray();
        }
        else
        {
            Material matToApply = mode == PlacementMode.Valid
                ? validPlacementMaterial : invalidPlacementMaterial;

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

    /// <summary>
    /// Initializes the initial materials dictionary to store original materials of mesh renderers.
    /// </summary>
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

    /// <summary>
    /// Checks if the provided game object is associated with the ground layer.
    /// </summary>
    /// <param name="o">The game object to check.</param>
    /// <returns>True if the game object is associated with the ground layer; otherwise, false.</returns>
    private bool _IsGround(GameObject o)
    {
        return ((1 << o.layer) & BuildingPlacer.instance.groundLayerMask.value) != 0;
    }
}
