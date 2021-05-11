using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Zipline : MonoBehaviour
{
    [SerializeField] private GameObject _post1, _post2;
    [SerializeField] private Vector3 yOffset;
    [SerializeField] private bool drawDebugInfo;
    
    private void Update()
    {
        if(drawDebugInfo) DrawDebugInfo();
    }
    
    public Vector3 GetDirectionVector()
    {
        return transform.TransformDirection((_post2.transform.position + yOffset) - (_post1.transform.position + yOffset));
    }

    public Vector3 GetNormalizedDirectionVector()
    {
        return GetDirectionVector().normalized;
    }

    public Vector3 GetShortestVectorToLine(in Vector3 pPlayerPosition)
    {
        float scalarProjection = GetScalarProjection(pPlayerPosition);
        Vector3 vecOnLine = transform.TransformDirection(_post1.transform.position + yOffset) + GetNormalizedDirectionVector() * scalarProjection;
        Vector3 vecToPlayer = pPlayerPosition - vecOnLine;
        Debug.DrawRay(vecOnLine, vecToPlayer, Color.blue);
        return vecToPlayer;
    }

    private float GetScalarProjection(in Vector3 pPlayerPosition)
    {
        return Vector3.Dot(GetNormalizedDirectionVector(), pPlayerPosition - transform.TransformDirection(_post1.transform.position + yOffset));
    }

    public bool ZiplineUsable(in Vector3 pPlayerPosition)
    {
        float scalarProjection = GetScalarProjection(pPlayerPosition);
        return scalarProjection > 0 && scalarProjection < GetDirectionVector().magnitude;
    }
    
    public bool ZiplineMovementValid(in Vector3 pPlayerPosition)
    {
        float scalarProjection = GetScalarProjection(pPlayerPosition);
        return scalarProjection > 1 && scalarProjection < GetDirectionVector().magnitude - 1;
    }

    private void DrawDebugInfo()
    {
        Debug.DrawLine(_post2.transform.position + yOffset, _post1.transform.position + yOffset, Color.magenta);
        GetShortestVectorToLine(GameObject.FindWithTag("Character").transform.position);
    }
}
