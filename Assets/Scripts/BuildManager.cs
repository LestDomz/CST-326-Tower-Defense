using UnityEngine;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one BuildManager in scene!");
            return;
        }
        instance = this;
    }

    public GameObject standardTurretPrefab;
    public GameObject missileLauncherPrefab;

    public GameObject buildEffect;

    private TurretBlueprint TurretToBuild;

    public bool CanBuild { get { return TurretToBuild != null; } }
    public bool HasMoney { get { return PlayerStats.Money >= TurretToBuild.cost; } }

    public GameObject GetTurretToBuild()  // 🔥 Fix: Added this method to return prefab
    {
        return TurretToBuild?.prefab;
    }

    public void BuildTurretOn(Node node)
    {
        if (TurretToBuild == null)
        {
            Debug.LogError("No turret selected!");
            return;
        }

        Debug.Log("Current Money: " + PlayerStats.Money + " | Turret Cost: " + TurretToBuild.cost);

        if (PlayerStats.Money < TurretToBuild.cost)
        {
            Debug.Log("Not enough money to build that!");
            return;
        }

        PlayerStats.Money -= TurretToBuild.cost;

        GameObject turret = Instantiate(TurretToBuild.prefab, node.GetBuildPosition(), Quaternion.identity);
        node.turret = turret;

        GameObject effect = (GameObject)Instantiate(buildEffect, node.GetBuildPosition(), Quaternion.identity);
        Destroy(effect, 5f);

        Debug.Log("Turret Built! Money left: " + PlayerStats.Money);
    }


    public void SelectTurretToBuild(TurretBlueprint turret)
    {
        if (turret == null)
        {
            Debug.LogError("Attempted to select a null turret!");
            return;
        }

        TurretToBuild = turret;
        Debug.Log("Turret Selected: " + TurretToBuild.prefab.name + " | Cost: " + TurretToBuild.cost);
    }

}
