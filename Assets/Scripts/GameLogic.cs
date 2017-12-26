using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Move UI-related functionality to separate class, including spriteHolder
public class GameLogic : MonoBehaviour {

    public static GameLogic Instance { set; get; }

    public GameObject spriteHolder;
    public Transform spriteHolderRadius;
    public SpriteRenderer spriteHolderGun;
    public ObjectPool towerPool;
    public ObjectPool rocketPool;
    public ObjectPool shellPool;

    public BasicTile selectedTile;
    public TowerItem towerHolder;

    public bool holding;
    public bool readyToSpawn;
    public bool willPlace;

    public int points;
    public int health;

    public float damageMultiplier;

    private Vector3 prevMousePos;

    private void Start()
    {
        Instance = this;
        holding = false;
        readyToSpawn = false;
        willPlace = false;

        UIControl.Instance.UpdatePts();
    }

    private void Update()
    {

        //turret placement
        if (holding)
        {
            Vector3 worldPos = GetMousePosition();
            if (!readyToSpawn)
            {
                readyToSpawn = true;
                spriteHolder.transform.position = worldPos;
                spriteHolder.GetComponent<SpriteRenderer>().sprite = towerHolder.baseSprite;
                spriteHolderRadius.localScale = new Vector3(towerHolder.range, towerHolder.range, 1);
                spriteHolderGun.sprite = towerHolder.gunSprite;
                spriteHolder.SetActive(true);
            }
            else
            {
                spriteHolder.transform.position = worldPos;
            }
        }

        if(Input.GetMouseButtonUp(0) && holding)
        {
            holding = false;
            readyToSpawn = false;
            spriteHolder.SetActive(false);

            if (selectedTile != null)
            {
                selectedTile.rendererComponent.color = selectedTile.defaultColor;
                if (willPlace) {
                    selectedTile.PlaceTurret(towerHolder);
                    willPlace = false;
                    ChangePoints(-towerHolder.priceToBuy);
                }
                else if(selectedTile.empty)
                {
                    selectedTile.obstacleComponent.enabled = false;
                    selectedTile.walkable = true;
                }
            }

            UIControl.Instance.UpdatePts();
        }

    }

    public Vector3 GetMousePosition()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        worldPos.y = 3f;
        return worldPos;
    }

    public void LoseHealth(int dmg)
    {
        health -= dmg;
        UIControl.Instance.UpdateHP(health);
    }

    public void ChangePoints(int difference)
    {
        points += difference;
        UIControl.Instance.UpdatePts();
    }
}