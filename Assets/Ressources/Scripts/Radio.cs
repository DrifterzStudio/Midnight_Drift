using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class Radio : MonoBehaviour
{
    [Tooltip("The list of the songs for the first track.")]
    [Space()]
    public AudioSource[] track1;

    [Tooltip("The list of the songs for the second track.")]
    [Space()]
    public AudioSource[] track2;

    [Tooltip("The list of the songs for the third track.")]
    [Space()]
    public AudioSource[] track3;

    [Tooltip("The list of the songs for the fourth track.")]
    [Space()]
    public AudioSource[] track4;

    [Tooltip("Text showing the current track.")]
    [Space()]
    public TMP_Text playlist;

    [Tooltip("Text showing the volume.")]
    [Space()]
    public TMP_Text vol;

    [Header("Input")]
    public Key togglePowerKey = Key.R;
    public Key volumeUpKey = Key.PageUp;
    public Key volumeDownKey = Key.PageDown;
    public Key nextTrackKey = Key.T;
    public float volumeStep = 0.1f;

    [Header("Track Names")]
    public string[] trackNames = { "Rock", "Chill" };

    private AudioSource[][] tracks;
    private AudioSource currentSong;
    private int currentTrack = 0;
    private float volume = 1.0f;
    private bool isOn = false;

    private void Awake()
    {
        tracks = new AudioSource[][] { track1, track2, track3, track4 };
    }

    private void Update()
    {
        HandleInput();

        if (isOn && currentSong && !currentSong.isPlaying)
            PlayRandomSong();

        UpdateUI();
    }

    private void HandleInput()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current[togglePowerKey].wasPressedThisFrame)
            TogglePower();

        if (Keyboard.current[volumeUpKey].wasPressedThisFrame)
            AdjustVolume(volumeStep);

        if (Keyboard.current[volumeDownKey].wasPressedThisFrame)
            AdjustVolume(-volumeStep);

        if (isOn && Keyboard.current[nextTrackKey].wasPressedThisFrame)
            SwitchToNextTrack();
    }

    private void TogglePower()
    {
        isOn = !isOn;

        if (isOn)
            PlayRandomSong();
        else
            StopCurrentSong();
    }

    private void AdjustVolume(float delta)
    {
        volume = Mathf.Clamp01(volume + delta);

        if (currentSong)
            currentSong.volume = volume;
    }

    private void SwitchToNextTrack()
    {
        StopCurrentSong();
        currentTrack = (currentTrack + 1) % tracks.Length;
        PlayRandomSong();
    }

    private void PlayRandomSong()
    {
        AudioSource[] pool = tracks[currentTrack];
        currentSong = pool[Random.Range(0, pool.Length)];
        currentSong.volume = volume;
        currentSong.Play();
    }

    private void StopCurrentSong()
    {
        if (currentSong)
            currentSong.Stop();
    }

    private void UpdateUI()
    {
        if (!isOn)
        {
            playlist.text = "Radio off.";
            vol.text = " ";
            return;
        }

        vol.text = "Volume: " + (int)(volume * 100);
        playlist.text = "Current Track: " + GetTrackName(currentTrack);
    }

    private string GetTrackName(int index)
    {
        if (index >= 0 && index < trackNames.Length)
            return trackNames[index];

        return "Unknown";
    }
}