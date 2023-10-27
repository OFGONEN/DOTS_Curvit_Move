using System.Collections.Generic;
using UnityEngine;

public class LineRendererWayEntityReference : MonoBehaviour
{
    public static Dictionary<uint, LineRendererWayEntityReference> ReferenceDictionary;
    
    [SerializeField]
    private LineRenderer lineRenderer;

    public void SetPosition(int index, Vector3 position)
    {
        lineRenderer.SetPosition(index, position);
    }

    public void SetPositionCount(int count)
    {
        lineRenderer.positionCount = count;
    }
}