using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AttackItemBase))]
public class AttackItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        
        base.OnInspectorGUI();
        EditorGUILayout.BeginVertical(GUI.skin.box);

        EditorGUILayout.LabelField("攻撃判定範囲");
        AttackItemBase attackItemBase = (AttackItemBase)target;
        Color EnableColor = Color.red;
        Color DisableColor = Color.white;
        serializedObject.Update();
        var AreaData = serializedObject.FindProperty("Area");
        List<Vector2Int> Area = new List<Vector2Int>();
        for (int i = 0; i < AreaData.arraySize; i++)
        {
            Area.Add(AreaData.GetArrayElementAtIndex(i).vector2IntValue);
        }
        Color ButtonColor = DisableColor;
        for (int i = 0; i < attackItemBase.BoardSize.x; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < attackItemBase.BoardSize.y; j++)
            {
                if (Area.Exists(element =>element == new Vector2Int(j, i) )){
                    ButtonColor = EnableColor;
                }
                else
                {
                    ButtonColor = DisableColor;
                }
                using (new BackgroundColorScope(ButtonColor))
                {
                    if (GUILayout.Button(""))
                    {
                        var idx = Area.IndexOf(new Vector2Int(j, i));
                        if (idx != -1)
                        {
                            AreaData.DeleteArrayElementAtIndex(idx);
                        }
                        else
                        {
                            AreaData.InsertArrayElementAtIndex(AreaData.arraySize);
                            AreaData.GetArrayElementAtIndex(AreaData.arraySize - 1).vector2IntValue = new Vector2Int(j, i);
                        }
                        serializedObject.ApplyModifiedProperties();
                    }
                }
                
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }
}
public class BackgroundColorScope : GUI.Scope
{
    private readonly Color color;
    public BackgroundColorScope(Color color)
    {
        this.color = GUI.backgroundColor;
        GUI.backgroundColor = color;
    }


    protected override void CloseScope()
    {
        GUI.backgroundColor = color;
    }
}

//AttackItemから派生するクラス
[CustomEditor(typeof(Fire))]
public class FireEditor : AttackItemEditor
{
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();
    }
}