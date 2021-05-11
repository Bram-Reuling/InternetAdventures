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
        return transform.TransformDirection((_post2.transform.position + yOffset) - (_post1.transform.position + yOffset));
    }

    public Vector3 GetNormalizedDirectionVector()
    {
        return GetDirectionVector().normalized;
    }

    public Vector3 GetShortestVectorToLine(in Vector3 pPlayerPosition)
    {
        float scalarProjection = Vector3.Dot(GetNormalizedDirectionVector(), pPlayerPosition - transform.TransformDirection(_post1.transform.position + yOffset));
        Vector3 vecOnLine = transform.TransformDirection(_post1.transform.position + yOffset) + GetNormalizedDirectionVector() * scalarProjection;
        Vector3 vecToPlayer = pPlayerPosition - vecOnLine;
        Debug.DrawRay(vecOnLine, vecToPlayer, Color.blue);
        return vecToPlayer;
    }

    public bool ZiplineUsable(in Vector3 pPlayerPosition)
    {
        float scalarProjection = Vector3.Dot(GetNormalizedDirectionVector(), pPlayerPosition - transform.TransformDirection(_post1.transform.position + yOffset));
        return scalarProjection > 0 && scalarProjection < GetDirectionVector().magnitude;
    }

    private void DrawDebugInfo()
    {
        Debug.DrawLine(_post2.transform.position + yOffset, _post1.transform.position + yOffset, Color.magenta);
        GetShortestVectorToLine(GameObject.FindWithTag("Character").transform.position);
    }
}
