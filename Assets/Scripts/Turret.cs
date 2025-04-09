using UnityEngine;

public class Turret : MonoBehaviour
{
    private Transform target;
    private Enemy targetEnemy;

    [Header("General")]
    public float range = 15f;

    [Header("Use Bullets (Default)")]
    public float fireRate = 1f;
    private float fireCountdown = 0f;
    public GameObject bulletPrefab;

    [Header("Use Laser")]
    public bool useLaser = false;

    public int damageOverTime = 30;
    public float slowPct = .5f;

    public LineRenderer lineRenderer;
    public ParticleSystem impactEffect;
    public Light impactLight;

    [Header("Unity Setup Fields")]
    public string enemyTag = "Enemy";
    public float turnSpeed = 10f;
    public Transform PartToRotate;

    
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
            target = nearestEnemy.transform;
            targetEnemy = nearestEnemy.GetComponent<Enemy>();
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
            if (useLaser)
            {
                if (lineRenderer.enabled) {
                    lineRenderer.enabled = false;
                    impactEffect.Stop();
                    lineRenderer.enabled = false;
                }
            }
            return;
        }
            if (PartToRotate == null)
            {
                Debug.LogError("PartToRotate is not assigned!");
                return;
            }

            // Rotate turret toward target
            LockOnTarget();

            if (useLaser)
            {
                Laser();
            }
            else
            {
                // Shooting logic
                fireCountdown -= Time.deltaTime;
                if (fireCountdown <= 0f)
                {
                    Shoot();
                    fireCountdown = 1f / fireRate;
                }
            fireCountdown -= Time.deltaTime;
            }
    }

        void LockOnTarget()
    {
        Vector3 dir = target.position - transform.position;
        //Vector3 dir = firePoint.position - target.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Vector3 rotation = Quaternion.Lerp(PartToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        PartToRotate.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    void Laser()
    {
        targetEnemy.TakeDamage(damageOverTime * Time.deltaTime);
        targetEnemy.Slow(slowPct);

        if (!lineRenderer.enabled)
        {
            lineRenderer.enabled = true;
            impactEffect.Play();
            impactLight.enabled = true;
        }

        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, target.position);

        Vector3 dir = firePoint.position - target.position;

        impactEffect.transform.position = target.position + dir.normalized;

        //impactEffect.transform.position = Quaternion.LookRotation(dir);
        //Vector3 dir = firePoint.position - target.position;
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