using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RotaterCharacterCustomize : MonoBehaviour
{
    [SerializeField]
    private Button thisButton;
    [SerializeField]
    private HubController hubObject;

    private void Update()
    {
        if (this.isActiveAndEnabled)
        {
            if (EventSystem.current.currentSelectedGameObject == this.gameObject)
            {
                if (Input.GetButton("Submit"))
                {
                    if (this.name == "RotateLeft")
                    {
                        if (hubObject.rotatingPlayer != -1)
                        {
                            //left
                            hubObject.rotatingPlayer = -1;
                        }
                    }
                    else
                    {
                        if (hubObject.rotatingPlayer != 1)
                        {
                            //right
                            hubObject.rotatingPlayer = 1;
                        }
                    }
                }
                else
                {
                    if (hubObject.rotatingPlayer != 0)
                    {
                        if (!Input.GetButton("Fire1"))
                        {
                            //zeroed
                            hubObject.rotatingPlayer = 0;
                        }
                    }
                }
            }
        }
    }
}
