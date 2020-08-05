using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public GameObject spawnLocationParent;
    public Transform goldSpawnLocationParent;

    public Transform[] spawnLocations;
    private List<Transform> goldSpawnLocations;
    //The 'asteroids'
    public GameObject Object;

    //GameObject which every spawned object is a child of
    public Transform ObjectContainer;

    public GameObject player;

    private bool spawnPause = false;
    private int goldSpawnSide = 1;

    private Transform spawnedGold;
    // Start is called before the first frame update
    void Start()
    {
        goldSpawnLocations = new List<Transform>();
        spawnLocations = spawnLocationParent.GetComponentsInChildren<Transform>();

        foreach (Transform goldLocation in goldSpawnLocationParent)
        {
            goldSpawnLocations.Add(goldLocation);
        }

        foreach (Transform spaceObject in ObjectContainer)
        {
            if (spaceObject.gameObject.name == "Collectible")
            {
                spawnedGold = spaceObject;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Handles the consistent spawning of asteroids
        if (!spawnPause)
        {
            SpawnObjects(Random.Range(1, 4));
            spawnPause = true;
            StartCoroutine(SpawnDelay());
        }
    }

    /// <summary>
    /// A delay between the spawning of the asteroids
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(4f);
        spawnPause = false;
    }

    /// <summary>
    /// Spawns the asteroids in the scene.
    /// </summary>
    /// <param name="numberOfAsteroids">Number of asteroids to spawn</param>
    void SpawnObjects(int numberOfAsteroids)
    {
        for (int i = 0; i <= numberOfAsteroids; i++)
        {
            int randInt = Random.Range(0, spawnLocations.Length);
            GameObject spaceObject = GameObject.Instantiate(Object, spawnLocations[randInt].position, Quaternion.identity, ObjectContainer.transform);

            //Faces the direction of the player
            spaceObject.transform.LookAt(player.transform);

            //Randomises the start location so the asteroids don't spawn on top of each other.
            spaceObject.transform.position += Vector3.left * Random.Range(-10f, 10f);
            spaceObject.transform.position += Vector3.up * Random.Range(-10f, 10f);

            //Randomizes the size of each asteroid
            float size = Random.Range(5f, 13f);
            spaceObject.transform.localScale = new Vector3(size, size, size);
        }
    }

    /// <summary>
    /// Destroys all of the objects in the ObjectContainer, essentially restarting the scene.
    /// </summary>
    public void RemoveAllObjects()
    {
        foreach (Transform objectToDestroy in ObjectContainer)
        {
            if (objectToDestroy.gameObject.name != "Collectible")
            {
                Destroy(objectToDestroy.gameObject);
            }
        }
    }

    /// <returns>The transform of the gold in the scene</returns>
    public Transform GetGoldGameObject()
    {
        return spawnedGold;
    }

    /// <summary>
    /// Moves the gold to the next goldSpawnLocation
    /// </summary>
    public void MoveGold()
    {
        spawnedGold.position = goldSpawnLocations[goldSpawnSide].position;
        SwapSpawnSide();
    }

    /// <summary>
    /// Changes the value goldSpawnSide, which is used as the index for goldSpawnLocations
    /// Only two spawn locations currently
    /// </summary>
    private void SwapSpawnSide()
    {
        if (goldSpawnSide == 0)
        {
            goldSpawnSide = 1;
        }
        else if (goldSpawnSide == 1)
        {
            goldSpawnSide = 0;
        }
    }
}
