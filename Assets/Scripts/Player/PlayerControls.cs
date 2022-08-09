using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public GameObject bombPrefab;
    private float inverseMoveTime;
    private Rigidbody2D rb2d;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    private Vector2 facingVector;
    [SerializeField]
    private Transform checkSource;
    [SerializeField]
    private Collider2D col2d;

    private Inventory inventory;

    public Vector2Int gridPosition;

    public bool isTakingDamage = false;

    public AudioClip bumpSound;
    public AudioClip[] footStepSounds;

    //TOUCH CONTROLS
    private Vector3 touchStart;
    private Vector3 touchEnd;
    private float dragDistance;

    private void Start()
    {
        dragDistance = Screen.height * 15 / 100;
        rb2d = GetComponent<Rigidbody2D>();
        //spriteRenderer = GetComponent<SpriteRenderer>();
        facingVector = Vector2.right;
        inverseMoveTime = 1f / GameManager.turnTime;
        gridPosition = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        inventory = GameManager.instance.GetComponent<Inventory>();
    }

    private void Update()
    {
        if (GameManager.instance.gameState == GameManager.GameState.NORMAL)
        {
            ManageInput();
        }
    }

    private void ManageInput()
    {
        if (!GameManager.instance.playerTurn)
        {
            return;
        }

        //
        Vector2 movementVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (movementVector != Vector2.zero)
        {
            if(movementVector.x != 0)
            {
                movementVector.y = 0;
            }
            facingVector = movementVector;

            spriteRenderer.flipX = facingVector.x < 0;

            AttemptMove(gridPosition + movementVector);

            return;
        }

        if(Input.GetKeyDown(KeyCode.B))
        {
            PlaceBomb();
        }else if(Input.GetKeyDown(KeyCode.X))
        {
            ExitDunegon();
        }

        //Controller Controls

        if(Input.GetKeyDown("joystick button 0"))
        {
            PlaceBomb();
        }else if(Input.GetKeyDown("joystick button 1"))
        {
            
        }else if(Input.GetKeyDown("joystick button 2"))
        {
            ExitDunegon();
        }
        else if (Input.GetKeyDown("joystick button 3"))
        {
            Debug.Log("You pushed the Y button");
            ToggleInventory();
        }
        else if (Input.GetKeyDown("joystick button 4"))
        {

        }
        else if (Input.GetKeyDown("joystick button 5"))
        {

        }
        else if (Input.GetKeyDown("joystick button 6"))
        {

        }


        //TOUCH CONTROLS
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if(touch.phase == TouchPhase.Began)
            {
                touchStart = touch.position;
                touchEnd = touch.position;
            }else if(touch.phase == TouchPhase.Moved)
            {
                touchEnd = touch.position;
            }else if(touch.phase == TouchPhase.Ended)
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
                    if (movementVector != Vector2.zero)
                    {
                        facingVector = movementVector;
                        AttemptMove(gridPosition + movementVector);
                    }
                }
                else
                {   //It's a tap as the drag distance is less than 20% of the screen height
                    Debug.Log("Tap");
                    PlaceBomb();
                }
            }
        }else if(Input.touchCount == 2)
        {
            ExitDunegon();
        }

    }

    private void AttemptMove(Vector2 newPosition)
    {
        if (!GameManager.instance.playerTurn)
            return;

        Vector2 movementVector = newPosition - gridPosition;

        col2d.enabled = false;
        RaycastHit2D hit = Physics2D.Raycast(checkSource.position, movementVector, 1f);
        col2d.enabled = true;

        bool isBlocked = false;

        if(hit.transform != null)
        {
            if (hit.transform.GetComponent<Interactable>())
            {
                isBlocked = hit.transform.GetComponent<Interactable>().OnInteract();
            }
            else if(hit.transform.GetComponent<EnemyMovement>())
                {
                    isBlocked = true;
            }else if(hit.transform.GetComponent<Hazard>())
            {
                Hazard hazard = hit.transform.GetComponent<Hazard>();
                isBlocked = hazard.preventMovement;
                hazard.OnPlayerContact();
            }
            else
            {
                Debug.Log("I hit a " + hit.transform.name);
            }
            
        }

        if (isBlocked)
        {
            AudioManager.instance.PlaySound(bumpSound);
            return;
        }

        AudioManager.instance.PlaySound(footStepSounds[Random.Range(0, footStepSounds.Length)]);

        GameManager.instance.playerTurn = false;

        if(GameManager.instance.mapTiles[(int)newPosition.x, (int)newPosition.y] == 0)
        {
            StartCoroutine(SmoothMovement(newPosition, true));
        }
        else if(GameManager.instance.mapTiles[(int)newPosition.x, (int)newPosition.y] == 1)
        {
            if(newPosition.x >= 1 && newPosition.x < MapBuilder.instance.width-1 && newPosition.y >= 1 && newPosition.y < MapBuilder.instance.height-1)
            {
                MapBuilder.instance.DestroyWallAtPosition(newPosition);
                ResolvePosition();
            }
            else
            {
                AudioManager.instance.PlaySound(bumpSound);
                ResolvePosition();
            }
        }
        else
        {
            AudioManager.instance.PlaySound(bumpSound);
            ResolvePosition();
        }

    }

    private void PlaceBomb()
    {
        if (!GameManager.instance.playerTurn)
            return;

        if (GameManager.instance.ActiveBombCount() >= GameManager.instance.maxBombs)
            return;

        StartCoroutine(RenderBomb());
    }

    private IEnumerator RenderBomb()
    {
        GameManager.instance.playerTurn = false;

        GameObject newBomb = Instantiate(bombPrefab, transform.position, Quaternion.identity) as GameObject;

        yield return new WaitForSeconds(GameManager.turnTime);

        ResolvePosition();
    }

    private void ExitDunegon()
    {
        DialogManager.instance.ShowQuestion("Exit the cave?", () =>
        {
            GameManager.instance.LoadTownScene();
        }, () => {
            Debug.Log("That's a no on the exit!");
        });
    }

    private void ToggleInventory()
    {
        Debug.Log("Toggling the inventory");
        if(inventory.isActive)
        {
            GameManager.instance.uiManager.HideInventory();
        }
        else
        {
            GameManager.instance.uiManager.ShowInventory();
        }
    }

    public void Knockback(Transform source)
    {
        float xDif = transform.position.x - source.position.x;
        float yDif = transform.position.y - source.position.y;
        int x = 0;
        int y = 0;
        if (Mathf.Abs(xDif) > Mathf.Abs(yDif))
        {
            x = xDif > 0 ? 1 : -1;
        }
        else
        {
            y = yDif > 0 ? 1 : -1;
        }
        if (GameManager.instance.mapTiles[(int)transform.position.x + x, (int)transform.position.y + y] == 0)
        {
            StartCoroutine(SmoothMovement(new Vector2(transform.position.x + x, transform.position.y + y), false));
        }
    }

    private IEnumerator SmoothMovement(Vector2 endPosition, bool playerTurn)
    {
        Vector2 startPosition = gridPosition;
        float elapsedTime = 0f;
        float duration = GameManager.turnTime;

        while(elapsedTime < duration)
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

        if(isTakingDamage)
        {
            isTakingDamage = false;
        }

        if (playerTurn)
            ResolvePosition();
    }

    public void TakeDamage(Transform source, bool knockback)
    {
        isTakingDamage = true;
        StartCoroutine(Flicker(.5f));
        if(knockback)
        {
            Knockback(source);
        }
    }

    private IEnumerator Flicker(float duration)
    {
        int frequency = 3;
        int count = 0;
        float elapsedTime = 0f;
        while(elapsedTime < duration)
        {
            elapsedTime += duration;
            if(count % frequency == 0)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
            }
            yield return null;
        }
        spriteRenderer.enabled = true;
    }

    private void ResolvePosition()
    {
        if (transform.position == GameManager.instance.exit.position)
        {
            GameManager.instance.LoadNextLevel();
        }
        else
        {
            for (int i = 0; i < GameManager.instance.pickups.Count; i++)
            {
                if(transform.position == GameManager.instance.pickups[i].transform.position)
                {
                    GameManager.instance.pickups[i].OnPickup();
                }
            }
            GameManager.instance.EndPlayerTurn();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawRay(checkSource.position, facingVector * 1.5f);
    }
}
