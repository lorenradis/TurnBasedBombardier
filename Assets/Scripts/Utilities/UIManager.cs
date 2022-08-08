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

    private bool isSelling = false;
    private bool isBuying = false;

    private List<Item> itemsToSell = new List<Item>();
    private List<Item> itemsToBuy = new List<Item>();


    [SerializeField]
    private RectTransform exitButton;

    [SerializeField]
    private GameObject shopKeeperPanel;
    [SerializeField]
    private ItemSlot[] shopItemSlots;
    private ShopKeeper shopKeeper;
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
        if (GameManager.instance.gameState != GameManager.GameState.NORMAL)
            return;
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
        if(isSelling || isBuying)
        {
            exitButton.gameObject.SetActive(false);
        }
        else
        {
            exitButton.gameObject.SetActive(true);
        }
        GameManager.instance.EnterMenuState();
        inventoryPanel.SetActive(true);
        inventory.isActive = true;
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < inventory.items.Count)
            {
                itemSlots[i].SetItem(GameManager.instance.inventory.items[i]);
            }
            else
            {
                itemSlots[i].image.enabled = false;
            }
        }
    }

    public void HideInventory()
    {
            GameManager.instance.ExitMenuState();
            Debug.Log("Now I'm hiding the inventory");
            inventoryPanel.SetActive(false);
            inventory.isActive = false;
    }

    public void DisplayShopInventory(List<Item> items, ShopKeeper _shopKeeper)
    {
        isSelling = false;
        isBuying = true;

        itemsToBuy.Clear();

        shopKeeper = _shopKeeper;
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
        shopKeeper = null;
        GameManager.instance.ExitMenuState();
    }

    public void SelectItem(Item item)
    {
        int cash = 0;
        if (isSelling)
        {
            if (itemsToSell.Contains(item))
            {
                itemsToSell.Remove(item);
            }
            else
            {
               itemsToSell.Add(item);
            }

            cash = 0;

            for (int i = 0; i < itemsToSell.Count; i++)
            {
                cash += itemsToSell[i].sellPrice;    
            }


        }else if(isBuying)
        {
            if(itemsToBuy.Contains(item))
            {
                itemsToBuy.Remove(item);
            }
            else
            {
                itemsToBuy.Add(item);
            }

            cash = 0;

            for (int i = 0; i < itemsToBuy.Count; i++)
            {
                cash += itemsToBuy[i].sellPrice;
            }

        }
        else
        {
            ShowItemInfo(item);
        }
    }

    public void ConfirmSell()
    {
        if(itemsToSell.Count < 1)
        {
            DialogManager.instance.ShowQuestion("You gotta tell me what you wanna sell, bud.", "Confirm", "Cancel", () => { ConfirmSell(); }, () => { CancelSell(); }); 
            return;
        }

        isSelling = false;
        isBuying = false;

        int cash = 0;
        for (int i = 0; i < itemsToSell.Count; i++)
        {
            cash += itemsToSell[i].sellPrice;
            inventory.RemoveItemFromList(itemsToSell[i]);
        }

        GameManager.instance.GainMoney(cash);

        itemsToSell.Clear();

        HideInventory();

        shopKeeper.SuccessfulPlayerSale();
    }

    public void CancelSell()
    {
        isSelling = false;
        itemsToSell.Clear();
        DialogManager.instance.ShowQuestion("Aw, that's too bad.  Anything else I can do for ya?", "Yes", "No", () => { }, () => { });
    }

    public void ConfirmPurchase()
    {

    }

    public void CancelPurchase()
    {
        isBuying = false;
        itemsToBuy.Clear();
    }


    public void DisplaySellableInventory(ShopKeeper _shopKeeper)
    {
        shopKeeper = _shopKeeper;
        itemsToSell.Clear();
        ShowInventory();
        isSelling = true;
        isBuying = false;
    }

    public void ShowItemInfo(Item item)
    {

    }

    private void HideItemInfo()
    {

    }
}
