using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
 
[CustomEditor(typeof(EnhanceCharacterJoint))]
public class EnhanceCharacterJointInspector : Editor
{
    #region Properties
    private EnhanceCharacterJoint targetScript = null;
    private static Color twistColor = new Color(255f / 255f, 187f / 255f, 0, 0.5f);
    private static Color swing1Color = new Color(31f / 255f, 82f / 255f, 0, 0.5f);
    private static Color swing2Color = new Color(0, 34f / 255f, 82f / 255f, 0.5f);
 
    #endregion
 
    #region Editor
    public override void OnInspectorGUI()
    {
        targetScript = (EnhanceCharacterJoint)target;
        twistColor = EditorGUILayout.ColorField("Twist Color", twistColor);
        swing1Color = EditorGUILayout.ColorField("Swing 1 Color", swing1Color);
        swing2Color = EditorGUILayout.ColorField("Swing 2 Color", swing2Color);
        EditorUtility.SetDirty(targetScript);
        Repaint();
        
    }

     public void OnSceneGUI()
    {
        targetScript = (EnhanceCharacterJoint)target;

        Color blackColor = Color.black;
  
        CharacterJoint joint = targetScript.GetComponent<CharacterJoint>();
        Vector3 jointCenter = joint.transform.TransformPoint(joint.anchor);
        Vector3 twistAxis = joint.transform.TransformDirection(joint.axis).normalized;
        Vector3 forwardAxis = joint.transform.TransformDirection(Vector3.forward);
 
		//  90 to the twist axis
        Vector3 twist90 = Vector3.Cross(twistAxis, forwardAxis);
        //  high limit
        Handles.color = twistColor;
        Handles.DrawSolidArc(jointCenter, twistAxis, twist90, -joint.highTwistLimit.limit, 1);
        //  Low limit
        blackColor.a = twistColor.a;
        Handles.color = Color.Lerp(blackColor, twistColor, 0.85f);
        Handles.DrawSolidArc(jointCenter, twistAxis, twist90, -joint.lowTwistLimit.limit, 1);
        //  Swing axis
        Vector3 swingAxis = joint.transform.TransformDirection(joint.swingAxis).normalized;
        Handles.color = swing1Color;
        Handles.DrawSolidArc(jointCenter, swingAxis, twistAxis, -joint.swing1Limit.limit, 1);
        Handles.DrawSolidArc(jointCenter, swingAxis, twistAxis, joint.swing1Limit.limit, 1);
        //  90 to the Swing axis and twist
        Vector3 swingOrtho = Vector3.Cross(swingAxis, twistAxis);
        Handles.color = swing2Color;
        Handles.DrawSolidArc(jointCenter, swingOrtho, twistAxis, -joint.swing2Limit.limit, 1);
        Handles.DrawSolidArc(jointCenter, swingOrtho, twistAxis, joint.swing2Limit.limit, 1);
    }
    #endregion
}