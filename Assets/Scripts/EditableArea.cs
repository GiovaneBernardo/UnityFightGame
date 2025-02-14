using UnityEngine;


public class EditableArea : MonoBehaviour
{
    public enum EditableAreaType
    {
        EDITABLE_AREA_BOX,
        EDITABLE_AREA_SPHERE,
        EDITABLE_AREA_CYLINDER,
        EDITABLE_AREA_ANY_SHAPE
    }
    public EditableAreaType Type;
    public Vector3 BoxSize;
    public bool ShowSolid;

    void Start()
    {

    }

    void Update()
    {

    }
    public bool IsPointInsideAreaBox(Vector3 point)
    {
        bool isInside =
            point.x > -BoxSize.x && point.x < BoxSize.x &&
            point.y > -BoxSize.y && point.y < BoxSize.y &&
            point.z > -BoxSize.z && point.z < BoxSize.z;
        return isInside;
    }

    public Vector3 GetRandomPosition(bool onMaxHeight)
    {
        return new Vector3(
            Random.Range(-BoxSize.x / 2, BoxSize.x / 2) + transform.position.x,
            onMaxHeight ? BoxSize.y + transform.position.y : Random.Range(-BoxSize.y / 2, BoxSize.y / 2) + transform.position.y,
            Random.Range(-BoxSize.z / 2, BoxSize.z / 2) + transform.position.z);
    }
}