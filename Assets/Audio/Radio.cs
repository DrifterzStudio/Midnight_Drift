using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Radio : MonoBehaviour {

    [Tooltip("The list of the songs for the first track.")]
    [Space()]
    public AudioSource[] track1;
    [Tooltip("The list of the songs for the second track.")]
    [Space()]
    public AudioSource[] track2;

    private AudioSource[][] tracks = { null, null };
    private AudioSource audioSource;
    private int currentTrack = 0;
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
            audioSource.volume += 0.1f;
        }
        if (Input.GetKeyDown(KeyCode.O)) {
            audioSource.volume -= 0.1f;
        }

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
