using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;
using UnityEngine.Events;

public class PlatformFinder : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Sound that should be played when the conform prompt is displayed")]
    private AudioSource locationFoundSound;

    [SerializeField]
    private RadialView radialView;

    [SerializeField]
    private GameObject objectToPlaceHandler;

    [SerializeField]
    private GameObject userInterface;

    [SerializeField]
    private GameObject handMenu;

    private float delayMoment;
    private float initTime;
    private SolverHandler solverHandler;

    // Awake is called during the loading
    private void Awake()
    {
        solverHandler = radialView.GetComponent<SolverHandler>();
        radialView.enabled = true;
        initTime = Time.time + 2;
        handMenu.SetActive(true);
    }

    private void OnEnable()
    {
        Reset();
    }

    private void Update()
    {
        CheckRadialViewDisable();
    }

    public void Reset()
    {
        solverHandler.enabled = true;
        radialView.enabled = true;
        initTime = Time.time + 2;
        userInterface.SetActive(false);
        objectToPlaceHandler.SetActive(true);
        handMenu.SetActive(true);
    }

    public void Accept()
    {
        objectToPlaceHandler.SetActive(false);
        gameObject.SetActive(false);
        userInterface.SetActive(true);
        handMenu.SetActive(false);
        locationFoundSound.Play();
    }

    private void CheckRadialViewDisable()
    {
        if (Time.time > initTime && radialView.enabled)
        {
            radialView.enabled = false;
            solverHandler.enabled = false;
            delayMoment = Time.time + 2;
        }
    }
}