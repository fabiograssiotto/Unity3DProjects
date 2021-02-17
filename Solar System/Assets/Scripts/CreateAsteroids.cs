using UnityEngine;
using System.Collections;

public class CreateAsteroids : MonoBehaviour {

	public GameObject spawnPrefab;

    public int numberOfAsteroids;
    public float radius1, radius2;
    public Transform target;

    private int asteroids = 0;
	
	// Update is called once per frame
	void Update () {

		if (asteroids < numberOfAsteroids) // is it time to spawn again?
		{
            SpawnAsteroids();
            asteroids++;
		}	
	}

	void SpawnAsteroids()
	{
        //We create a random vector that points into space
        Vector3 rngVector = new Vector3(Random.Range(radius1, radius2), 0, Random.Range(radius1, radius2));

        //now we normalize this Vector, which means - if you would rotate it around 360° it describes a circle with the radius of 1 instead of a Quad with the length of one side of radius
        rngVector.Normalize();

        //now we have a vector pointing in an random Direction, we can multiply it with a random value to point to a specific location
        rngVector *= Random.Range(radius1, radius2);

        //now you can instantiate your new Gameobject
        GameObject.Instantiate(spawnPrefab, target.transform.position + rngVector, Quaternion.identity);

        // create a new gameObject
        GameObject clone = Instantiate(spawnPrefab, transform.position, transform.rotation) as GameObject;
	}
}
