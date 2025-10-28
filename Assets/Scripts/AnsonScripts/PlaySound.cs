using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    public EventReference PlaySoundClip;
    public EventInstance PlaySoundEvent;
    
    public void PlayAudio()
    {
        PlaySoundEvent = RuntimeManager.CreateInstance(PlaySoundClip);
        PlaySoundEvent.start();
    }

    public void StopAudio()
    {
        PlaySoundEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
