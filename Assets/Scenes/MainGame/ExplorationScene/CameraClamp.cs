using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClamp : MonoBehaviour
{
    [SerializeField]
    private Transform target;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(
            // need to figure out values for these dynamically for each map? yes....
            Mathf.Clamp(target.position.x, -100f, 100f),
            Mathf.Clamp(target.position.y, -100f, 100f),
            transform.position.z
        );
    }
}
