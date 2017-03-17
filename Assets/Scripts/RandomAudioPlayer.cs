using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RandomAudioPlayer : MonoBehaviour
{
    #region vars
    public float RandomChange = 100f;
    public float delay = 0.4f;
    public bool DontRepeatSound = false;
    public AudioSource[] Audio;
    public bool PlayOnAwake = false;
    #endregion
    #region init
    private void Start()
    {
        if (PlayOnAwake) Play();
    }
    #endregion
    #region logic
    #endregion
    #region interface
    public void Play()
    {
        if (DontRepeatSound) {
            foreach (var item in Audio)
            {
                if (item.isPlaying) return;
            }
        }

        if (Random.Range(0, 100) < RandomChange)
            Rand(Audio).PlayDelayed(delay);
    }

    public static T Rand<T>(IEnumerable<T> enumerable)
    {
        if (enumerable.Count() == 0)
        {
            Debug.LogError("");
        }

        return enumerable.ElementAt(Random.Range(0, enumerable.Count()));
    }
    #endregion
    #region internal
    #endregion
    #region events
    #endregion
}
