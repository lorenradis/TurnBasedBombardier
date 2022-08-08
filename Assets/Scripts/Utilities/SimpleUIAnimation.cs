using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleUIAnimation : MonoBehaviour
{
    public float frameRate;

    public Sprite[] frames;

    public Image image;

    private float elapsedTime = 0f;

    private float frameTime;

    private int index;

    private void Start()
    {
        frameTime = frameRate / 60f;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= frameTime)
        {
            elapsedTime = 0f;
            index++;
            if( index >= frames.Length)
            {
                index = 0;
            }
            image.sprite = frames[index];
        }
    }
}
