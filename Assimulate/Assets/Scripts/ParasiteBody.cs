using UnityEngine;

public class ParasiteBody : MonoBehaviour
{
    [SerializeField] public CharacterController characterController;
    private Rigidbody _body;
    private SoundSet _walkingSoundSet;
    private SoundSet _jumpingSoundSet;


    void Awake()
    {
        _body = gameObject.GetComponent<Rigidbody>();

        #region Find Sound Sets

        SoundSet[] soundSets = gameObject.GetComponents<SoundSet>();

        foreach(SoundSet s in soundSets)
        {
            switch(s.name)
            {
                case "walk":
                    _walkingSoundSet = s;
                    break;
                case "jump":
                    _jumpingSoundSet = s;
                    break;
            }
        }

        #endregion
    }


    void FixedUpdate()  
    {
        UpdateSound();
    }

    /// <summary>
    /// Handles sound logic for the parasite body
    /// </summary>
    void UpdateSound()
    {
        #region Walking

        //if the parasite is on the ground and is moving faster than 0.1, play the walking sound set
        if(characterController.IsGrounded() && _body.linearVelocity.magnitude > 0.1f)
        {
            _walkingSoundSet.PlaySet();
        }
        else
        {
            _walkingSoundSet.StopSet();
        }

        #endregion
    }

    //Right now, it feels like the parasite and character controller stuff are heavily entangled so I put these here instead of in the parasite update method

    //currently this is a bit buggy where it plays while the game object is diabled and so it waits until the game object is enabled again
    // I think this will resolve naturally once we have proper separation of the parasite body and the character controller
    public void PlayJumpSound()
    {
        //play 1 jump noise and then stop
        _jumpingSoundSet.PlaySet(1);
    }
}
