using UnityEngine;
using System;

public class AudioControlDevice : MonoBehaviour
{
    //drop down stuff
    [Serializable]
    public class VariableHolder
    {
        public bool soundVolume;
        public bool musicVolume;
    }

    public VariableHolder VolumeType = new VariableHolder();

    private AudioSource objectAudioSource;
    private SaveAndLoadData gamesData;

    private void Awake()
    {
        objectAudioSource = this.GetComponent<AudioSource>();
        gamesData = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();
    }

    private void OnEnable()
    {
        if (objectAudioSource == null)
        {
            objectAudioSource = this.GetComponent<AudioSource>();
        }

        if (gamesData == null)
        {
            //find it and set it
            gamesData = GameObject.Find("PersistantSaveAndLoad").GetComponent<SaveAndLoadData>();
        }
    }

    private void Update()
    {
        if (VolumeType.soundVolume && objectAudioSource.volume != gamesData.soundVolume)
        {
            objectAudioSource.volume = gamesData.soundVolume;
        }else if (VolumeType.musicVolume && objectAudioSource.volume != gamesData.musicVolume)
        {
            objectAudioSource.volume = gamesData.musicVolume;
        }
    }
}
