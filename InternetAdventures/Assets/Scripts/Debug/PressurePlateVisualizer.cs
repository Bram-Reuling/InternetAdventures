using UnityEngine;

public class PressurePlateVisualizer : MonoBehaviour
{
    [SerializeField] private GameObject platform;
    [SerializeField] private GameObject target;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, 0.5f);
        Gizmos.DrawCube(target.transform.position, platform.transform.localScale);
        
        Gizmos.DrawLine(platform.transform.position, target.transform.position);
    }
}
