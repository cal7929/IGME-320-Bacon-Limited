using UnityEngine;

/// <summary>
/// A set of sounds which are played together
/// </summary>
public class SoundSet : MonoBehaviour
{
    [Header("General")]
    [SerializeField] public new string name;
    [SerializeField] public AudioClip[] sounds;

    [Header("Variation and Randomization")]
    //if the sounds should be played randomly from the pool of sounds
    [SerializeField] public bool randomizeOrder;
    //how high and low the pitch can be randomized at
    [SerializeField][Range(0f, 100f)] public float pitchVariationThreshold;


    #region Playing Status

    //the amount of times this sound set has left to play (if negative, playing forever until manually stopped)
    private int _iterationsLeft = 0;
    public bool IsPlaying => _iterationsLeft != 0;
    /// <summary>
    /// True if playing forever without a limit until manually stopped
    /// </summary>
    public bool IsPlayingIndefinite => _iterationsLeft < 0;
    /// <summary>
    /// True if playing with a set amount of iterations before finishing
    /// </summary>
    public bool IsPlayingFinite => _iterationsLeft > 0;

    #endregion

    #region Cooldown

    [Header("Cooldown")]

    //how often a sound should be played (the cooldown time until another sound is played) in positive seconds
    [SerializeField][Range(0f, 100f)] public float soundCooldown;
    //how fast and slow the cooldown can be randomized at
    [SerializeField] public float cooldownVariationThreshold;
    //the next time this sound should be played (when the cooldown expires)
    private float _nextPlayTime;

    #endregion

    
    //the game object that the sound should play from in 3D space
    private AudioSource source;

    //the last sound index played
    private int _index = 0;


    public void Awake()
    {
        //add audio source to this game object so it can play sound
        source = gameObject.AddComponent<AudioSource>();
        //have it sound 3D
        source.spatialize = true;
        source.spatialBlend = 1f;
    }

    private void Update()
    {
        #region Playing Status

        //if not playing, return early
        if(!IsPlaying) return;
        //decrement if finite
        if(IsPlayingFinite) _iterationsLeft--;

        #endregion

        #region Cooldown Status

        //if still in cooldown, return early
        if(Time.time < _nextPlayTime) return;
        //otherwise calculate the next time
        float totalCooldown = Time.time + soundCooldown;
        if(cooldownVariationThreshold > 0f)
        {
            totalCooldown = UnityEngine.Random.Range(totalCooldown - cooldownVariationThreshold, totalCooldown + cooldownVariationThreshold);
        }
        _nextPlayTime = totalCooldown;

        #endregion

        #region Iterate Index

        //if random, set sound to random
        if(randomizeOrder)
        {
            _index = UnityEngine.Random.Range(0, sounds.Length);
        }
        else //if not randomized iterate index
        {
            _index++;
            //wrap back around if too high
            if(_index >= sounds.Length) _index = 0;
        }

        #endregion

        #region Pitch

        float finalPitch = 1f;
        //if pitch is randomized
        if(pitchVariationThreshold > 0f)
        {
            finalPitch = UnityEngine.Random.Range(finalPitch - pitchVariationThreshold, finalPitch + pitchVariationThreshold);
        }
        source.pitch = finalPitch;

        #endregion

        //if it makes it this far, play the sound
        source.PlayOneShot(sounds[_index]);
    }

    /// <summary>
    /// Enables this set to play indefinetly until manually stopped
    /// </summary>
    public void PlaySet()
    {
        _iterationsLeft = -1;
    }

    /// <summary>
    /// Enables this set to play the specified amount of times
    /// </summary>
    public void PlaySet(int count)
    {
        _iterationsLeft = count;
    }

    /// <summary>
    /// Stops playing this set
    /// </summary>
    public void StopSet()
    {
        _iterationsLeft = 0;
    }
}
