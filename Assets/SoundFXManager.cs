using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundFXManage : MonoBehaviour
{
    public static SoundFXManage Instance { get; private set; }

    [SerializeField] private AudioClipData audioClipData;

    private float volume = 1;
    
    // Add cooldown system to prevent audio spam
    private float lastExplodeTime = 0f;
    private float explodeCooldown = 0.1f; // Minimum time between explosion sounds
    
    private void Awake()
    {
        Instance = this;
   
    }

    private void Start()
    {

        DeathObj.OnDeath += DeathObj_OnDeath;
        UnlockDoorBomb.OnBombExplode += UnlockDoorBomb_OnBombExplode;
        UnlockDoorBomb.OnGetUnlockBomb += UnlockDoorBomb_OnGetUnlockBomb;
        UnclockKey.OnGetUnlockkey += UnclockKey_OnGetUnlockkey;
        UnlockedDoorFlash.OnDoorUnlocked += UnlockedDoorFlash_OnDoorUnlocked;
        WinFlagGoal.instance.OnTriggerWinFlag += WinFlagGoal_OnTriggerWinFlag;

       

        //SElfExplode

        //closingGate


    }


    private void DeathObj_OnDeath(object sender, System.EventArgs e)
    {
        DeathObj deathObj = sender as DeathObj;
        PlaySound(audioClipData.death, deathObj.transform.position);
    }
    private void UnlockDoorBomb_OnBombExplode(object sender, System.EventArgs e)
    {
        UnlockDoorBomb unlockdoorBombm = sender as UnlockDoorBomb;
        PlaySound(audioClipData.doorExplode, unlockdoorBombm.transform.position);
    }

    private void UnlockDoorBomb_OnGetUnlockBomb(object sender, System.EventArgs e)
    {
        UnlockDoorBomb bomb = sender as UnlockDoorBomb;
        PlaySound(audioClipData.getObj, bomb.transform.position);
    }

    private void UnclockKey_OnGetUnlockkey(object sender, System.EventArgs e)
    {
        UnclockKey key = sender as UnclockKey;
        PlaySound(audioClipData.getObj, key.transform.position);
    }

    private void UnlockedDoorFlash_OnDoorUnlocked(object sender, System.EventArgs e)
    {
        UnlockedDoorFlash doorFlash = sender as UnlockedDoorFlash;
        PlaySound(audioClipData.doorKeyUnlocked, doorFlash.transform.position);
    }

    private void WinFlagGoal_OnTriggerWinFlag(object sender, System.EventArgs e)
    {
        WinFlagGoal win = WinFlagGoal.instance;
        PlaySound(audioClipData.win, win.transform.position);
    }

  

    private void PlaySound(AudioClipData.SoundEffect soundEffect, Vector3 position)
    {
        if (soundEffect.clips.Length > 0)
        {
            AudioClip clipToPlay = soundEffect.clips[Random.Range(0, soundEffect.clips.Length)];
            PlaySoundAt(clipToPlay, position, soundEffect.volume);
        }
    }

    private void PlaySoundAt(AudioClip audioClip, Vector3 position, float volume = 1f)
    {
        // Create a temporary GameObject for the AudioSource
        GameObject tempAudioSource = new GameObject("TempAudio");
        tempAudioSource.transform.position = position;
        
        AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = volume * this.volume; // Use both individual and master volume
        audioSource.Play();
        
        // Destroy the temporary GameObject after the clip finishes
        Destroy(tempAudioSource, audioClip.length);
    }



    public void PlayExplodeSound()
    {
        // Add cooldown to prevent audio spam when rocket jumping rapidly
        if (Time.time >= lastExplodeTime + explodeCooldown)
        {
            PlaySound(audioClipData.shootExplode, Vector3.zero);
            lastExplodeTime = Time.time;
        }
    }

    public void PlayButtonClickSound(Vector3 position)
    {
        PlaySound(audioClipData.buttonClick, position);
    }

    public void PlayShootSound(Vector3 position)
    {
        PlaySound(audioClipData.shoot, position);
    }
    
    // Master volume control
    public void SetMasterVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
    }
    
    public float GetMasterVolume()
    {
        return volume;
    }
}
