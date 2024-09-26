using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private float rotationSpeed = 30f; // 控制旋转的速度
    private Vector3 lastMousePosition;
 
    void OnTriggerEnter(Collider collider)
    {
        Debug.LogError(collider.gameObject.tag);
    }
    void Update()
    {
        //if (Input.GetMouseButton(0)) // 按住左键时
        //{
        //    Vector3 mousePosition = Input.mousePosition;
        //    Vector3 delta = mousePosition - lastMousePosition;
        //    float rotationX = delta.y * rotationSpeed * Time.deltaTime;

        //    // 使用四元数来旋转
        //    Quaternion rotation = Quaternion.Euler(rotationX, 0, 0);
        //    transform.rotation = rotation * transform.rotation; // 更新旋转

        //    OutputInpectorEulers();
        //}

        //lastMousePosition = Input.mousePosition;
    }

    private void OutputInpectorEulers()
    {
        Vector3 angle = transform.eulerAngles;
        float x = angle.x;
        float y = angle.y;
        float z = angle.z;

        if (Vector3.Dot(transform.up, Vector3.up) >= 0f)
        {
            if (angle.x >= 0f && angle.x <= 90f)
            {
                x = angle.x;
            }
            if (angle.x >= 90f && angle.x <= 180)
            {
                x = angle.x;
                y = angle.y - 180f;
                z = angle.z - 180f;
                Debug.LogError("Y:" + y + "   angleY" + angle.y);
            }
            if (angle.x >= 270f && angle.x <= 360f)
            {
                x = angle.x - 360f;
            }
        }
        if (Vector3.Dot(transform.up, Vector3.up) < 0f)
        {
            if (angle.x >= 0f && angle.x <= 90f)
            {
                x = 180 - angle.x;
            }
            if (angle.x >= 90f && angle.x <= 180)
            {
                x = 180 - angle.x;
                y = 180f - angle.y;
                z = 180f - angle.z;
            }
            if (angle.x >= 270f && angle.x <= 360f)
            {
                x = 180 - angle.x;
            }
        }

        if (angle.y > 180)
        {
            y = angle.y - 360f;
        }

        if (angle.z > 180)
        {
            z = angle.z - 360f;
        }

        //Debug.LogError(" Inspector Euler:  " + x + " , " + y + " , " + z + "  angle:" + angle.ToString());
        //Debug.LogError(" Inspector Euler:  " + Mathf.Round(x) + " , " + Mathf.Round(y) + " , " + Mathf.Round(z));
    }
}
