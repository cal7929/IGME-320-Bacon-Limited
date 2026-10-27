using UnityEngine;

public class Buttons : MonoBehaviour
{
    [SerializeField] public HelpWithButtons helpWithButtons;

    [SerializeField] public Material buttonOn;
    [SerializeField] public Material buttonOff;

    [SerializeField] private GameObject laser;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("InteractableObj"))
        {
            Debug.Log("Button pressed");
            helpWithButtons.buttonsPressed++;
            //this.GetComponent<MeshRenderer>().material = buttonOn;
            //transform.position = new Vector3(transform.position.x, transform.position.y - 1.0f, transform.position.z);
            if (helpWithButtons.buttonsPressed == 4)
            {
                laser.SetActive(false);
                //delete laser in front of the player or hide it so it doesn't activate
            }
            Debug.Log(helpWithButtons.buttonsPressed);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("InteractableObj"))
        {
            helpWithButtons.buttonsPressed--;
            //this.GetComponent<MeshRenderer>().material = buttonOff;
            //transform.position = new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z);
            //if laser is invisible, make it visible again
            if (helpWithButtons.buttonsPressed != 4)
            {
                laser.SetActive(true);
                //delete laser in front of the player or hide it so it doesn't activate
            }
            Debug.Log(helpWithButtons.buttonsPressed);
        }
    }
}
