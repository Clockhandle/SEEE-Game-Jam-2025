using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio", menuName = "Audio/SoundFX")]
public class AudioClipData : ScriptableObject
{
    public AudioClip[] shoot;
    public AudioClip[] shootExplode;
    public AudioClip[] selfExplode;
    public AudioClip[] death;
    public AudioClip[] win;
    public AudioClip[] getObj;
    public AudioClip[] buttonClick;
    public AudioClip[] doorExplode;
    public AudioClip[] doorKeyUnlocked;
    public AudioClip[] closingGate;

}
