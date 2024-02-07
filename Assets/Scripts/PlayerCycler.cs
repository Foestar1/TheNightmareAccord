using System.Collections.Generic;
using UnityEngine;

public class PlayerCycler : MonoBehaviour
{
    private List<GameObject> playerObjects;
    private int currentIndex = 0;
    private Transform cameraTransform;
    public float orbitSpeed = 50f;

    void Start()
    {
        // Initialize the list
        playerObjects = new List<GameObject>();

        // Find all game objects tagged as "Player" and "PlayerSpirit" and add them to the list
        playerObjects.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        playerObjects.AddRange(GameObject.FindGameObjectsWithTag("PlayerSpirit"));

        // Move this game object to the position of the first player object, if available
        if (playerObjects.Count > 0)
        {
            transform.position = playerObjects[currentIndex].transform.position;
        }

        // Find and set the camera child
        if (transform.childCount > 0)
        {
            cameraTransform = transform.GetChild(0);
        }
    }

    void Update()
    {
        // Check for the "Fire1" button input to cycle through player objects
        if (Input.GetButtonDown("Fire1"))
        {
            CycleToNextPlayerObject();
        }

        // Orbit the camera around this game object on the Y plane
        if (cameraTransform != null)
        {
            cameraTransform.RotateAround(transform.position, Vector3.up, orbitSpeed * Time.deltaTime);
        }
    }

    void CycleToNextPlayerObject()
    {
        if (playerObjects.Count > 0)
        {
            // Increment the current index, and wrap around if it goes beyond the list's count
            currentIndex = (currentIndex + 1) % playerObjects.Count;

            // Move this game object to the position of the next player object
            transform.position = playerObjects[currentIndex].transform.position;
        }
    }
}
