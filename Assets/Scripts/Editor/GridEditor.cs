using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MyGrid))]
public class GridEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MyGrid grid = (MyGrid)target;

        GUILayout.BeginHorizontal();

            if(GUILayout.Button("Generate Grid"))
            {
                grid.CreateGrid();
            }

            if (GUILayout.Button("Reset Grid"))
            {
                grid.ClearGrid();
            }

        GUILayout.EndHorizontal();
    }

}

[CustomEditor(typeof(Enemy))]
public class EnemyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Enemy enemy = (Enemy)target;

        if (GUILayout.Button("Teleport"))
        {
            enemy.Teleport();
        }

    }
}
