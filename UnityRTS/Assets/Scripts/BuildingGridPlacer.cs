using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingGridPlacer : BuildingPlacer
{
    public float cellSize;
    public Vector2 gridOffset;
    public Renderer gridRenderer;

#if UNITY_EDITOR
    private void OnValidate()
    {
        _UpdateGridVisual();
    }
#endif

    private void Start()
    {
        _UpdateGridVisual();
        _EnableGridVisual(false);
    }

    private void Update()
    {
        if (_buildingPrefab != null)
        { // if in build mode

            // right-click: cancel build mode
            if (Input.GetMouseButtonDown(1))
            {
                _toBuild.GetComponent<Building>().CancelConstruction();
                _toBuild = null;
                _buildingPrefab = null;
                _EnableGridVisual(false);
                return;
            }

            // hide preview when hovering UI
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (_toBuild.activeSelf) _toBuild.SetActive(false);
                return;
            }
            else if (!_toBuild.activeSelf) _toBuild.SetActive(true);

            _ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(_ray, out _hit, Mathf.Infinity, groundLayerMask))
            {
                if (!_toBuild.activeSelf) _toBuild.SetActive(true);
                _toBuild.transform.position = _ClampToNearest(_hit.point, cellSize);


                if (_toBuild.GetComponent<BuildingManager>().hasValidPlacement)
                {
                    _toBuild.GetComponent<BuildingManager>().SetPlacementMode(PlacementMode.Valid);
                }



                if (Input.GetMouseButtonDown(0))
                { // if left-click
                    BuildingManager m = _toBuild.GetComponent<BuildingManager>();

                    //Guarding
                    if (!m.hasValidPlacement)
                    {
                        SoundFeedback.Instance.PlaySound(SoundType.wrongPlacement);
                        return;
                    }


                    SoundFeedback.Instance.PlaySound(SoundType.Place);
                    Building building = _toBuild.GetComponent<Building>();
                    building.InitalizeHealth();
                    building.buildingConstruction.isUnderConstruction = true;

                    GameManager.instance.RemoveResources(0, (int)building.buildingSO.goldCost, (int)building.buildingSO.woodCost, (int)building.buildingSO.foodCost, 0);
                    GameManager.instance.increaseBuildingCount(building.buildingSO);


                    m.SetPlacementMode(PlacementMode.Fixed);

                    // shift-key: chain builds
                    if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                    {

                        if (GameManager.instance.AreResourcesAvailable(0, (int)building.buildingSO.goldCost, (int)building.buildingSO.woodCost, (int)building.buildingSO.foodCost, 0) && GameManager.instance.CheckBuildingCountAvailable(building.buildingSO))
                        {

                            AstarPath.active.UpdateGraphs(_toBuild.GetComponent<BoxCollider>().bounds);
                            _toBuild = null;
                            _PrepareBuilding();

                        }
                        else
                        {
                            _buildingPrefab = null;
                            AstarPath.active.UpdateGraphs(_toBuild.GetComponent<BoxCollider>().bounds);
                            _toBuild = null;
                            _EnableGridVisual(false);

                        }

                    }
                    else
                    {
                        _buildingPrefab = null;
                        AstarPath.active.UpdateGraphs(_toBuild.GetComponent<BoxCollider>().bounds);
                        _toBuild = null;
                        _EnableGridVisual(false);


                    }


                }

            }
            else if (_toBuild.activeSelf) _toBuild.SetActive(false);
        }
    }

    protected override void _PrepareBuilding()
    {
        base._PrepareBuilding();
        _EnableGridVisual(true);
    }

    private Vector3 _ClampToNearest(Vector3 pos, float threshold)
    {
        float t = 1f / threshold;
        Vector3 v = ((Vector3)Vector3Int.FloorToInt(pos * t)) / t;

        float s = threshold * 0.5f;
        v.x += s + gridOffset.x; // (recenter in middle of cells)
        v.z += s + gridOffset.y;

        return v;
    }

    private void _EnableGridVisual(bool on)
    {
        if (gridRenderer == null) return;
        gridRenderer.gameObject.SetActive(on);
    }

    private void _UpdateGridVisual()
    {
        if (gridRenderer == null) return;
        gridRenderer.sharedMaterial.SetVector(
            "_Cell_Size", new Vector4(cellSize, cellSize, 0, 0));
    }


}