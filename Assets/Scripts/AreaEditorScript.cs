using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EditableArea))]
public class AreaEditorScript : Editor
{
    public void OnSceneGUI()
    {
        EditableArea area = (EditableArea)target;
        Transform transform = area.transform;

        Vector3 center = transform.position;
        Vector3 size = area.BoxSize;

        EditorGUI.BeginChangeCheck();

        Vector3 newSize = Handles.ScaleHandle(size, center, Quaternion.identity, 1f);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(area, "Resize Box");
            area.BoxSize = newSize - center;
        }

        Handles.DrawWireCube(center, size);

        if (area.ShowSolid)
        {
            transform.localScale = area.BoxSize;
            area.GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            area.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}