using UnityEngine;

public class LaserTarget : MonoBehaviour
{
    public void Hit()
    {
        Debug.Log("Target Hit " + name);
    }
}
