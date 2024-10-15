using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketLauncher : MonoBehaviour
{
    public Transform[] Transforms; // Array of transforms to spawn rockets from
    public GameObject rocketPrefab; // Reference to the rocket prefab
    public float spawnInterval = 2.0f; // Time interval between spawns

    // Start is called before the first frame update
    void Start()
    {
        // Start the coroutine to spawn rockets
        StartCoroutine(SpawnRockets());
    }

    // Coroutine to spawn rockets at regular intervals
    private IEnumerator SpawnRockets()
    {
        while (true)
        {
            // Choose a random transform from the array
            Transform spawnPoint = Transforms[Random.Range(0, Transforms.Length)];

            // Instantiate the rocket at the chosen transform's position and rotation
            Instantiate(rocketPrefab, spawnPoint.position, spawnPoint.rotation);

            // Wait for the specified spawn interval before spawning the next rocket
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
