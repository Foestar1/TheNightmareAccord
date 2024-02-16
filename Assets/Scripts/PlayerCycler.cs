using System.Collections.Generic;
using UnityEngine;

public class PlayerCycler : MonoBehaviour
{
    private List<GameObject> playerObjects;
    private int currentIndex = 0;
    private Transform cameraTransform;
    public float orbitSensitivity = 100f; // Adjust this value to control the orbit sensitivity

    void Start()
    {
        FindAndSetPlayerObjects();

        // Find and set the camera child
        if (transform.childCount > 0)
        {
            cameraTransform = transform.GetChild(0);
        }
    }

    void Update()
    {
        // Check if the current target exists and is active
        if (playerObjects.Count == 0 || !playerObjects[currentIndex] || !playerObjects[currentIndex].activeInHierarchy)
        {
            FindAndSetPlayerObjects(); // Refresh the list if the current target is missing or inactive
        }

        // Continuously follow the current target object
        transform.position = playerObjects[currentIndex].transform.position;

        // Check for the "Fire1" button input to cycle through player objects
        if (Input.GetButtonDown("Fire1"))
        {
            CycleToNextPlayerObject();
        }

        // Orbit the camera around this game object on the Y plane based on mouse movement
        if (cameraTransform != null)
        {
            float mouseX = Input.GetAxis("Mouse X");
            cameraTransform.RotateAround(transform.position, Vector3.up, mouseX * orbitSensitivity * Time.deltaTime);
        }
    }

    void CycleToNextPlayerObject()
    {
        currentIndex = (currentIndex + 1) % playerObjects.Count;
        if (playerObjects.Count == 0 || !playerObjects[currentIndex] || !playerObjects[currentIndex].activeInHierarchy)
        {
            FindAndSetPlayerObjects(); // Ensure the new target is valid
        }
    }

    void FindAndSetPlayerObjects()
    {
        playerObjects = new List<GameObject>();
        playerObjects.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        playerObjects.AddRange(GameObject.FindGameObjectsWithTag("PlayerSpirit"));
        if (playerObjects.Count > 0)
        {
            currentIndex = 0; // Reset to the first object in the list
            transform.position = playerObjects[currentIndex].transform.position;
        }
    }
}
