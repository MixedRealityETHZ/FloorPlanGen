using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandButtonController : MonoBehaviour
{
    public GameObject toggleButtonObject;
    public GameObject handButtonObjects;

    private PressableButtonHoloLens2 toggleButton;
    private PressableButtonHoloLens2[] handButtons;

    bool buttonToggleAlreadyClicked = false;

    void Start()
    {
        toggleButton = toggleButtonObject.GetComponent<PressableButtonHoloLens2>();

        handButtons = handButtonObjects.GetComponentsInChildren<PressableButtonHoloLens2>();

    }

    private void Update()
    {
        
    }

    void updateButtons(PressableButtonHoloLens2 buttonClicked)
    {

        if (buttonClicked == toggleButton)
        {
            //deactive and gray out or reactive and put white every other button of the furniture
            for (int i = 0; i < handButtons.Length; i++)
            {
                if (buttonToggleAlreadyClicked)
                {
                    handButtons[i].transform.Find("IconAndText").GetComponentsInChildren<MeshRenderer>()[0].material.color = Color.white;
                    handButtons[i].transform.Find("IconAndText").GetComponentsInChildren<MeshRenderer>()[1].material.color = Color.white;
                    handButtonObjects.GetComponentsInChildren<Interactable>()[i].IsEnabled = true;
                }

                else
                {
                    handButtons[i].transform.Find("IconAndText").GetComponentsInChildren<MeshRenderer>()[0].material.color = Color.gray;
                    handButtons[i].transform.Find("IconAndText").GetComponentsInChildren<MeshRenderer>()[1].material.color = Color.gray;
                    handButtonObjects.GetComponentsInChildren<Interactable>()[i].IsEnabled = false;
                }
            }

            buttonToggleAlreadyClicked = !buttonToggleAlreadyClicked;
        }

        else
        {
            //no need to check that toggle button has been clicked because when it's the case these buttons are disabled so we should 
            //not be able to click on them and trigger the event
            //ENABLE DOES NOT WORK APPARENTLY SO I NEED TO CHECK

            if (!buttonToggleAlreadyClicked)
            {
                for (int i = 0; i < handButtons.Length; i++)
                {
                    if (handButtons[i] != buttonClicked)
                    {
                        handButtons[i].transform.Find("IconAndText").GetComponentsInChildren<MeshRenderer>()[0].material.color = Color.gray;
                        handButtons[i].transform.Find("IconAndText").GetComponentsInChildren<MeshRenderer>()[1].material.color = Color.gray;
                        handButtonObjects.GetComponentsInChildren<Interactable>()[i].IsEnabled = false;
                    }
                }


                buttonClicked.transform.Find("IconAndText").GetComponentsInChildren<MeshRenderer>()[0].material.color = Color.white;
                buttonClicked.transform.Find("IconAndText").GetComponentsInChildren<MeshRenderer>()[1].material.color = Color.white;
                buttonClicked.GetComponent<Interactable>().IsEnabled = true;
            }
            
        }
    }


    public void clickAndChangeOthers(GameObject gameobject)
    { 
        Debug.Log("Cliked on" + gameobject.name);
        updateButtons(gameobject.GetComponent<PressableButtonHoloLens2>());
    }
}
