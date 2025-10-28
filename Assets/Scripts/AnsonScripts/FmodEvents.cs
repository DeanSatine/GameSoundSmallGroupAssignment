using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FmodEvents : MonoBehaviour
{
    //[field: Header("PlayerWalking")] public EventReference playerWalkingEvent;
    
    public EventReference music;
    public static FmodEvents instance { get; private set; }

    private void Awake()
    {
        instance = this;
    }
}
