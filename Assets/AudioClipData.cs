using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Audio", menuName = "Audio/SoundFX")]
public class AudioClipData : ScriptableObject
{
    [System.Serializable]
    public class SoundEffect
    {
        public AudioClip[] clips;
        [Range(0f, 1f)]
        public float volume = 1f;
    }
    
    public SoundEffect shoot = new SoundEffect();
    public SoundEffect shootExplode = new SoundEffect();
    public SoundEffect selfExplode = new SoundEffect();
    public SoundEffect death = new SoundEffect();
    public SoundEffect win = new SoundEffect();
    public SoundEffect getObj = new SoundEffect();
    public SoundEffect buttonClick = new SoundEffect();
    public SoundEffect doorExplode = new SoundEffect();
    public SoundEffect doorKeyUnlocked = new SoundEffect();
    public SoundEffect closingGate = new SoundEffect();
}
