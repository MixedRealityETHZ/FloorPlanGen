using UnityEngine;

public class PlatformFinder : MonoBehaviour
{
    private AudioSource audioPlayer;

    [SerializeField]
    private GameObject objectToPlaceHandler;

    [SerializeField]
    private GameObject userInterface;

    [SerializeField]
    private GameObject handMenu;

    private Model model;

    public void Start()
    {
        model = GameObject.FindGameObjectsWithTag("Model")[0].GetComponent<Model>();
        audioPlayer = GetComponent<AudioSource>();
    }

    public void Reset()
    {
        // Place outline in front of user field of view
        model.moveOutlineInFrontOfUser();
    }

    public void Accept()
    {
        objectToPlaceHandler.SetActive(false);
        handMenu.SetActive(false);
        userInterface.SetActive(true);
        audioPlayer.Play();

        model.onConfirmOutline();
    }
}