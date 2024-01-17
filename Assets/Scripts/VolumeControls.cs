using UnityEngine;
using UnityEngine.UI;
using System;

public class VolumeControls : MonoBehaviour
{
    //volume stuff UI
    [SerializeField]
    private Image volumeImage;
    [SerializeField]
    private Sprite[] volumeImageChoices;
    [SerializeField]
    private Slider volumeSlider;

    private SaveAndLoadData gamesData;

    //drop down stuff
    [Serializable]
    public class VariableHolder
    {
        public bool soundVolume;
        public bool musicVolume;
    }

    public VariableHolder VolumeType = new VariableHolder();

    private void OnEnable()
    {
        //no save and load object?
        if (gamesData == null)
        {
            //find it and set it
            gamesData = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();
        }

        //check what volume this component is
        if (VolumeType.soundVolume)
        {
            volumeSlider.value = gamesData.soundVolume;
        }else if (VolumeType.musicVolume)
        {
            volumeSlider.value = gamesData.musicVolume;
        }

        changeIcon();
    }

    public void changeIcon()
    {
        if (volumeSlider.value != 0)
        {
            volumeImage.sprite = volumeImageChoices[0];
        }
        else
        {
            volumeImage.sprite = volumeImageChoices[1];
        }
    }

    public void changeVolume()
    {
        //check what volume this component is
        if (VolumeType.soundVolume)
        {
            gamesData.soundVolume = volumeSlider.value;
        }
        else if (VolumeType.musicVolume)
        {
            gamesData.musicVolume = volumeSlider.value;
        }
    }

    public void saveVolumes()
    {
        gamesData.saveInfo();
    }
}
