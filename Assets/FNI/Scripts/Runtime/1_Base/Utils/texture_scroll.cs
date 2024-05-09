using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Texture_scroll : MonoBehaviour
{
    public float ScrollX = 0.01f;
    public float ScrollY = 0.01f;

    public Renderer myRenderer;

    // Start is called before the first frame update
    void Start()
    {
        if (myRenderer == null)
            myRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (myRenderer == null)
            return;
        
        myRenderer.material.mainTextureOffset = new Vector2(ScrollX, ScrollY) * Time.time;
    }
}
