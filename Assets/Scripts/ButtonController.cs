using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public GameObject nextLevel;
    public GameObject failPanel;

    public void OpenObject()
    {
        Debug.Log("Button Clicked!");

        // Close the current object
        if (failPanel != null)
        {
            failPanel.SetActive(false);
            Debug.Log("Object Closed: " + failPanel.name);
        }

        // Open the new object
        if (nextLevel != null)
        {
            nextLevel.SetActive(true);
            Debug.Log("Object Opened: " + nextLevel.name);
        }
    }
}
