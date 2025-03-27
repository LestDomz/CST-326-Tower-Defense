using UnityEngine;

public class Turret : MonoBehaviour
{
    private Transform target;

    [Header("Attributes")]
    public float range = 15f;
    public float fireRate = 1f;
    private float fireCountdown = 0f;

    [Header("Unity Setup Fields")]
    public string enemyTag = "Enemy";
    public float turnSpeed = 10f;
    public Transform PartToRotate;

    public GameObject bulletPrefab;
    public Transform firePoint;

    public void Start()
    {
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
            if (bulletPrefab == null)
                Debug.LogError("BulletPrefab is not assigned in " + gameObject.name);

            if (firePoint == null)
                Debug.LogError("FirePoint is not assigned in " + gameObject.name);

    }

    void UpdateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        if (enemies.Length == 0)
        {
            //Debug.Log("No enemies found!");
            target = null;
            return;
        }

        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            if (distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        if (nearestEnemy != null && shortestDistance <= range)
        {
            Debug.Log("Target acquired: " + nearestEnemy.name);
            target = nearestEnemy.transform;
        }
        else
        {
            //Debug.Log("No enemies in range.");
            target = null;
        }
    }



    void Update()
    {
        if (target == null)
        {
            UpdateTarget();
            return;
        }

        if (PartToRotate == null)
        {
            Debug.LogError("PartToRotate is not assigned!");
            return;
        }

        // Rotate turret toward target
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(PartToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        PartToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);

        // Shooting logic
        fireCountdown -= Time.deltaTime;
        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }
    }



    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogError("BulletPrefab or FirePoint is not assigned.");
            return;
        }

        GameObject bulletGO = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = bulletGO.GetComponent<Bullet>();

        if (bullet != null)
        {
            Debug.Log("Bullet fired at " + target.name);
            bullet.Seek(target);
        }
    }




    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
