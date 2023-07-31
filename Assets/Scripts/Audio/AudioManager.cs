using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;
    public AudioClip backgroundMusic;  // Assign your background music in the Inspector
    public AudioClip coinCollectSound;  // Assign CoinCollect.mp3 in the Inspector
    public AudioClip starCollectSound;  // Assign StarCollect.mp3 in the Inspector
    public AudioClip eatEnemySound;  // Assign EatEnemy.mp3 in the Inspector
    public AudioClip deathSound;  // Assign deathSound.mp3 in the Inspector

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = backgroundMusic;  // Set the AudioSource to play the background music
        audioSource.loop = true;  // Set the music to loop
        audioSource.Play();  // Play background music at start
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayCoinCollectSound()
    {
        audioSource.PlayOneShot(coinCollectSound, 3f);
    }

    public void PlayStarCollectSound()
    {
        audioSource.PlayOneShot(starCollectSound, 6f);
    }

    public void PlayEatEnemySound()
    {
        audioSource.PlayOneShot(eatEnemySound, 4f);
    }

    public void PlayDeathSound()
    {
        audioSource.PlayOneShot(deathSound, 7f);
    }
}
