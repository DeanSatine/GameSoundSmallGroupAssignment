using System.Collections;
using System.Collections.Generic;
using FMOD;
using FMODUnity;
using FMOD.Studio;
using UnityEngine;
using Debug = FMOD.Debug;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }
    
    public bool needMusic;
    
    public EventInstance CurrentSound;
    public EventInstance musicEventInstance;
    
    private PlaySound psound;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (needMusic)
        {
            InitializeMusic(FmodEvents.instance.music);
        }
    }

    //Says music here but it's for the ambiance
    private void InitializeMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateEventInstance(musicEventReference);
        musicEventInstance.start();
    }
    
    //USABLE METHODS FOR OTHER SCRIPTS START HERE

	//Basically you do AudioManager.instance.PlayOneShot(clip, position) on an script you want one shot sound in
    public void PlayOneShot(EventReference clip, Vector3 position)
    {
        RuntimeManager.PlayOneShot(clip, position);
    }
    
    
    //Because Fmod is weird in getting clip length this creates a new EventInstance to keep track of timing
    public IEnumerator PlayVoiceLine(EventReference clip, GameObject gameObject, bool waittoEnd)
    {
        CurrentSound = CreateEventInstance(clip);
        RuntimeManager.AttachInstanceToGameObject(CurrentSound, gameObject, false);
        CurrentSound.getDescription(out EventDescription description);

        int soundLength;
        description.getLength(out soundLength);
        //UnityEngine.Debug.Log(soundLength);
        CurrentSound.start();

        if (waittoEnd)
        {
            yield return new WaitForSeconds(soundLength / 1000f);
        }
    }

    //Release EventInstance to save resources
    public void ReleaseEventInstance()
    {
        if (CurrentSound.isValid())
        {
            CurrentSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            CurrentSound.release();
        }
    }

    //To check how long an audio clip would play for
    public float GetSoundLengthInSeconds(EventReference clip)
    {
        CurrentSound = CreateEventInstance(clip);
        CurrentSound.getDescription(out EventDescription description);
        
        description.getLength(out var soundLength);
        //UnityEngine.Debug.Log(soundLength);
        CurrentSound.release();
        return (soundLength / 1000f); //in ms so have to divide 1000
    }

    public EventInstance CreateEventInstance(EventReference eventRef)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventRef);
        return eventInstance;
    }

    //USABLE SCRIPTS END HERE
    
    //Bellow are stuff that is left over from my previous game that I don't want to delete lol
    
    /*public void SetMusicArea(MusicEnum area)
    {
        musicEventInstance.setParameterByName("area", (float) area);
    }*/
    
    public void StopEventInstance(EventInstance eventInstance)
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    
    public void DisableSound()
    {
        psound = GetComponent<PlaySound>();
        EventInstance eventInstance = psound.PlaySoundEvent;
        instance.StopEventInstance(eventInstance);
    }
    
    public bool IsPlaying(EventInstance instance) {
        PLAYBACK_STATE state;   
        instance.getPlaybackState(out state);
        return state != PLAYBACK_STATE.STOPPED;
    }
    
    public void StopMusic()
    {
        if (musicEventInstance.isValid())
        {          
            musicEventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        
            musicEventInstance.release();
        }
    }
}
