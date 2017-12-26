using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class TowerItem
{
    public string name;
    public Sprite baseSprite;
    public Sprite gunSprite;
    public float range;
    public float damage;
    public float fireDelay;
    public float fireDuration; // flamethrower only
    public float speedMultiplier;
    public int priceToBuy;
    public int priceToSell;
    public short type; // 0 - turret; 1 - AA turret; 2 -flamethrower; 3 - rocket launcher (rl); 4 - AA rl; 5 - mortar; 6 - slow tower
    public short upgradeLevel; // 0: 0, 3, 6; 1: all; 2: all
}

public class UIControl : MonoBehaviour {

    public static UIControl Instance { set; get; }

    public Text pointsText;
    public Text healthText;

    public GameObject towerOptions;
    private BasicTile currentTile;

    public TowerButton[] upgradeButtons = new TowerButton[3];

    public List<TowerItem> upgradesList;

    public GameObject buttonsHolder;
    public TowerButton turretButton;
    public TowerButton slowTowerButton;
    public TowerButton rocketButton;

    public Text sellPrice;

    private void Start()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (towerOptions.activeSelf)
                CloseTowerOptions();
            else
                Application.Quit();
        }            
    }

    public void UpdatePts()
    {
        pointsText.text = "$$$: " + GameLogic.Instance.points.ToString();
        turretButton.CheckPrice();
        slowTowerButton.CheckPrice();
        rocketButton.CheckPrice();
        TurretButtonsVisible(true);

        if (towerOptions.activeSelf)
            CheckUpgradePrice();
    }

    public void UpdateHP(int hp)
    {
        healthText.text = "Health: " + hp.ToString() + "hp";
    }

    public void OpenTowerOptions(BasicTile tile)
    {
        towerOptions.SetActive(true);
        currentTile = tile;
        currentTile.towerHolder.RadiusVisible(true);
        sellPrice.text = "Sell for " + tile.towerHolder.currentItem.priceToSell.ToString() + "$";

        TurretButtonsVisible(false);

        SetupButtons(tile.towerHolder.currentItem);
    }

    public void CloseTowerOptions()
    {
        towerOptions.SetActive(false);
        currentTile.towerHolder.RadiusVisible(false);
        currentTile = null;

        TurretButtonsVisible(true);
    }

    public void Sell()
    {
        GameLogic.Instance.ChangePoints(currentTile.towerHolder.currentItem.priceToSell);
        currentTile.towerHolder.RadiusVisible(false);
        towerOptions.SetActive(false);
        currentTile.RemoveTurret();
        currentTile = null;

        TurretButtonsVisible(true);
    }

    private void SetupButtons(TowerItem item)
    {
        switch (item.upgradeLevel)
        {
            case 0:
                switch (item.type)
                {
                    case 0:
                        SetupUpgrades(upgradesList[0], upgradesList[1], upgradesList[2]);
                        break;
                    case 6:
                        SetupUpgrades(upgradesList[3], null, null);
                        break;
                    case 3:
                        SetupUpgrades(upgradesList[6], upgradesList[7], upgradesList[8]);
                        break;
                    default:
                        Debug.Log("Not yet implemented");
                        SetupUpgrades(null, null, null);
                        break;
                }
                break;
            case 1:
                switch (item.type)
                {
                    case 6:
                        SetupUpgrades(upgradesList[4], null, null);
                        break;
                    case 0:
                        SetupUpgrades(upgradesList[5], null, null);
                        break;
                    case 3:
                        SetupUpgrades(upgradesList[9], null, null);
                        break;
                    case 4:
                        SetupUpgrades(upgradesList[10], null, null);
                        break;
                    case 1:
                        SetupUpgrades(upgradesList[11], null, null);
                        break;
                    case 2:
                        SetupUpgrades(upgradesList[12], null, null);
                        break;
                    case 5:
                        SetupUpgrades(upgradesList[13], null, null);
                        break;
                    default:
                        Debug.Log("Not yet implemented");
                        SetupUpgrades(null, null, null);
                        break;
                }
                break;
            default:
                SetupUpgrades(null, null, null);
                break;
        }

        CheckUpgradePrice();
    }

    public void Upgrade(TowerButton button)
    {
        TowerItem chosenUpgrade = button.tower;

        GameLogic.Instance.ChangePoints(-chosenUpgrade.priceToBuy);
        currentTile.towerHolder.Setup(chosenUpgrade);
        CloseTowerOptions();
    }

    private void SetupUpgrades(TowerItem first, TowerItem second, TowerItem third)
    {
        TowerItem[] tempUpArray = { first, second, third };

        for(int i = 0; i < 3; i++)
        {
            TowerButton currButton = upgradeButtons[i];
            if (tempUpArray[i] == null)
            {
                currButton.gameObject.SetActive(false);
            } else
            {
                currButton.gameObject.SetActive(true);
                currButton.tower = tempUpArray[i];
                currButton.Setup();
            }
        }
    }

    private void CheckUpgradePrice()
    {
        foreach(TowerButton tButton in upgradeButtons)
        {
            if (tButton.gameObject.activeSelf)
            {
                tButton.CheckPrice();
            }
        }
    }

    public void TurretButtonsVisible(bool visible)
    {
        buttonsHolder.GetComponent<Animator>().SetBool("Visible", visible);
    }
}
