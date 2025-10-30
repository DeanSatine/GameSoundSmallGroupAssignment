using UnityEngine;
using FMODUnity;

public class TriggerPlaySound : MonoBehaviour
{
    public EventReference soundEvent;

    void OnTriggerEnter(Collider other)
    {
        AudioManager.instance.PlayOneShot(soundEvent, transform.position);
    }
}