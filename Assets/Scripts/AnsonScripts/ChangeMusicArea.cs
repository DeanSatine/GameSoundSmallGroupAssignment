using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMusicArea : MonoBehaviour
{
    public MusicEnum area;
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag.Equals("Player"))
        {
            AudioManager.instance.SetMusicArea(area);
        }
    }
}    