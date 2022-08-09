using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public AudioClip hurtSound;
    public AudioClip healSound;
    public AudioClip coinSound;

    public Button StartGameButton;

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
    public AudioManager audioManager;
    public UpgradeManager upgradeManager;

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
    public float bombRadius = 2f;
    private float smallRadius = 2f;
    private float medRadius = 3f;
    private float largeRadius = 4f;
    private float maxRadius = 5f;
    public int bombTime = 4;
    public int bombDamage = 2;
    public int maxBombs = 1;
    public int money;
    public int kills;
    public int speed = 10;
    public int timeWaited = 0;

    //upgrades
    public bool hasCompass = false;
    public bool hasMap = false;

    public delegate void OnHPChanged();
    public static OnHPChanged onHPChangedCallback;
    public delegate void OnMoneyChanged();
    public static OnMoneyChanged onMoneyChangedCallback;
    public delegate void OnKillsChanged();
    public static OnKillsChanged onKillsChangedCallback;

    public delegate void OnPlayerTurnEnd();
    public static OnPlayerTurnEnd onPlayerTurnEndCallback;

    [SerializeField]
    private SceneInfo caveSceneInfo;
    [SerializeField]
    private SceneInfo townSceneInfo;
    [SerializeField]
    private SceneInfo mainMenuSceneInfo;

    public Transform player;

    private void Start()
    {
        UpgradeManager.instance = upgradeManager;
        upgradeManager.SetupUpgrades();
        audioManager = GetComponent<AudioManager>();
        AudioManager.instance = audioManager;
    }

    private void Update()
    {
        Manageinput();    
    }

    private void Manageinput()
    {
        switch(gameState)
        {
            case GameState.INTRO:
                EventSystem.current.SetSelectedGameObject(StartGameButton.gameObject);
                if(Input.anyKeyDown || Input.touchCount > 0)
                {
                    //StartGame();
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
            }
            yield return new WaitForSeconds(turnTime * .51f);
            maxTime -= Time.deltaTime;
            if (maxTime < 0)
            {
                Debug.Log("We left the while loop early");
                break;

            }
            yield return null;
        }

        yield return new WaitForSeconds(turnTime * .5f);

        if (MapBuilder.instance != null)
        {
            MapBuilder.instance.GetComponent<MiniMap>().DrawMiniMap();
        }
        playerTurn = true;
    }

    public void GainMoney(int amount)
    {
        audioManager.PlaySound(coinSound);
        money += amount;
        if (onMoneyChangedCallback != null)
            onMoneyChangedCallback.Invoke();
    }

    public void SpendMoney(int amount)
    {
        money -= amount;
        if (onMoneyChangedCallback != null)
            onMoneyChangedCallback.Invoke();
    }

    public void GainHP(int amount)
    {
        audioManager.PlaySound(healSound);
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

    public void IncreaseBombRange()
    {
        if(bombRadius == smallRadius)
        {
            bombRadius = medRadius;
        }else if(bombRadius == medRadius)
        {
            bombRadius = largeRadius;
        }else if(bombRadius == largeRadius)
        {
            bombRadius = maxRadius;
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

        audioManager.PlaySound(hurtSound);

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
        StartCoroutine(FadeToNewScene(mainMenuSceneInfo));
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
        if (gameState != GameState.INTRO)
            return;
        StartCoroutine(FadeToNewScene(caveSceneInfo));
        
        uiManager.StartGame();
    }

    public void LoadNewScene(SceneInfo sceneToLoad)
    {
        StartCoroutine(FadeToNewScene(sceneToLoad));
    }

    private IEnumerator FadeToNewScene(SceneInfo sceneToLoad)
    {
        ChangeState(GameState.LOADING);
        uiManager.FadeOut();
        audioManager.PlayBGM(sceneToLoad.BGMClip);
        yield return new WaitForSeconds(.5f);

        SceneManager.LoadScene(sceneToLoad.sceneName);

        yield return new WaitForSeconds(.25f);

        

        uiManager.FadeIn();
        yield return new WaitForSeconds(.25f);

        if(sceneToLoad.sceneName == "MainMenuScene")
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
        StartCoroutine(FadeToNewScene(caveSceneInfo));
    }

    public void LoadTownScene()
    {
        StartCoroutine(FadeToNewScene(townSceneInfo));
    }
}