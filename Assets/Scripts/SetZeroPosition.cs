using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetZeroPosition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Đặt vị trí ban đầu của GameObject là (0, 0, 0)
        transform.position = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // Luôn cập nhật vị trí của GameObject về (0, 0, 0)
        transform.position = Vector3.zero;
    }
}
