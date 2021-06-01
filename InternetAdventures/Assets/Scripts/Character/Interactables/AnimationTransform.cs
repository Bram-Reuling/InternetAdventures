using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class AnimationTransform : MonoBehaviour
{
    [SerializeField] private GameObject animationGameObject;

    private void Update()
    {
        transform.position = animationGameObject.transform.position;
        transform.rotation = animationGameObject.transform.rotation;
    }
}
