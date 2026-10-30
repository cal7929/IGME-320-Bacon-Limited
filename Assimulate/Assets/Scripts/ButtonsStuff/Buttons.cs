using UnityEngine;

public class Buttons : MonoBehaviour
{
    [SerializeField] public HelpWithButtons helpWithButtons;

    [SerializeField] private GameObject button;

    [SerializeField] private MeshRenderer buttonMesh;

    [SerializeField] public Material buttonOn;
    [SerializeField] public Material buttonOff;

    [SerializeField] private GameObject laser;
    [SerializeField] private GameObject laser2;

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("InteractableObj"))
        {
            Debug.Log("Button pressed");
            //Make the buttons pressed +1
            helpWithButtons.buttonsPressed++;

            //Change the button to green 
            buttonMesh.material = buttonOn;

            //Move button down
            button.transform.position = new Vector3(
                button.transform.position.x, 
                button.transform.position.y - .05f, 
                button.transform.position.z);
            //If buttons pressed equal 4 set the laser to invisible 
            if (helpWithButtons.buttonsPressed == 4)
            {
                laser.SetActive(false);
                laser2.SetActive(false);
                //delete laser in front of the player or hide it so it doesn't activate
            }
            Debug.Log(helpWithButtons.buttonsPressed);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("InteractableObj"))
        {
            //Make the buttons pressed -1
            helpWithButtons.buttonsPressed--;

            //Change the button to red 
            buttonMesh.material = buttonOff;

            //Move the button back up
            button.transform.position = new Vector3(
                button.transform.position.x,
                button.transform.position.y + .05f,
                button.transform.position.z);
            //if laser is invisible, make it visible again
            if (helpWithButtons.buttonsPressed != 4)
            {
                laser.SetActive(true);
                laser2.SetActive(true);
                //delete laser in front of the player or hide it so it doesn't activate
            }
            Debug.Log(helpWithButtons.buttonsPressed);
        }
    }
}
