using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterColor : MonoBehaviour
{
    private float m_RemainderTime = 0;
    private SpriteRenderer renderer;
    private float R = 0.7f;
    private float G = 0.7f;
    private float B = 0.0f;
    private float A = 0;
    private Color color;


    void Start()
    {
        this.m_RemainderTime = 0.0f;
        renderer = this.GetComponent<SpriteRenderer>();
    }
    void Update()
    {

        if (this.m_RemainderTime < 1.0f)
        {
            this.m_RemainderTime += Time.deltaTime;
        }
        else
        {
            R = UnityEngine.Random.Range(0.5f, 1.0f); 
            G = UnityEngine.Random.Range(0.0f, 0.5f); 
            B = UnityEngine.Random.Range(0.5f, 1.0f); 
            color = new Color(1.0f - R, 1.0f - G, 1.0f - B, 1.0f);
            renderer.material.SetColor("_TargetColor01", color);
            this.m_RemainderTime = 0;
        }
    }
}
