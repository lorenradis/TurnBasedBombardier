using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldPlayerControls : MonoBehaviour
{

    private Rigidbody2D rb2d;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    private Vector2 facingVector;
    [SerializeField]
    private Transform checkSource;
    private Collider2D col2d;

    private Vector2Int gridPosition;

    public bool isTakingDamage = false;

    public enum PlayerState { IDLE, MOVE, TALK }
    public PlayerState playerState;
    private void ChangeState(PlayerState newState)
    {
        if(playerState != newState)
        {
            playerState = newState;
        }
    }

    //TOUCH CONTROLS
    private Vector3 touchStart;
    private Vector3 touchEnd;
    private float dragDistance;

    private void Start()
    {
        if (GameManager.instance != null)
            GameManager.instance.player = transform;
        dragDistance = Screen.height * 15 / 100;
        rb2d = GetComponent<Rigidbody2D>();

        facingVector = Vector2.right;
        gridPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
    }

    private void Update()
    {
        if(GameManager.instance != null)
        {
            if(GameManager.instance.gameState != GameManager.GameState.NORMAL)
            {
                return;
            }
        }
            ManageInput();
            spriteRenderer.flipX = facingVector.x < 0;
        
    }

    private void ManageInput()
    {
        if (playerState != PlayerState.IDLE)
            return;

        Vector2 movementVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        //TOUCH CONTROLS
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchStart = touch.position;
                touchEnd = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                touchEnd = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                touchEnd = touch.position;

                //Check if drag distance is greater than 20% of the screen height
                if (Mathf.Abs(touchEnd.x - touchStart.x) > dragDistance || Mathf.Abs(touchEnd.y - touchStart.y) > dragDistance)
                {//It's a drag
                 //check if the drag is vertical or horizontal
                    if (Mathf.Abs(touchEnd.x - touchStart.x) > Mathf.Abs(touchEnd.y - touchStart.y))
                    {   //If the horizontal movement is greater than the vertical movement...
                        if ((touchEnd.x > touchStart.x))  //If the movement was to the right)
                        {   //Right swipe
                            Debug.Log("Right Swipe");
                            movementVector = Vector2.right;
                        }
                        else
                        {   //Left swipe
                            Debug.Log("Left Swipe");
                            movementVector = Vector2.left;
                        }
                    }
                    else
                    {   //the vertical movement is greater than the horizontal movement
                        if (touchEnd.y > touchStart.y)  //If the movement was up
                        {   //Up swipe
                            Debug.Log("Up Swipe");
                            movementVector = Vector2.up;
                        }
                        else
                        {   //Down swipe
                            Debug.Log("Down Swipe");
                            movementVector = Vector2.down;
                        }
                    }
                }
                else
                {   //It's a tap as the drag distance is less than 20% of the screen height
                    Debug.Log("Tap");

                }
            }
        }

        if (movementVector != Vector2.zero)
        {
            if (movementVector.x != 0)
            {
                movementVector.y = 0;
            }
            facingVector = movementVector;
            AttemptMove(gridPosition + movementVector);
        }
    }

    private void AttemptMove(Vector2 newPosition)
    {
        Vector2 movementVector = newPosition - gridPosition;

        //col2d.enabled = false;
        RaycastHit2D hit = Physics2D.Raycast(checkSource.position, movementVector, 1f);
        //col2d.enabled = true;

        if (hit.transform != null)
        {
            if (hit.transform.GetComponent<Interactable>())
            {
                hit.transform.GetComponent<Interactable>().OnInteract();
                return;
            }
        }
        else
        {
            StartCoroutine(SmoothMovement(newPosition));
        }

    }

    private IEnumerator SmoothMovement(Vector2 endPosition)
    {
        Vector2 startPosition = gridPosition;
        float elapsedTime = 0f;
        float duration = GameManager.turnTime;

        ChangeState(PlayerState.MOVE);

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            t = t * t * (3f - 2f * t);
            Vector2 newPosition = Vector2.Lerp(startPosition, endPosition, t);
            transform.position = newPosition;
            yield return null;
        }

        gridPosition = new Vector2Int((int)endPosition.x, (int)endPosition.y);

        transform.position = new Vector3(gridPosition.x, gridPosition.y, 0);

        ChangeState(PlayerState.IDLE);
    }
}