using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIManager : MonoBehaviour
{
    public Sprite heartSprite;
    public Sprite emptyHeartSprite;
    public Image[] hearts;

    public Animator sceneTransitionsAnimator;

    public GameObject playerInfoPanel;
    public GameObject mainMenuPanel;

    public TextMeshProUGUI moneyText;

    public TextMeshProUGUI levelText;

    public TextMeshProUGUI killsText;

    public GameObject inventoryPanel;
    public ItemSlot[] itemSlots;

    private Inventory inventory;

    [SerializeField]
    private GameObject shopKeeperPanel;
    [SerializeField]
    private ItemSlot[] shopItemSlots;
    private List<Item> shopKeeperInventory = new List<Item>();

    private void Start()
    {
        GameManager.onHPChangedCallback += DisplayHP;
        GameManager.onMoneyChangedCallback += DisplayMoney;
        GameManager.onKillsChangedCallback += DisplayKills;

        inventory = GameManager.instance.GetComponent<Inventory>();

        DisplayHP();
        DisplayMoney();
        DisplayLevel();
        DisplayKills();
    }

    public void StartGame()
    {
        Invoke("ActivatePlayerPanel", .5f);
        Invoke("DeactivateMainMenuPanel", .5f);
    }

    public void RestartGame()
    {
        DisplayHP();
        DisplayMoney();
        DisplayLevel();
        DeactivePlayerPanel();
        ActivateMainMenuPanel();
    }

    public void FadeOut()
    {
        sceneTransitionsAnimator.SetTrigger("wipeIn");
    }

    public void FadeIn()
    {
        sceneTransitionsAnimator.SetTrigger("wipeOut");
    }

    public void DeactivePlayerPanel()
    {
        playerInfoPanel.SetActive(false);
    }

    public void ActivatePlayerPanel()
    {
        playerInfoPanel.SetActive(true);
    }

    private void ActivateMainMenuPanel()
    {
        mainMenuPanel.SetActive(true);
    }

    private void DeactivateMainMenuPanel()
    {
        mainMenuPanel.SetActive(false);
    }

    private void DisplayHP()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i].enabled = true;
            if (i < GameManager.instance.hp)
            {
                hearts[i].sprite = heartSprite;
            }
            else if (i < GameManager.instance.maxHP)
            {
                hearts[i].sprite = emptyHeartSprite;
            }
            else
            {
                hearts[i].enabled = false;
            }
        }
    }

    private void DisplayMoney()
    {
        moneyText.text = "Money: $" + GameManager.instance.money;
    }

    private void DisplayKills()
    {
        killsText.text = "x " + GameManager.instance.kills;
    }

    public void DisplayLevel()
    {
        levelText.text = GameManager.instance.level + "F";
    }

    public void ToggleInventory()
    {
        if(inventoryPanel.activeSelf)
        {
            HideInventory();
        }
        else
        {
            ShowInventory();
        }
    }

    public void ShowInventory()
    {
        Debug.Log("Should be showing inventory");
        inventoryPanel.SetActive(true);
        inventory.isActive = true;
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                itemSlots[i].image.sprite = inventory.items[i].itemIcon;
                itemSlots[i].image.enabled = true;
            }
            else
            {
                itemSlots[i].image.enabled = false;
            }
        }
    }

    public void HideInventory()
    {
        Debug.Log("Now I'm hiding the inventory");
        inventoryPanel.SetActive(false);
        inventory.isActive = false;
    }

    public void DisplayShopInventory(List<Item> items, ShopKeeper shopKeeper)
    {
        shopKeeperInventory.Clear();

        shopKeeperPanel.SetActive(true);

        for (int i = 0; i < items.Count; i++)
        {
            shopKeeperInventory.Add(items[i]);
        }

        for (int i = 0; i < shopItemSlots.Length; i++)
        {
            shopItemSlots[i].gameObject.SetActive(false);
            if(i < shopKeeperInventory.Count)
            {
                shopItemSlots[i].gameObject.SetActive(true);
                shopItemSlots[i].SetItem(shopKeeperInventory[i], shopKeeper);
            }
        }

        EventSystem.current.SetSelectedGameObject(shopItemSlots[0].gameObject);

        GameManager.instance.EnterMenuState();
    }

    public void HideShopInventory()
    {
        shopKeeperPanel.SetActive(false);
        GameManager.instance.ExitMenuState();
    }

    public void DisplaySellableInventory()
    {

    }

    public void HideSellableInventory()
    {

    }
}
