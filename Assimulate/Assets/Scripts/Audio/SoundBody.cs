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
    [SerializeField] 
    float isWalkingSpeed = 0.1f;

    Rigidbody _body;
    Movement _movement;
    PossessionHandler _possessionHandler;

    Host _host;

    SoundSet _walkingSoundSet;
    SoundSet _jumpingSoundSet;
    SoundSet _biteSoundSet;

    void Awake()
    {
        _body = GetComponent<Rigidbody>();
        _movement = GetComponent<Movement>();
        _host = GetComponent<Host>();

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
                case "bite":
                    _biteSoundSet = s;
                    break;
            }
        }

        #endregion
    }

    void OnEnable()
    {
        _movement.Jumped += OnJumped;

        if (_possessionHandler != null)
        {
            _possessionHandler.Possessed += OnPossessed;
            _possessionHandler.Released += OnReleased;
        }
    }

    void OnDisable()
    {
        _movement.Jumped -= OnJumped;

        if (_possessionHandler != null)
        {
            _possessionHandler.Possessed -= OnPossessed;
            _possessionHandler.Released -= OnReleased;
        }
    }

    void OnJumped()
    {
        if (_jumpingSoundSet != null)
        {
            _jumpingSoundSet.PlaySet(1);
        }
    }

    void OnPossessed(Host possessedHost)
    {
        if (possessedHost == _host)
        {
            PlayBite();
        }
    }

    void OnReleased(Host releasedHost)
    {
        if (releasedHost == _host)
        {
            PlayBite();
        }
    }

    void PlayBite()
    {
        if (_biteSoundSet != null)
        {
            _biteSoundSet.PlaySet(1);
        }
    }

    void FixedUpdate()
    {
        UpdateSound();
    }

    void UpdateSound()
    {
        if (_walkingSoundSet == null) return;

        Vector3 v = _body.linearVelocity;
        float sidewaysSpeed = new Vector2(v.x, v.z).magnitude;

        if (_movement.IsGrounded() && sidewaysSpeed > isWalkingSpeed)
        {
            _walkingSoundSet.PlaySet();
        }
        else
        {
            _walkingSoundSet.StopSet();
        }
    }
}