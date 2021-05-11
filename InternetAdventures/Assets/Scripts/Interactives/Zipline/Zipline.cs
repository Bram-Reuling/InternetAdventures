using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Zipline : MonoBehaviour
{
    [SerializeField] private GameObject _post1, _post2;
    [SerializeField] private Vector3 yOffset;
    
    private void Update()
    {
        DrawDebugInfo();
    }
    
    public Vector3 GetDirectionVector()
    {
        return _post2.transform.position + yOffset - _post1.transform.position + yOffset;
    }

    public Vector3 GetNormalizedDirectionVector()
    {
        return GetDirectionVector().normalized;
    }

    private void DrawDebugInfo()
    {
        Debug.DrawLine(_post2.transform.position + yOffset, _post1.transform.position + yOffset, Color.magenta);
    }
}
