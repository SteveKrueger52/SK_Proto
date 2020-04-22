/*Prototyping for XD
 * Mark Sivak
 * This script plays a piece of audio when the player collides with a sphereical location
 */

using UnityEngine;

public class AudioTriggerOnce : MonoBehaviour
{
    private AudioSource source; //the audio source
    private bool played;

    void Start()
    {
        source = GetComponent<AudioSource>();  //get the audio source from this game component   
    }

    void OnTriggerEnter(Collider collider)
    {
        if (!played)
        {
            source.Play(); //play the audio
            played = true;
        }
    }
}
