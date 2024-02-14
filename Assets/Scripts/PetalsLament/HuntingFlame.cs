using UnityEngine;

public class HuntingFlame : MonoBehaviour
{
    public GameObject targetObject;
    public float speed;

    public void Update()
    {
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetObject.transform.position, step);
    }
}
