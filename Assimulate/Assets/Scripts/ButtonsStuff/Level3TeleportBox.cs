using UnityEngine;

public class Level3TeleportBox : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        collision.gameObject.transform.position = new Vector3(-14.89f, 0.9300001f, -18.29f);
    }
}
