using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;
using UnityEngine.Events;

public class PlatformFinder : MonoBehaviour
{
    private AudioSource audioPlayer;

    [SerializeField]
    private GameObject objectToPlaceHandler;

    [SerializeField]
    private GameObject userInterface;

    [SerializeField]
    private GameObject handMenu;

    public void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
    }

    public void Reset()
    {
        // TODO: place 
    }

    public void Accept()
    {
        objectToPlaceHandler.SetActive(false);
        handMenu.SetActive(false);
        userInterface.SetActive(true);
        audioPlayer.Play();

        Model model = GameObject.FindGameObjectsWithTag("Model")[0].GetComponent<Model>();
        model.onConfirmOutline();
    }
}