using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private float maxDist = 4f;
    public float moveSpeed = 8f;

    [SerializeField]
    private AudioClip ambientSound;
    public AudioClip AmbientSound { get { return ambientSound; } set { } }
    [SerializeField]
    private AudioClip bgmSound;
    public AudioClip BGMSound { get { return bgmSound; } set { } }

    private void Start()
    {
        if (GameManager.instance.player == null)
            return;
        float x = GameManager.instance.player.position.x;
        float y = GameManager.instance.player.position.y;
        transform.position = new Vector3(x, y, -10f);
    }

    void LateUpdate()
    {
        if (GameManager.instance.player == null)
            return;
        float dist = Vector2.Distance(transform.position, GameManager.instance.player.position);
        float x = 0f;
        float y = 0f;
        if(dist > maxDist)
        {
            x = GameManager.instance.player.position.x;
            y = GameManager.instance.player.position.y;
        }
        else
        {
            x = Mathf.Lerp(transform.position.x, GameManager.instance.player.position.x, moveSpeed * Time.deltaTime);
            y = Mathf.Lerp(transform.position.y, GameManager.instance.player.position.y, moveSpeed * Time.deltaTime);
        }
        transform.position = new Vector3(x, y, -10f);
    }
}
