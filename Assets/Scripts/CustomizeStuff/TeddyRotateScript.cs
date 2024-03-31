using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TeddyRotateScript : MonoBehaviour
{
    [SerializeField]
    private Button leftButton;
    [SerializeField]
    private Button rightButton;
    private int rotatingTeddy;

    private void Update()
    {
        if (this.isActiveAndEnabled)
        {
            if (EventSystem.current.currentSelectedGameObject == leftButton.gameObject || EventSystem.current.currentSelectedGameObject == rightButton.gameObject)
            {
                if (Input.GetButton("Submit"))
                {
                    if (EventSystem.current.currentSelectedGameObject == leftButton.gameObject)
                    {
                        if (rotatingTeddy != -1)
                        {
                            //left
                            rotatingTeddy = -1;
                        }
                    }
                    else
                    {
                        if (rotatingTeddy != 1)
                        {
                            //right
                            rotatingTeddy = 1;
                        }
                    }
                }
                else
                {
                    if (rotatingTeddy != 0)
                    {
                        if (!Input.GetButton("Fire1"))
                        {
                            //zeroed
                            rotatingTeddy = 0;
                        }
                    }
                }
            }
        }

        if (rotatingTeddy == -1)
        {
            this.transform.Rotate(Vector3.up, 50f * Time.deltaTime);
        }
        else if (rotatingTeddy == 1)
        {
            this.transform.Rotate(Vector3.up, -50f * Time.deltaTime);
        }
    }

    public void rotatePlushiesLeft()
    {
        if (rotatingTeddy != -1)
        {
            rotatingTeddy = -1;
        }
    }

    public void rotatePlushiesRight()
    {
        if (rotatingTeddy != 1)
        {
            rotatingTeddy = 1;
        }
    }

    public void resetPlushiesPlayer()
    {
        if (rotatingTeddy != 0)
        {
            rotatingTeddy = 0;
        }
    }
}
