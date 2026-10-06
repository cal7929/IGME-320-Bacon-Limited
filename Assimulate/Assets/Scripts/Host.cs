using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Host : MonoBehaviour
{
    public static readonly List<Host> All = new List<Host>();

    [Header("Camera")]
    public float cameraHeight = 1.2f;
    public float cameraDistance = 5f;

    [Header("Possession")]
    //Where the parasite leaves the host from
    public Vector3 ejectOffset = new Vector3(0f, 2f, 0f); 
    
    //This is for the eventual "press e to "Assimulate" prompt or whatever we want to do.
    public GameObject prompt;

    public Rigidbody Body { get; private set; }

    Collider col;

    void Awake()
    {
        Body = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        ShowPrompt(false);
    }

    void OnEnable()
    {
        All.Add(this);
    }

    void OnDisable()
    {
        All.Remove(this);
    }

    /// <summary>
    /// Determines the distance from a point to this creature's surface.
    /// </summary>
    /// <param name="point"></param>
    /// <returns>Distance between host and player</returns>
    public float DistanceTo(Vector3 point)
    {
        return Vector3.Distance(col.ClosestPoint(point), point);
    }      

    /// <summary>
    /// Shows the interact prompt on creatures the parasite can possess
    /// </summary>
    /// <param name="show"></param>
    public void ShowPrompt(bool show)
    {
        if (prompt != null)
        {
            prompt.SetActive(show);
        }
    }
}
