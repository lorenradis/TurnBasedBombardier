using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            uiManager = GetComponent<UIManager>();
            inventory = GetComponent<Inventory>();
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
        enemies.Clear();
    }

    public enum GameState { INTRO, NORMAL, MENU, LOADING, PAUSE }
    public GameState gameState;
    private void ChangeState(GameState newState)
    {
        if (gameState != newState)
        {
            gameState = newState;
        }
    }

    private void ChangeGameState(GameState newState)
    {
        if(gameState != newState)
        {
            gameState = newState;
        }
    }

    public enum StatusEffect { NONE, POISON, SLOW, STUN, CONFUSE, BLIND }
    public StatusEffect status;

    private int statusDuration = 0;

    public void ApplyStatus(StatusEffect newStatus, int duration)
    {
        statusDuration = duration;
        status = newStatus;
    }

    public static int actThreshhold = 100;
    public static float turnTime = .2f;

    public UIManager uiManager;
    public Inventory inventory;

    public int[,] mapTiles;

    public List<EnemyMovement> enemies = new List<EnemyMovement>();
    public List<Pickup> pickups = new List<Pickup>();
    private List<Bomb> activeBombs = new List<Bomb>();
    public int ActiveBombCount() { return activeBombs.Count; }

    public Transform exit;

    public bool playerTurn = false;
    public int level = 1;
    public int hp = 2;
    public int maxHP = 3;
    public int maxHPCap = 12;
    public int sightRange = 7;
    public float bombRadius = 1.5f;
    private float smallRadius = 1.5f;
    private float medRadius = 2.5f;
    private float largeRadius = 4.5f;
    public int bombTime = 4;
    public int bombDamage = 2;
    public int maxBombs = 1;
    public int money;
    public int kills;
    public int speed = 10;
    public int timeWaited = 0;

    public bool hasCompass = false;

    public delegate void OnHPChanged();
    public static OnHPChanged onHPChangedCallback;
    public delegate void OnMoneyChanged();
    public static OnMoneyChanged onMoneyChangedCallback;
    public delegate void OnKillsChanged();
    public static OnKillsChanged onKillsChangedCallback;

    public delegate void OnPlayerTurnEnd();
    public static OnPlayerTurnEnd onPlayerTurnEndCallback;

    public Transform player;

    private void Update()
    {
        Manageinput();    
    }

    private void Manageinput()
    {
        switch(gameState)
        {
            case GameState.INTRO:
                if(Input.anyKeyDown || Input.touchCount > 0)
                {
                    StartGame();
                }
                break;
            case GameState.NORMAL:
                if (Input.GetKeyDown(KeyCode.P))
                {
                    PauseGame();
                }
                if (Input.GetKeyDown("joystick button 7"))
                {
                    PauseGame();
                }
                break;
            case GameState.MENU:

                break;
            case GameState.LOADING:

                break;
            case GameState.PAUSE:
                if (Input.GetKeyDown(KeyCode.P))
                {
                    PauseGame();
                }
                if (Input.GetKeyDown("joystick button 7"))
                {
                    PauseGame();
                }
                break;
            default:

                break;
        }
    }

    public void AddEnemyToList(EnemyMovement enemy)
    {
        if(!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    public void RemoveEnemyFromList(EnemyMovement enemy)
    {
        if(enemies.Contains(enemy))
        {
            enemies.Remove(enemy); 
        }
    }

    public void AddPickupToList(Pickup pickup)
    {
        if(!pickups.Contains(pickup))
        {
            pickups.Add(pickup);
        }
    }

    public void RemovePickupFromList(Pickup pickup)
    {
        if(pickups.Contains(pickup))
        {
            pickups.Remove(pickup);
        }
    }

    public void AddBombToList(Bomb bomb)
    {
        if(!activeBombs.Contains(bomb))
        {
            activeBombs.Add(bomb);
        }
    }

    public void RemoveBombFromList(Bomb bomb)
    {
        if(activeBombs.Contains(bomb))
        {
            activeBombs.Remove(bomb);
        }
    }

    public void EndPlayerTurn()
    {
        if(MapBuilder.instance != null)
        {
            MapBuilder.instance.RevealTiles(player.position, sightRange);
        }

        for (int i = 0; i < activeBombs.Count; i++)
        {
            activeBombs[i].CountDown();
        }

        float speedMod = status == StatusEffect.SLOW ? .5f : 1f;

        while(timeWaited < actThreshhold)
        {
            timeWaited += (int)(speed * speedMod);
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].TickSpeed();
            }
        }

        timeWaited -= actThreshhold;

        if(statusDuration > 0)
        {
            statusDuration--;
            if(statusDuration <= 0)
            {
                ApplyStatus(StatusEffect.NONE, 0);
            }
        }

        if (onPlayerTurnEndCallback != null)
            onPlayerTurnEndCallback.Invoke();

        StartCoroutine(RenderEnemyTurns());
    }

    private IEnumerator RenderEnemyTurns()
    {
        List<EnemyMovement> activeEnemies = new List<EnemyMovement>();

        for (int i = 0; i < enemies.Count; i++)
        {
            if(enemies[i].IsReady())
            {
                activeEnemies.Add(enemies[i]);
            }
        }
        float maxTime = 1f;
        while (activeEnemies.Count > 0)
        {
            for (int i = activeEnemies.Count - 1; i >= 0; i--)
            {
                activeEnemies[i].ChooseAction();

                if (!activeEnemies[i].IsReady())
                {
                    activeEnemies.RemoveAt(i);
                }

                yield return null;

            }
            maxTime -= Time.deltaTime;
            if (maxTime < 0)
            {
                Debug.Log("We left the while loop early");
                break;

            }
            yield return new WaitForSeconds(turnTime);
        }
        if(MapBuilder.instance != null)
        {
            MapBuilder.instance.GetComponent<MiniMap>().DrawMiniMap();
        }
        playerTurn = true;
    }

    public void GainMoney(int amount)
    {
        money += amount;
        if (onMoneyChangedCallback != null)
            onMoneyChangedCallback.Invoke();
    }

    public void GainHP(int amount)
    {
        hp += amount;
        if (hp > maxHP)
            hp = maxHP;
        if (onHPChangedCallback != null)
            onHPChangedCallback.Invoke();
    }

    public void GainSpeed(int amount)
    {
        speed += amount;
    }

    public void IncreaseBombStrength(int amount)
    {
        bombDamage += amount;
    }

    public void IncreaseBombRange(float amount)
    {
        if(bombRadius == smallRadius)
        {
            bombRadius = medRadius;
        }else if(bombRadius == medRadius)
        {
            bombRadius = largeRadius;
        }
    }

    public void IncreaseMaxHP(int amount)
    {
        maxHP += amount;
        if (maxHP > maxHPCap)
            maxHP = maxHPCap;
        if (onHPChangedCallback != null)
            onHPChangedCallback.Invoke();
    }

    public void IncrementKills()
    {
        kills++;
        if (onKillsChangedCallback != null)
            onKillsChangedCallback.Invoke();
    }

    public void TakeDamage(int amount)
    {
        if (player.GetComponent<PlayerControls>().isTakingDamage)
            return;

        player.GetComponent<PlayerControls>().isTakingDamage = true;
        hp -= amount;
        if(onHPChangedCallback != null)
            onHPChangedCallback.Invoke();
        if(hp <= 0)
        {
            hp = 0;
            Die();
        }
    }

    public void ExitDungeon()
    {

    }

    private void Die()
    {
        player.gameObject.SetActive(false);

        Invoke("RestartGame", 2f);
    }

    public void EnterMenuState()
    {
        ChangeGameState(GameState.MENU);
    }

    public void ExitMenuState()
    {
        ChangeGameState(GameState.NORMAL);
    }

    public void PauseGame()
    {
        if(gameState == GameState.PAUSE)
        {
            ChangeGameState(GameState.NORMAL);
        }
        else
        {
            ChangeGameState(GameState.PAUSE);
        }
    }

    private void RestartGame()
    {
        StartCoroutine(FadeToNewScene("MainMenuScene"));
        maxHP = 3;
        hp = maxHP;
        money = 0;
        level = 1;
        kills = 0;
        bombRadius = 1.5f;
        bombDamage = 2;
        bombTime = 4;
        uiManager.RestartGame();
    }

    public void StartGame()
    {
        StartCoroutine(FadeToNewScene("CaveScene"));
        
        uiManager.StartGame();
    }

    private IEnumerator FadeToNewScene(string sceneName)
    {
        ChangeState(GameState.LOADING);
        uiManager.FadeOut();
        yield return new WaitForSeconds(.5f);

        SceneManager.LoadScene(sceneName);

        yield return new WaitForSeconds(.25f);

        uiManager.FadeIn();
        yield return new WaitForSeconds(.25f);

        if(sceneName == "MainMenuScene")
        {
            ChangeState(GameState.INTRO);
        }
        else
        {
            ChangeState(GameState.NORMAL);
        }
    }

    public void LoadNextLevel()
    {
        level++;
        uiManager.DisplayLevel();
        StartCoroutine(FadeToNewScene("CaveScene"));
    }

    public void LoadNewScene(string sceneToLoad)
    {
        StartCoroutine(FadeToNewScene(sceneToLoad));
    }
}