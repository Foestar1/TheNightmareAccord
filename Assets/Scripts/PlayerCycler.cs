using System.Collections.Generic;
using UnityEngine;

public class PlayerCycler : MonoBehaviour
{
    private List<GameObject> playerObjects;
    private int currentIndex = 0;
    private Transform cameraTransform;
    public float orbitSensitivity = 100f; // Adjust this value to control the orbit sensitivity
    public float minDistance = 2f; // Minimum distance from the object
    public float maxDistance = 10f; // Maximum distance the camera can be from the object
    public LayerMask ignoreLayers; // Layers to ignore by the raycast

    void Start()
    {
        FindAndSetPlayerObjects();

        // Find and set the camera child
        if (transform.childCount > 0)
        {
            cameraTransform = transform.GetChild(0);
        }

        // Set up the layer mask to ignore specific layers (6, 8, 10)
        ignoreLayers = ~((1 << 6) | (1 << 8) | (1 << 10));
    }

    void Update()
    {
        if (playerObjects.Count == 0 || !playerObjects[currentIndex] || !playerObjects[currentIndex].activeInHierarchy)
        {
            FindAndSetPlayerObjects();
        }

        transform.position = playerObjects[currentIndex].transform.position;

        if (Input.GetButtonDown("Fire1"))
        {
            CycleToNextPlayerObject();
        }

        if (cameraTransform != null)
        {
            float mouseX = Input.GetAxis("Mouse X");
            cameraTransform.RotateAround(transform.position, Vector3.up, mouseX * orbitSensitivity * Time.deltaTime);
            AdjustCameraDistance();
        }
    }

    void CycleToNextPlayerObject()
    {
        currentIndex = (currentIndex + 1) % playerObjects.Count;
        if (playerObjects.Count == 0 || !playerObjects[currentIndex] || !playerObjects[currentIndex].activeInHierarchy)
        {
            FindAndSetPlayerObjects();
        }
    }

    void FindAndSetPlayerObjects()
    {
        playerObjects = new List<GameObject>();
        playerObjects.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        playerObjects.AddRange(GameObject.FindGameObjectsWithTag("PlayerSpirit"));
        if (playerObjects.Count > 0)
        {
            currentIndex = 0;
            transform.position = playerObjects[currentIndex].transform.position;
        }
    }

    void AdjustCameraDistance()
    {
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - transform.position;
        float desiredDistance = maxDistance;

        if (Physics.Raycast(transform.position, direction, out hit, maxDistance, ignoreLayers))
        {
            desiredDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }

        cameraTransform.position = transform.position + direction.normalized * desiredDistance;
    }
}
