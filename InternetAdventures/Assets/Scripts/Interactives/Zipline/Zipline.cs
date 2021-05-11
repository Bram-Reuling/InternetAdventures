using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Zipline : MonoBehaviour
{
    [SerializeField] private GameObject _post1, _post2;
    [SerializeField] private float yOffset;
    
    private void Start()
    {
        
    }

    private void Update()
    {
        DrawDebugInfo();
    }
    
    public Vector3 GetDirectionVector()
    {
        return _post2.transform.position - _post1.transform.position;
    }

    public Vector3 GetNormalizedDirectionVector()
    {
        return GetDirectionVector().normalized;
    }

    private void DrawDebugInfo()
    {
        Debug.DrawLine(_post2.transform.position, _post1.transform.position, Color.magenta);
    }
}
