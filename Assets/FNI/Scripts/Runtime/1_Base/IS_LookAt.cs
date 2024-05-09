using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FNI.IS;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 오브젝트가 대상을 바라보게 합니다.
/// </summary>
public class IS_LookAt : MonoBehaviour
{
    private float CorrectionSnapAngle
    {
        get { return snapAngle * 0.5f; }
    }

	[Header("바라볼 대상, 없으면 메인카메라")]
	public Transform lookTarget;
	[Header("바라보게 할 대상, 없으면 자기 자신")]
	public Transform target;

    [Header("Option")]
    [Tooltip("Fixed Axis")]
    public LookatOption option;
    public Vector3 addAngle;
    public float snapAngle;
    public bool reverse;

	private void Awake()
	{
		if (target == null)
			target = transform;
        if (lookTarget == null)
            lookTarget = Camera.main.transform;
	}

	/// <summary>
	/// Lookat은 LateUpdate에 적용해야 화면이 밀리지 않고 적용됨
	/// </summary>
	void LateUpdate()
	{
        if (lookTarget)
        {
            Vector3 dir = Vector3.zero;
            if (option.FlagChecker(LookatOption.Right))
                dir += new Vector3(1f, 0f, 0f);
            if (option.FlagChecker(LookatOption.Up))
                dir += new Vector3(0f, 1f, 0f);
            if (option.FlagChecker(LookatOption.Forward))
                dir += new Vector3(0f, 0f, 1f);

            target.LookAt(lookTarget, dir);

            float[] rot = new float[] { 0, 0, 0};
            if (reverse)
            {
                if (option.FlagChecker(LookatOption.Right))
                    rot[0] = CorrectAngle(-target.localEulerAngles.x);
                if (option.FlagChecker(LookatOption.Up))
                    rot[1] = CorrectAngle(-target.localEulerAngles.y);
                if (option.FlagChecker(LookatOption.Forward))
                    rot[2] = CorrectAngle(-target.localEulerAngles.z);
            }
            else
            {
                if (option.FlagChecker(LookatOption.Right))
                    rot[0] = CorrectAngle(target.localEulerAngles.x);
                if (option.FlagChecker(LookatOption.Up))
                    rot[1] = CorrectAngle(target.localEulerAngles.y);
                if (option.FlagChecker(LookatOption.Forward))
                    rot[2] = CorrectAngle(target.localEulerAngles.z);
            }

            target.localEulerAngles = new Vector3(rot[0], rot[1], rot[2]) + addAngle;
        }
	}
    public bool SelectChecker(LookatOption option)
    {
        return (this.option & option) == option;
    }
    private float CorrectAngle(float angle)
    {
        float snap = (angle - (angle % snapAngle));
        angle = (snapAngle != 0 ? snap : angle) + CorrectionSnapAngle;

        return angle;
    }
}
#if UNITY_EDITOR
[CanEditMultipleObjects]
[CustomEditor(typeof(IS_LookAt))]
public class IS_LookAtEditor : Editor
{
    private IS_LookAt Target
    {
        get
        {
            if (m_target == null)
                m_target = base.target as IS_LookAt;

            return m_target;
        }
    }
    private IS_LookAt m_target;

    int a = 0;
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        EditorGUI.BeginChangeCheck();

        //EditorGUILayout.Space();

        EditorGUILayout.LabelField("Target Setting", EditorStyles.boldLabel);
        Target.lookTarget = (Transform)EditorGUILayout.ObjectField("바라볼 대상" + (Target.lookTarget == null ? " (메인카메라)" : ""), 
                                                                   Target.lookTarget, typeof(Transform), true);
        Target.target = (Transform)EditorGUILayout.ObjectField("회전할 대상" + (Target.target == null ? " (대상 = 자신)" : ""),
                                                               Target.target, typeof(Transform), true);

        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
        float width = (EditorGUIUtility.currentViewWidth / 2) - 30;

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("바라보는 주체", GUILayout.Width(width));
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextField(Target.target == null ? "(대상 = 자신)" : Target.target.name, GUILayout.Width(width));
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("▶", GUILayout.Width(20), GUILayout.Height(EditorGUIUtility.singleLineHeight * 2));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("바라 볼 대상", GUILayout.Width(width));
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextField(Target.lookTarget == null ? "(메인카메라)" : Target.lookTarget.name, GUILayout.Width(width));
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Option", EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        Target.option = (LookatOption)EditorGUILayout.EnumFlagsField("Use Axis", Target.option);
        Target.reverse = EditorGUILayout.Toggle("Reverse", Target.reverse);
        EditorGUILayout.BeginHorizontal();
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.LabelField("", GUILayout.Width(EditorGUIUtility.labelWidth));
        GUILayout.Toggle(Target.option.FlagChecker(LookatOption.Right), "Right", EditorStyles.miniButtonLeft);
        GUILayout.Toggle(Target.option.FlagChecker(LookatOption.Up), "Up", EditorStyles.miniButtonMid);
        GUILayout.Toggle(Target.option.FlagChecker(LookatOption.Forward), "Forward", EditorStyles.miniButtonRight);
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        Target.addAngle = EditorGUILayout.Vector3Field("Add Angle", Target.addAngle);
        Target.snapAngle = EditorGUILayout.FloatField("Snap Angle", Target.snapAngle);

        //여기까지 검사해서 필드에 변화가 있으면
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(targets, "Changed Update Mode");
            //변경이 있을 시 적용된다. 이 코드가 없으면 인스펙터 창에서 변화는 있지만 적용은 되지 않는다.
            EditorUtility.SetDirty(Target);
        }
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
    }
}
#endif

public enum LookatOption
{
    Fixed = 0x0,
	Up = 0x1,
    Right = 0x2,
    Forward = 0x4,
}
