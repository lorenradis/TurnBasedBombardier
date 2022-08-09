using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeShopKeeper : Interactable
{
    [SerializeField]
    private string startShopDialog;
    [SerializeField]
    private string successSellDialog;
    [SerializeField]
    private string cancelSellDialog;

    private List<Upgrade> upgrades = new List<Upgrade>();

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

    private void Start()
    {
        Upgrade Compass = new Upgrade("Compass", 250, () => { });
        Compass.icon = compassSprite;
        Upgrade Map = new Upgrade("Map", 100, () => { });
        Map.icon = mapSprite;
        Upgrade bigBombs = new Upgrade("Big Bombs", 30, () => { });
        bigBombs.icon = bigBombSprite;
        Upgrade biggerBombs = new Upgrade("Bigger Bombs", 300, () => { });
        biggerBombs.icon = biggerBombSprite;
        Upgrade biggestBombs = new Upgrade("Biggest bombs", 3000, () => { });
        biggestBombs.icon = biggestBombSprite;
        Upgrade bigBombBag;
        Upgrade biggerBombBag;
        Upgrade biggestBombBag;
        Upgrade bigSatchel;
        Upgrade biggerSatchel;
        Upgrade biggestSatchel;
        Upgrade fastShoes;
        Upgrade fasterShoes;
        Upgrade fastestShoes;

        upgrades.Add(Compass);
        upgrades.Add(Map);
        upgrades.Add(bigBombs);
        upgrades.Add(biggerBombs);
        upgrades.Add(biggestBombs);
    }

    public override bool OnInteract()
    {
        StartShop();
        return base.OnInteract();
    }

    public void StartShop()
    {
        DialogManager.instance.ShowQuestion(startShopDialog, () => {
            ShowUpgrades();
        }, () => {
            DialogManager.instance.ShowDialog(cancelSellDialog);
        });
    }

    public void ShowUpgrades()
    {
        GameManager.instance.uiManager.ShowUpgrades(upgrades, this);
    }

    private void EndShopping()
    {
        GameManager.instance.uiManager.HideUpgrades();
    }
}