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
    public TrackingHub trackingHub;
    public Model model;

    private PressableButtonHoloLens2 toggleButton;
    private PressableButtonHoloLens2[] handButtons;

    bool toggleInManualTracking = false; //true = automatic mode; manual by default

    void Start()
    {
        toggleButton = toggleButtonObject.GetComponent<PressableButtonHoloLens2>();

        handButtons = handButtonObjects.GetComponentsInChildren<PressableButtonHoloLens2>();


        //by default, automatic mode so gray out every button
        for (int i = 0; i < handButtons.Length; i++)
        {
            handButtons[i].transform.Find("IconAndText").GetComponentsInChildren<MeshRenderer>()[0].material.color = Color.gray;
            handButtons[i].transform.Find("IconAndText").GetComponentsInChildren<MeshRenderer>()[1].material.color = Color.gray;
            //no need of below I do my own tests and putting enable false throws errors when clicking and not checking instead of just 
            //no taking care of the click
            //handButtonObjects.GetComponentsInChildren<Interactable>()[i].IsEnabled = false;
        }

    }

    private void Update()
    {
        
    }

    void clickedOnToggle()
    {
        //deactive and gray out or reactive and put white every other button of the furniture
        for (int i = 0; i < handButtons.Length; i++)
        {
            //if we were in manual, go to automatic
            if (toggleInManualTracking)
            {
                handButtons[i].transform.Find("IconAndText").GetComponentsInChildren<MeshRenderer>()[0].material.color = Color.gray;
                handButtons[i].transform.Find("IconAndText").GetComponentsInChildren<MeshRenderer>()[1].material.color = Color.gray;
                //handButtonObjects.GetComponentsInChildren<Interactable>()[i].IsEnabled = false;
            }

            //if we were in automatic, go to manual
            else
            {
                handButtons[i].transform.Find("IconAndText").GetComponentsInChildren<MeshRenderer>()[0].material.color = Color.white;
                handButtons[i].transform.Find("IconAndText").GetComponentsInChildren<MeshRenderer>()[1].material.color = Color.white;
                //handButtonObjects.GetComponentsInChildren<Interactable>()[i].IsEnabled = true;
            }
        }

        toggleInManualTracking = !toggleInManualTracking;
    }

    void clickedOnButton(PressableButtonHoloLens2 buttonClicked)
    {
        for (int i = 0; i < handButtons.Length; i++)
        {
            if (handButtons[i] != buttonClicked)
            {
                handButtons[i].transform.Find("IconAndText").GetComponentsInChildren<MeshRenderer>()[0].material.color = Color.gray;
                handButtons[i].transform.Find("IconAndText").GetComponentsInChildren<MeshRenderer>()[1].material.color = Color.gray;
                //handButtonObjects.GetComponentsInChildren<Interactable>()[i].IsEnabled = false;
            }
        }


        buttonClicked.transform.Find("IconAndText").GetComponentsInChildren<MeshRenderer>()[0].material.color = Color.white;
        buttonClicked.transform.Find("IconAndText").GetComponentsInChildren<MeshRenderer>()[1].material.color = Color.white;
        //buttonClicked.GetComponent<Interactable>().IsEnabled = true;

        trackingHub.setTarget(buttonClicked.gameObject.GetComponent<ModelTargetID>().id);
    }

    void updateButtons(PressableButtonHoloLens2 buttonClicked)
    {

        if (buttonClicked == toggleButton)
        {
            clickedOnToggle();

            trackingHub.updateManualTracking(toggleInManualTracking);
        }

        else
        {
            //if in manual
            //in theory no need to check that toggle button has been clicked because when it's the case
            //these buttons are disabled so we should not be able to click on them and trigger the event
            //ENABLE DOES NOT WORK APPARENTLY SO I NEED TO CHECK

            if (toggleInManualTracking)
            {
                clickedOnButton(buttonClicked);
            }
            
        }
    }

    // Send Mesh request to API
    public void generateFloorPlan(GameObject button)
    {
        model.startMeshGeneration(button);
    }

    public void clickAndChangeOthers(GameObject gameobject)
    {
        Debug.Log("Cliked on button" + gameobject.name);
        updateButtons(gameobject.GetComponent<PressableButtonHoloLens2>());
    }
}
