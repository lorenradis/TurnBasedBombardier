using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UpgradeManager 
{

    public static UpgradeManager instance = null;

    [SerializeField]
    private string startShopDialog;
    [SerializeField]
    private string confirmPurchaseDialog;
    [SerializeField]
    private string notEnoughMoneyDialog;
    [SerializeField]
    private string anythingElseDialog;
    [SerializeField]
    private string successSellDialog;
    [SerializeField]
    private string cancelSellDialog;
    [SerializeField]
    private string goodbyeDialog;

    [SerializeField]
    private Sprite compassSprite;
    [SerializeField]
    private Sprite mapSprite;
    [SerializeField]
    private Sprite bigBombSprite;
    [SerializeField]
    private Sprite biggerBombSprite;
    [SerializeField]
    private Sprite biggestBombSprite;
    [SerializeField]
    private Sprite bigBombBagSprite;
    [SerializeField]
    private Sprite biggerBombBagSprite;
    [SerializeField]
    private Sprite biggestBombBagSprite;

    public List<Upgrade> upgrades = new List<Upgrade>();

    public UpgradeManager()
    {

    }

    public void SetupUpgrades()
    {
        Upgrade Compass = new Upgrade("Compass", 250, () => { GameManager.instance.hasCompass = true;});
        Compass.icon = compassSprite;
        Upgrade Map = new Upgrade("Map", 100, () => { GameManager.instance.hasMap = true; });
        Map.icon = mapSprite;
        Upgrade bigBombs = new Upgrade("Big Bombs", 30, () => { GameManager.instance.IncreaseBombRange(); });
        bigBombs.icon = bigBombSprite;
        Upgrade biggerBombs = new Upgrade("Bigger Bombs", 300, () => { GameManager.instance.IncreaseBombRange(); });
        biggerBombs.icon = biggerBombSprite;
        Upgrade biggestBombs = new Upgrade("Biggest bombs", 3000, () => { GameManager.instance.IncreaseBombRange(); });
        biggestBombs.icon = biggestBombSprite;
        Upgrade bigBombBag = new Upgrade("Big Bomb Bag", 150, () => { GameManager.instance.maxBombs++; });
        Upgrade biggerBombBagnew  = new Upgrade("Bigger Bomb Bag", 1500, () => { GameManager.instance.maxBombs++; }); ;
        Upgrade biggestBombBagnew  = new Upgrade("Biggest Bomb Bag", 4500, () => { GameManager.instance.maxBombs++; }); ;
        Upgrade bigSatchel = new Upgrade("Big Satchel", 2000, () => { GameManager.instance.inventory.maxItems += 2; });
        Upgrade biggerSatchel = new Upgrade("Bigger Satchel", 5000, () => { GameManager.instance.inventory.maxItems += 2; }); ;
        Upgrade biggestSatchel = new Upgrade("Biggest Satchel", 10000, () => { GameManager.instance.inventory.maxItems += 2; }); ;
        Upgrade fastShoes;
        Upgrade fasterShoes;
        Upgrade fastestShoes;

        upgrades.Add(Compass);
        upgrades.Add(Map);
        upgrades.Add(bigBombs);
        upgrades.Add(biggerBombs);
        upgrades.Add(biggestBombs);
    }

    public void BuyUpgrade(Upgrade upgrade)
    {

        if(GameManager.instance.money >= upgrade.upgradeCost)
        {
            ConfirmPurchase(upgrade);
        }
        else
        {
            NotEnoughMoney();
        }
    }

    public void StartUpgradeShop()
    {
        DialogManager.instance.ShowQuestion(startShopDialog, "Never mind", "Forget it", () => {

        }, () => {

        });
        GameManager.instance.uiManager.ShowUpgrades();
    }

    public void ConfirmPurchase(Upgrade upgrade)
    {
        DialogManager.instance.ShowQuestion(confirmPurchaseDialog, () => {
            upgrade.onUpgradeAction();
            upgrade.hasPurchased = true;
            GameManager.instance.SpendMoney(upgrade.upgradeCost);
            AnythingElse();
            Debug.Log("You bought a " + upgrade.upgradeName);
        }, () => {
            CanceledPurchase();
        });
    }

    public void NotEnoughMoney()
    {
        DialogManager.instance.ShowDialog(notEnoughMoneyDialog);
        AnythingElse();
    }

    public void SuccessPurchase()
    {
        DialogManager.instance.ShowDialog(successSellDialog);
        AnythingElse();
    }

    public void CanceledPurchase()
    {
        DialogManager.instance.ShowDialog(cancelSellDialog);
        AnythingElse();
    }

    public void AnythingElse()
    {
        DialogManager.instance.ShowQuestion(anythingElseDialog, () => {
            StartUpgradeShop();
        }, () => {
            Goodbye();
        });
    }

    public void Goodbye()
    {
        DialogManager.instance.ShowDialog(goodbyeDialog);
        GameManager.instance.uiManager.HideUpgrades();
    }
}
