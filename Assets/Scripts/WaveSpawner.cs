using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    public Transform enemyPrefab;
    public float countdown = 2f;

    public Text waveCountdownText;
    public float timeBetweenWaves = 5f;

    public Transform spawnPoint;

    private int waveNumber = 1;

    void Update()
    {
        if (countdown <= 0f)
        {
            StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
        }
        countdown -= Time.deltaTime;

        countdown = Mathf.Clamp(countdown, 0f, Mathf.Infinity);

        waveCountdownText.text = string.Format("{0:00:00}", countdown);
    }

    IEnumerator SpawnWave()
    {
        waveNumber++;
        PlayerStats.Rounds++;

        for (int i = 0; i < waveNumber; i++){
            SpawnEnemy();
            yield return new WaitForSeconds(0.5f);
        }
        Debug.Log("Wave Incoming!");

    }

    void SpawnEnemy()
    {
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
}
