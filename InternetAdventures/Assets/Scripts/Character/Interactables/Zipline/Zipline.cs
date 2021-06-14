using UnityEngine;

public class Zipline : MonoBehaviour
{
    //Public
    [SerializeField] private GameObject _post1, _post2;
    
    [Tooltip("This sets the distance when the character should be thrown off the zipline." +
             "0 throws off at the actual post, 1 throws off one unit before.")]
    [SerializeField] private float throwoffDistance;
    
    //Info: 'yOffset' is the offset on the y-axis from the center of a zipline post. This way the 'actual'
    //zipline can be moved up/down the post.
    [SerializeField] private Vector3 yOffset;
    [SerializeField] private bool drawDebug;
    
    private Vector3 GetDirectionVector()
    {
        //Returns the vector from one post to the second post - represents the zipline
        //Note: the 'yOffset' is taken into account here.
        return transform.TransformDirection(_post2.transform.position + yOffset - (_post1.transform.position + yOffset));
    }

    public Vector3 GetNormalizedDirectionVector()
    {
        return GetDirectionVector().normalized;
    }

    public Vector3 GetShortestVectorToLine(in Vector3 pPlayerPosition)
    {
        float scalarProjection = GetScalarProjection(pPlayerPosition);
        //Gets post position in world space and calculates vector from that post to the second using the projection value as scale.
        Vector3 vecOnLine = transform.TransformDirection(_post1.transform.position + yOffset) + GetNormalizedDirectionVector() * scalarProjection;
        //Calculates the vector from that point to the player
        Vector3 vecToPlayer = pPlayerPosition - vecOnLine;
        Debug.DrawRay(vecOnLine, vecToPlayer, Color.blue);
        return vecToPlayer;
    }

    private float GetScalarProjection(in Vector3 pPlayerPosition)
    {
        //Note: The position of the post is brought to world space by using 'TransformDirection'.
        return Vector3.Dot(GetNormalizedDirectionVector(), pPlayerPosition - transform.TransformDirection(_post1.transform.position + yOffset));
    }

    public bool ZiplineUsable(in Vector3 pPlayerPosition)
    {
       //Info: Checks whether the character is in front of the first or past the second post. 
       float scalarProjection = GetScalarProjection(pPlayerPosition);
        return scalarProjection > 0 && scalarProjection < GetDirectionVector().magnitude;
    }
    
    public bool ZiplineMovementValid(in Vector3 pPlayerPosition)
    {
        //Info: This is called while already being on the zipline. This methods provide functionality so that the character is thrown of the zipline
        //1 unit before the post and not at the actual post, to provide better movement.
        float scalarProjection = GetScalarProjection(pPlayerPosition);
        return scalarProjection > throwoffDistance && scalarProjection < GetDirectionVector().magnitude - throwoffDistance;
    }
    
    private void OnDrawGizmos()
    {
        if(!drawDebug) return;
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_post2.transform.position + yOffset, _post1.transform.position + yOffset);
        //GetShortestVectorToLine(GameObject.FindWithTag("Character").transform.position);
    }
}
