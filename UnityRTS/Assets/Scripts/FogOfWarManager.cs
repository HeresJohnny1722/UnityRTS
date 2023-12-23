using UnityEngine;

public class FogOfWarManager : MonoBehaviour
{
    public LayerMask revealLayer;
    public float revealRadius = 10f;
    public GameObject fogGroup; // Reference to your fog planes
    public bool isVisible = true;

    void Update()
    {

        UpdateFogOfWar();
    }

    void UpdateFogOfWar()
    {
        
        for (int i = 0; i < fogGroup.transform.childCount; i++)
        {
            bool shouldReveal = CheckRevealCondition(fogGroup.transform.GetChild(i).transform.position);
            if (shouldReveal == false)
            {
                isVisible = false;
                fogGroup.transform.GetChild(i).gameObject.SetActive(shouldReveal);
                //destroy the fog things
            }
        }
    }

    bool CheckRevealCondition(Vector3 planePosition)
    {
        Collider[] hitColliders = Physics.OverlapSphere(planePosition, revealRadius, revealLayer);

        return hitColliders.Length <= 0;
    }
}
