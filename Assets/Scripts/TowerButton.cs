using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerButton : MonoBehaviour {

    public Button button;
    public TowerItem tower;
    public Image towerImage;
    public Text towerName;
    public Text price;
    public bool independent;

    private void Start()
    {
        //button = GetComponent<Button>();
        Setup();
    }

    public void Setup()
    {
        if(tower != null)
        {
            towerImage.sprite = tower.gunSprite;
            towerName.text = tower.name;
            price.text = tower.priceToBuy.ToString() + "$";
        }
    }

    private void OnMouseDown()
    {
        if (button.interactable && independent)
        {
            GameLogic.Instance.holding = true;
            GameLogic.Instance.towerHolder = tower;
            button.interactable = false;
            UIControl.Instance.TurretButtonsVisible(false);
        }        
    }

    public void CheckPrice()
    {
        if (GameLogic.Instance.points >= tower.priceToBuy)
            button.interactable = true;
        else
            button.interactable = false;
    }

}
