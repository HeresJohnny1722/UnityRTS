using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlacementMode
{
    Fixed,
    Valid,
    Invalid
}

public class BuildingManager : MonoBehaviour
{
    public Material validPlacementMaterial;
    public Material invalidPlacementMaterial;

    public MeshRenderer[] meshComponents;
    public Dictionary<MeshRenderer, List<Material>> initialMaterials;

    [HideInInspector] public bool hasValidPlacement;
    [HideInInspector] public bool isFixed;

    private int _nObstacles;

    private int wallCounter = 0;
    public int wallNumber = 0;

    public bool isWallPlacementValid = true;

    private void Awake()
    {
        //meshComponents = this.GetComponent<BuildingConstruction>().meshComponents;
        hasValidPlacement = true;
        isFixed = true;
        _nObstacles = 0;

        _InitializeMaterials();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isFixed) return;

        // ignore ground objects
        if (_IsGround(other.gameObject)) return;

        _nObstacles++;
        SetPlacementMode(PlacementMode.Invalid);
    }

    private void OnTriggerExit(Collider other)
    {
        if (isFixed) return;

        // ignore ground objects
        if (_IsGround(other.gameObject)) return;

        _nObstacles--;
        NumberWallsInsideRadius();



        if (_nObstacles == 0 && wallNumber <= 2)
            SetPlacementMode(PlacementMode.Valid);
    }

    public void NumberWallsInsideRadius()
    {
        if (GetComponent<Building>().buildingSO.buildingType == BuildingSO.BuildingType.Wall)
        {
            wallCounter = 0;
            Debug.Log("Trying to build a wall");

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1f);

            foreach (var collider in hitColliders)
            {
                if (collider.gameObject != gameObject && !IsChildOf(transform, collider.transform))
                {
                    if (collider.GetComponent<Building>() != null)
                    {
                        if (collider.GetComponent<Building>().buildingSO.buildingType == BuildingSO.BuildingType.Wall)
                        {
                            Debug.Log("Is Next to a Wall");
                            wallCounter++;
                            Debug.Log(wallCounter);
                            
                        }
                    }
                }
            }

            // If no walls were found, set wallNumber to 0
            wallNumber = wallCounter;

            /*if (wallCounter > 2)
            {
                //Placement is invalid
                SetPlacementMode(PlacementMode.Invalid);
                wallCounter = 0;
            }
            else
            {
                if (hasValidPlacement)
                {
                    SetPlacementMode(PlacementMode.Valid);
                }
            }*/
        }
    }

    private void Update()
    {

        if (!isFixed)
        {
            NumberWallsInsideRadius();
            
            
        }
    }

    // Helper function to check if a transform is a child of another transform
    bool IsChildOf(Transform parent, Transform potentialChild)
    {
        while (potentialChild != null)
        {
            if (potentialChild == parent)
            {
                return true;
            }
            potentialChild = potentialChild.parent;
        }
        return false;
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        _InitializeMaterials();
    }
#endif

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

    private bool _IsGround(GameObject o)
    {
        return ((1 << o.layer) & BuildingPlacer.instance.groundLayerMask.value) != 0;
    }

}