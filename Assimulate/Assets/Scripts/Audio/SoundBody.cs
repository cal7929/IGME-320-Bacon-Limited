using UnityEngine;

/// <summary>
/// My original goal was to make this as dynamic as possible for when we eventually add more sounds 
/// for other creatures, but for right now thats a bit too big or impossible a task with current architecture. 
/// Its not a bad thing, but we may just have to have different sound bodies for each creature, not a biggie just something to do later.
/// Basically just reads events from other scripts and plays sounds accordingly. Walking is the exception since 
/// its easier as of the current build to just check the horizontal velocity.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Movement))]
public class SoundBody : MonoBehaviour
{
    /// <summary>
    /// The minimum speed needed for the walking sounds to play
    /// </summary>
    [SerializeField] 
    float _MinWalkSpeedForSound = 0.1f;

    Rigidbody _body;
    Movement _movement;
    Airdash _airdash;
    Rigidbody _currentPushingBox;
    PossessionHandler _possessionHandler;

    Host _host;

    SoundSet _walkingSoundSet;
    SoundSet _jumpingSoundSet;
    SoundSet _captureSoundSet;
    SoundSet _releaseSoundSet;
    SoundSet _pushStartSoundSet;
    SoundSet _pushingSoundSet;
    SoundSet _dashSoundSet;
    SoundSet _laserSoundSet;


    void Awake()
    {
        _body = GetComponent<Rigidbody>();
        _movement = GetComponent<Movement>();
        _host = GetComponent<Host>();

        //optional components
        _airdash = GetComponent<Airdash>();

        _possessionHandler = FindFirstObjectByType<PossessionHandler>();

        #region Find Sound Sets

        SoundSet[] soundSets = GetComponents<SoundSet>();

        foreach (SoundSet s in soundSets)
        {
            switch (s.name)
            {
                case "walk":
                    _walkingSoundSet = s;
                    break;
                case "jump":
                    _jumpingSoundSet = s;
                    break;
                case "capture":
                    _captureSoundSet = s;
                    break;
                case "release":
                    _releaseSoundSet = s;
                    break;
                case "pushStart":
                    _pushStartSoundSet = s;
                    break;
                case "push":
                    _pushingSoundSet = s;
                    break;
                case "dash":
                    _dashSoundSet = s;
                    break;
                case "laser":
                    _laserSoundSet = s;
                    break;
            }
        }

        #endregion
    }

    void OnEnable()
    {
        _movement.OnJump += OnJumped;

        //check if airdash is here
        if(_airdash != null) _airdash.OnStartDash += OnDashSound;

        if(_possessionHandler != null)
        {
            _possessionHandler.Possessed += OnPossessed;
            _possessionHandler.Released += OnReleased;
        }
    }

    void OnDisable()
    {
        _movement.OnJump -= OnJumped;

        if(_possessionHandler != null)
        {
            _possessionHandler.Possessed -= OnPossessed;
            _possessionHandler.Released -= OnReleased;
        }
    }

    void FixedUpdate()
    {
        //Walking
        Vector3 v = _body.linearVelocity;
        float sidewaysSpeed = new Vector2(v.x, v.z).magnitude;

        if(_movement.IsGrounded() && sidewaysSpeed > _MinWalkSpeedForSound)
        {
            StartSSIfExists(_walkingSoundSet);
        }
        else
        {
            StopSSIfExists(_walkingSoundSet);
        }

        //pushing
        bool shouldPushSound = false;
        if(_currentPushingBox != null && _pushingSoundSet != null)
        {
            Vector3 bv = _currentPushingBox.linearVelocity;
            float boxSidewaysSpeed = new Vector2(bv.x, bv.z).magnitude;
            shouldPushSound = boxSidewaysSpeed > _MinWalkSpeedForSound;
        }

        if(shouldPushSound && !_pushingSoundSet.IsPlaying)
        {
            OnBoxPushStartSound();
        }
        else if(!shouldPushSound && _pushingSoundSet != null && _pushingSoundSet.IsPlaying)
        {
            OnBoxPushStopSound();
        }

    }

    //for when touching something else
    private void OnTriggerEnter(Collider other)
    {
        //if touching box
        if(other.CompareTag("InteractableObj"))
        {
            _currentPushingBox = other.GetComponentInParent<Rigidbody>();
        }

        //if touching laser
        if(other.CompareTag("Laser"))
        {
            OnLaserTouchSound();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if stop touching box
        if(other.CompareTag("InteractableObj"))
        {
            _currentPushingBox = null;
        }
    }

    #region Sound Events

    #region Helper functions for null checking

    bool PlaySSIfExists(SoundSet soundSet, int count)
    {
        if(soundSet != null)
        {
            soundSet.PlaySet(count);
            return true;
        }
        return false;
    }
    bool StartSSIfExists(SoundSet soundSet)
    {
        if(soundSet != null)
        {
            soundSet.StartSet();
            return true;
        }
        return false;
    }

    bool StopSSIfExists(SoundSet soundSet)
    {
        if(soundSet != null)
        {
            soundSet.StopSet();
            return true;
        }
        return false;
    }

    #endregion

    void OnJumped()
    {
        PlaySSIfExists(_jumpingSoundSet, 1);
    }

    void OnPossessed(Host possessedHost)
    {
        if(possessedHost == _host) OnCaptureSound();
    }

    void OnReleased(Host releasedHost)
    {
        if(releasedHost == _host) OnReleaseSound();
    }

    void OnCaptureSound()
    {
        PlaySSIfExists(_captureSoundSet, 1);
    }

    void OnReleaseSound()
    {
        PlaySSIfExists(_releaseSoundSet, 1);
    }

    /// <summary>
    /// Plays when an ability is used dash
    /// </summary>
    void OnDashSound()
    {
        PlaySSIfExists(_dashSoundSet, 1);
    }

    /// <summary>
    /// Plays when pushing on a box
    /// </summary>
    void OnBoxPushStartSound()
    {
        StartSSIfExists(_pushingSoundSet);
    }

    /// <summary>
    /// Plays when you stop pushing on a box
    /// </summary>
    void OnBoxPushStopSound()
    {
        StopSSIfExists(_pushingSoundSet);
    }

    /// <summary>
    /// Plays when touching a laser
    /// </summary>
    void OnLaserTouchSound()
    {
        PlaySSIfExists(_laserSoundSet, 1);
    }

    #endregion

}