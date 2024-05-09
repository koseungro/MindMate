using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyControl : MonoBehaviour
{
    private Camera mainCam;
    private Vector3 vec = new Vector3();

    private float cameraY;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        cameraY = (mainCam.transform.rotation.eulerAngles.y < 180f ? mainCam.transform.rotation.eulerAngles.y : mainCam.transform.rotation.eulerAngles.y - 360f) / 6;

        vec.x = cameraY;
        transform.position = new Vector3(cameraY, 0, 0);
    }
}
