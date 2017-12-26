using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class BasicTile : MonoBehaviour {

    public Color readyToPlace;
    public Color invalidTile;
    public Color defaultColor;

    public bool empty = true;
    private bool noEnemies = true;
    private bool pathFound = true;
    public bool walkable = true;
    public bool placeable = true;

    public List<Collider> triggerList;

    public NavMeshObstacle[] corners;

    public NavMeshObstacle obstacleComponent;
    public SpriteRenderer rendererComponent;

    public Tower towerHolder;

    public Text gCost;
    public Text hCost;
    public Text fCost;
    public Transform arrow;

    public Vector3 next;
    public BasicTile nextTile;

    public List<Enemy> headingHere;

    private void Awake()
    {
        triggerList = new List<Collider>();
        obstacleComponent = GetComponent<NavMeshObstacle>();
        rendererComponent = GetComponent<SpriteRenderer>();
        headingHere = new List<Enemy>();
    }

    private void OnMouseUpAsButton()
    {
        if (!empty && !UIControl.Instance.towerOptions.activeSelf && placeable)
            UIControl.Instance.OpenTowerOptions(this);
    }

    private void OnMouseUp()
    {
        rendererComponent.color = defaultColor;
    }

    private void OnMouseEnter()
    {
        if (GameLogic.Instance.readyToSpawn && empty && placeable)
        {
            walkable = false;
            pathFound = MyGrid.Instance.FindPath();
        } else if(GameLogic.Instance.readyToSpawn && !placeable)
            rendererComponent.color = invalidTile;
    }

    private void OnMouseOver()
    {

        if (GameLogic.Instance.readyToSpawn && empty && noEnemies && pathFound && placeable)
        {
            rendererComponent.color = readyToPlace;
            GameLogic.Instance.selectedTile = this;
            GameLogic.Instance.willPlace = true;
        }
        else if (GameLogic.Instance.readyToSpawn && placeable)
        {
            rendererComponent.color = invalidTile;
            GameLogic.Instance.willPlace = false;
            GameLogic.Instance.selectedTile = this;
        }
    }

    private void OnMouseExit()
    {
        if (GameLogic.Instance.readyToSpawn && placeable)
        {
            rendererComponent.color = defaultColor;
            GameLogic.Instance.willPlace = false;

            if (empty)
            {
                walkable = true;
            }                
        } else if (GameLogic.Instance.readyToSpawn && !placeable)
            rendererComponent.color = defaultColor;
    }

    public void PlaceTurret(TowerItem item)
    {
        GameObject newTower = GameLogic.Instance.towerPool.GetObject(transform, true);
        towerHolder = newTower.GetComponent<Tower>();
        towerHolder.Setup(item);
        rendererComponent.color = defaultColor;
        obstacleComponent.enabled = true;
        empty = false;
        newTower.transform.localPosition = new Vector3(2, -2, 0);
        walkable = false;

        foreach(Enemy e in headingHere)
            e.StopMoving();

        MyGrid.Instance.DoHeatMap();

        foreach (Enemy e in headingHere)
            e.StartMoving(Vector3.zero);

        headingHere.Clear();
    }

    public void RemoveTurret()
    {
        towerHolder.RadiusVisible(false);
        towerHolder.Sell();
        GameLogic.Instance.towerPool.ReturnObject(towerHolder.gameObject);
        obstacleComponent.enabled = false;
        empty = true;
        walkable = true;
        towerHolder = null;

        MyGrid.Instance.DoHeatMap();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggerList.Contains(other) && other.tag == "Enemy")
        {
            if (other.GetComponent<Enemy>().airborne)
                return;

            triggerList.Add(other);
            noEnemies = false;
            other.GetComponent<Enemy>().onTiles.Add(this);
        }
    }

    public void CheckCollider(Collider toCheck)
    {
        if (triggerList.Contains(toCheck))
        {
            triggerList.Remove(toCheck);
            noEnemies = triggerList.Count == 0;
        }
    }
}
