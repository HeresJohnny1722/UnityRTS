using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public interface IBuilderPlacer
{
    public void SetBuildingPrefab(GameObject prefab);
}
public class BuildingPlacer : MonoBehaviour, IBuilderPlacer
{
    public static BuildingPlacer instance; // (Singleton pattern)

    public LayerMask groundLayerMask;

    public GameObject _buildingPrefab;
    protected GameObject _toBuild;

    protected Camera _mainCamera;

    protected Ray _ray;
    protected RaycastHit _hit;

    private void Awake()
    {
        instance = this; // (Singleton pattern)
        _mainCamera = Camera.main;
        _buildingPrefab = null;
    }

    private void Update()
    {
        if (_buildingPrefab != null)
        { // if in build mode

            try
            {
                // right-click: cancel build mode
                if (Input.GetMouseButtonDown(1))
                {
                    Destroy(_toBuild);
                    _toBuild = null;
                    _buildingPrefab = null;
                    return;
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }

            

            // hide preview when hovering UI
            if (EventSystem.current.IsPointerOverGameObject())
            {
                if (_toBuild.activeSelf) _toBuild.SetActive(false);
                return;
            }
            else if (!_toBuild.activeSelf) _toBuild.SetActive(true);

            // rotate preview with Spacebar
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _toBuild.transform.Rotate(Vector3.up, 90);
            }

            _ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(_ray, out _hit, 1000f, groundLayerMask))
            {
                if (!_toBuild.activeSelf) _toBuild.SetActive(true);
                _toBuild.transform.position = _hit.point;

                if (Input.GetMouseButtonDown(0))

                { // if left-click
                    BuildingManager m = _toBuild.GetComponent<BuildingManager>();
                    if (m.hasValidPlacement)
                    {
                        
                        m.SetPlacementMode(PlacementMode.Fixed);

                        /*
                        // shift-key: chain builds
                        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                        {
                            _toBuild = null; // (to avoid destruction)
                            _PrepareBuilding();
                        } 
                        // exit build mode
                        else
                        {*/
                            _buildingPrefab = null;
                            _toBuild = null;
                        BuildingSelection.Instance.DeselectBuilding();
                        //}
                    }
                }

            }
            else if (_toBuild.activeSelf) _toBuild.SetActive(false);
        }
    }

    public void SetBuildingPrefab(GameObject prefab)
    {
        UnitSelection.Instance.DeselectAll();
        Building building = prefab.GetComponent<Building>();
        if (GameManager.instance.AreResourcesAvailable(0, (int)building.buildingSO.goldCost, (int)building.buildingSO.woodCost, (int)building.buildingSO.foodCost, 0))
        {
            if (GameManager.instance.CheckBuildingCountAvailable(building.buildingSO))
            {
                //building.setMaterials();
                SoundFeedback.Instance.PlaySound(SoundType.Click);
                BuildingSelection.Instance.DeselectBuilding();
                _buildingPrefab = prefab;
                _PrepareBuilding();
                EventSystem.current.SetSelectedGameObject(null); // cancel keyboard UI nav
            } else
            {
                SoundFeedback.Instance.PlaySound(SoundType.wrongPlacement);
            }
            

            
        }
    }

    protected virtual void _PrepareBuilding()
    {
        if (_toBuild) Destroy(_toBuild);

        _toBuild = Instantiate(_buildingPrefab);
        _toBuild.SetActive(false);

        BuildingManager m = _toBuild.GetComponent<BuildingManager>();
        m.isFixed = false;
        m.SetPlacementMode(PlacementMode.Valid);
    }

}