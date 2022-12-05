using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

public class LinePlacer : MonoBehaviour
{
    [SerializeField]
    private GameObject objectToPlace;

    public void PlaceObject(MixedRealityPose pose)
    {
        var obj = Instantiate(objectToPlace, gameObject.transform);
        obj.transform.position = pose.Position;
        obj.transform.rotation *= pose.Rotation;
    }
}
