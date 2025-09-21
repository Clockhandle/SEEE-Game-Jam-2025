using UnityEngine;

[CreateAssetMenu(fileName = "SimpleMusicData", menuName = "Audio/SimpleMusic")]
public class SimpleMusicData : ScriptableObject
{
    [Header("Music Tracks")]
    [Tooltip("Music for MainMenu and LevelSelect scenes")]
    public AudioClip menuMusic;
    
    [Tooltip("Music for all Demo_ level scenes")]
    public AudioClip levelMusic;
    
    [Header("Settings")]
    [Range(0f, 1f)]
    public float menuMusicVolume = 1f;
    
    [Range(0f, 1f)]
    public float levelMusicVolume = 1f;
    
    [Tooltip("Fade duration when switching between menu and level music")]
    public float fadeDuration = 1f;
}