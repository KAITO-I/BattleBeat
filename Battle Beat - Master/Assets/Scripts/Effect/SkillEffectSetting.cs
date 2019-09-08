using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[RequireComponent(typeof(AttackItemBase))]
public class SkillEffectSetting : MonoBehaviour
{
    [SerializeField]
    public Vector2Int Pos;
    [SerializeField]
    public Vector3 offSet;
    [SerializeField]
    public float scale;
    [SerializeField]
    public float speed;

    [SerializeField]
    public BaseEffect.Effect effect;
}
#if UNITY_EDITOR
[CustomEditor(typeof(SkillEffectSetting))]
public class SkillEffectSettingEditor : Editor
{
    public override void OnInspectorGUI()
    {

        base.OnInspectorGUI();
        EditorGUILayout.BeginVertical(GUI.skin.box);
        SkillEffectSetting skillEffectSetting = (SkillEffectSetting)target;
        EditorGUILayout.LabelField("エフェクト生成位置");
        Color EnableColor = Color.red;
        Color DisableColor = Color.white;
        serializedObject.Update();
        AttackItemBase attackItemBase = skillEffectSetting.gameObject.GetComponent<AttackItemBase>();
        Color ButtonColor = DisableColor;
        for (int i = 0; i < attackItemBase.BoardSize.x; i++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < attackItemBase.BoardSize.y; j++)
            {
                if (skillEffectSetting.Pos == new Vector2Int(j, i))
                {
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
                        skillEffectSetting.Pos = new Vector2Int(j, i);
                        serializedObject.ApplyModifiedProperties();
                    }
                }

            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }
}
#endif