/*===============================================================================
Copyright (c) 2021 PTC Inc. All Rights Reserved.

Vuforia is a trademark of PTC Inc., registered in the United States and other
countries.
===============================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Vuforia;
using TMPro;

public class VuforiaStateInfo : MonoBehaviour
{
    public GameObject TextObject;

    const string ACTIVE_TARGETS_TITLE = "<b>Tracking and position of targets: </b>";

    List<ObserverBehaviour> observers = new List<ObserverBehaviour>();

    string mTargetStatusInfo;

    readonly Dictionary<string, string> mTargetsStatus = new Dictionary<string, string>();

    void Start()
    {
        VuforiaApplication.Instance.OnVuforiaStarted += OnVuforiaStarted;
    }

    void OnDestroy()
    {
        VuforiaApplication.Instance.OnVuforiaStarted -= OnVuforiaStarted;
    }

    void Update()
    {
        //update only observers where tracking found
        foreach (var observer in observers)
        {
            //check if position has change to be faster
            TargetStatusChanged(observer);  //can i delete the call to target status change in unity?
        }
    }

    void OnVuforiaStarted()
    {
        UpdateText(); //will write completeinfo in description
    }

    /// <summary>
    /// Public method to be called by an EventHandler's Lost/Found Events
    /// </summary>
    /// <param name="observerBehaviour"></param>
    /// 
    // check if i can just do GetComponent<ObserverBehaviour>() and so not add it to each thing
    public void TargetStatusChanged(ObserverBehaviour observerBehaviour) 
    {
        var status = GetStatusString(observerBehaviour.TargetStatus, observerBehaviour.transform, observerBehaviour.GetComponentInChildren<Node>().getObjectSize().ToString("0.0"));
        

        var targetName = observerBehaviour.TargetName;
        //fill dict for each new or existing tracked target
        if (mTargetsStatus.ContainsKey(targetName))
            mTargetsStatus[targetName] = status;
        else
            mTargetsStatus.Add(targetName, status);

        //fill description
        UpdateText();
    }

    public void TargetFound(ObserverBehaviour observerBehaviour)
    {
        Debug.Log("Heyyyy Found " + observerBehaviour.TargetName);
        observers.Add(observerBehaviour);
    }

    public void TargetLost(ObserverBehaviour observerBehaviour)
    {
        Debug.Log("Heyyyy Lost " + observerBehaviour.TargetName);
        observers.Remove(observerBehaviour);
    }

    void UpdateText()
    {
        mTargetStatusInfo = GetTargetsStatusInfo(); //if start = "", else = description of targetstatus keys/values

        var completeInfo = ACTIVE_TARGETS_TITLE; //Tracking and position of targets:

        if (mTargetStatusInfo.Length > 0)
            completeInfo += $"\n{mTargetStatusInfo}";

        SampleUtil.AssignStringToTextComponent(TextObject ? TextObject : gameObject, completeInfo); //assign completeinfo to textobject (description)
    }

    string GetStatusString(TargetStatus targetStatus, Transform transform, string size)
    {
        /*Debug.Log(gameObject);
        Debug.Log(gameObject.ToString());*/
        

        return $"{targetStatus.Status} -- {targetStatus.StatusInfo} \nworld position (cm): {transform.position*100} \nsize (m²): {size}";
        
        //example Target status: Astronaut2 NO_POSE -- NOT_OBSERVED
    }
    
    string GetTargetsStatusInfo()
    {
        var targetsAsMultiLineString = "";
        
        foreach (var targetStatus in mTargetsStatus)
            targetsAsMultiLineString += "\n" + targetStatus.Key + ": " + targetStatus.Value;

        return targetsAsMultiLineString;
    }
}
