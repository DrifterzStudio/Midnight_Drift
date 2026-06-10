using UnityEngine;

public class Radio : MonoBehaviour {


    [Tooltip("The list of the radio songs.")]
    [Space()]
    public AudioSource[] audioSources;

    private AudioSource audioSource;
    private int currentTrack = 0;
    private bool isPlaying = false;
    private bool toggleChange = false;

    private void Start() {
        audioSource = GetComponent<AudioSource>();
        audioSource = audioSources[currentTrack];
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            isPlaying = !isPlaying;
            if (isPlaying) {
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

        if (isPlaying) {
            if (Input.GetKeyDown(KeyCode.T)) {
                toggleChange = true;
            }

            if (toggleChange) {
                audioSource.Stop();
                audioSource = NextSong();
                audioSource.Play();
                toggleChange = false;
            }
        }
    }

    private AudioSource NextSong() {
        if (currentTrack + 1 > audioSources.Length - 1) {
            currentTrack = 0; ;
        }
        else {
            currentTrack += 1;
        }
        return audioSources[currentTrack];
    }
}
