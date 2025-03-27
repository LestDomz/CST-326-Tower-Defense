using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour
{
    public Color hoverColor;
    public Color notEnoughMoneyColor;
    public Vector3 positionOffset;

    [Header("Optional")]
    private Renderer rend;
    private Color startColor;

    public GameObject turret;

    BuildManager buildManager;

    void Start()
    {
        rend = GetComponent<Renderer>();
        startColor = rend.material.color;

        buildManager = BuildManager.instance;
    }

    public Vector3 GetBuildPosition()
    {
        return transform.position + positionOffset;
    }

    void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (!buildManager.CanBuild) return;

        if (EventSystem.current.IsPointerOverGameObject())
            return;
        if (buildManager.HasMoney) {
            rend.material.color = hoverColor;
        }
        else
        {
            rend.material.color = notEnoughMoneyColor;
        }
    }

    void OnMouseExit()
    {
        rend.material.color = startColor;
    }

    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (!buildManager.CanBuild)
            return;

        if (turret != null)
        {
            Debug.Log("Can't build here! - Turret already placed");
            return;
        }

        if (buildManager == null)
        {
            Debug.LogError("BuildManager instance is NULL!");
            return;
        }

        GameObject turretToBuild = buildManager.GetTurretToBuild(); // FIXED LINE
        if (turretToBuild == null)
        {
            Debug.LogError("No turret prefab set in BuildManager!");
            return;
        }

        buildManager.BuildTurretOn(this);
    }
}

