using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnitAnimator))]
public class UnitAnimatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        UnitAnimator inspectorInstance = (UnitAnimator)target;

        if (GUILayout.Button("Show Down")) //DOWN
        {
            inspectorInstance.ChangeSpriteDirection(Direction2D.S);
        }
        if (GUILayout.Button("Show Right")) //RIGHT
        {
            inspectorInstance.ChangeSpriteDirection(Direction2D.E);
        }
        if (GUILayout.Button("Show Left")) //LEFT
        {
            inspectorInstance.ChangeSpriteDirection(Direction2D.W);
        }
        if (GUILayout.Button("Show Up")) //UP
        {
            inspectorInstance.ChangeSpriteDirection(Direction2D.N);
        }
    }
}
