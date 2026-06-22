using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Radio : MonoBehaviour {

    [Tooltip("The list of the songs for the first track.")]
    [Space()]
    public AudioSource[] track1;
    [Tooltip("The list of the songs for the second track.")]
    [Space()]
    public AudioSource[] track2;
    [Tooltip("Text showing the current track.")]
    [Space()]
    public Text playlist;
    [Tooltip("Text showing the volume.")]
    [Space()]
    public Text vol;

    private AudioSource[][] tracks = { null, null };
    private AudioSource audioSource;
    private int currentTrack = 0;
    private float volume = 1.0f;
    private bool isOn = false;
    private bool toggleChange = false;

    private void Start() {
        tracks[0] = track1;
        tracks[1] = track2;
        audioSource = GetComponent<AudioSource>();
        audioSource = RandomSong();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            isOn= !isOn;
            if (isOn) {
                audioSource.Play();
            }
            else {
                audioSource.Stop();
            }
        }
        if (Input.GetKeyDown(KeyCode.I)) {
            volume += 0.1f;
            if (volume > 1) volume = 1;
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            volume -= 0.0999f;
            if (volume < 0) volume = 0;
        }
        audioSource.volume = volume;

        if (isOn) {
            if (!audioSource.isPlaying) {
                audioSource = RandomSong();
            }
            if (Input.GetKeyDown(KeyCode.T)) {
                toggleChange = true;
            }

            if (toggleChange) {
                audioSource.Stop();
                NextTrack();
                audioSource = RandomSong();
                audioSource.Play();
                toggleChange = false;
            }
        }
        if (!isOn) {
            playlist.text = "Radio off.";
            vol.text = " ";
        }
        else {
            vol.text = "Volume: " + (int)(volume * 100);
            if (currentTrack == 0) {
                playlist.text = "Current Track: Rock";
            }
            else {
                playlist.text = "Current Track: Chill";
            }
        }

    }

   private void NextTrack() {
        if (currentTrack + 1 > tracks.Length - 1) {
            currentTrack = 0;
        }
        else {
            currentTrack += 1;
        }
    }

    private AudioSource RandomSong() {
        if (currentTrack == 0) {
            return track1[Random.Range(0, track1.Length)];
        }
        else {
            return track2[Random.Range(0, track2.Length)];
        }
    }
}
