using UnityEngine;

public class Buttons : MonoBehaviour
{
    [SerializeField] public HelpWithButtons helpWithButtons;

    [SerializeField] private GameObject laser;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("InteractableObj"))
        {
            Debug.Log("Button pressed");
            helpWithButtons.buttonsPressed++;
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
