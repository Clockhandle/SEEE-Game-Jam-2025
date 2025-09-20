using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SoundFXManage : MonoBehaviour
{
    public static SoundFXManage Instance { get; private set; }

    [SerializeField] private AudioClipData audioClipData;

    private float volume = 1;
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

    //private void ShopSlot_OnClickBuy(object sender, System.EventArgs e)
    //{
    //    UnclockKey key = sender as UnclockKey;
    //    PlaySound(audioClipData.getObj, key.transform.position);
    //}

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

  

    private void PlaySound(AudioClip[] audioClipArray, Vector3 position)
    {
        PlaySoundAt(audioClipArray[Random.Range(0, audioClipArray.Length)], position);
    }

    private void PlaySoundAt(AudioClip audioClip, Vector3 position)
    {
        AudioSource.PlayClipAtPoint(audioClip, position);
    }



    public void PlayExplodeSound()
    {
        PlaySound(audioClipData.shootExplode, Vector3.zero);
    }

    public void PlayButtonClickSound(Vector3 position)
    {
        PlaySound(audioClipData.buttonClick, position);
    }

    public void PlayShootSound(Vector3 position)
    {
        PlaySound(audioClipData.shoot, position);
    }
    //public void PlayPouringSound(Vector3 position)
    //{
    //    PlaySound(audioClipData.drinkPour, position);
    //}
}
