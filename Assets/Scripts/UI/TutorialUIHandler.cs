using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUIHandler : MonoBehaviour
{
    [SerializeField] private Canvas helpCanvas;

    // Update is called once per frame
    private void Update()
    {
        if (InputManager.instance.Controllers.P1_Controls.ESC.IsPressed())
        {
            helpCanvas.enabled = false;
        }
    }

    public void OnHelpBTN()
    {
        helpCanvas.enabled = !helpCanvas.enabled;
    }
}
