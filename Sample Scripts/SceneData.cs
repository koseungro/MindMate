/// 작성자: 고승로
/// 작성일: 2020-08-24
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력

using FNI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

using UnityEditor;

namespace FNI
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Scene Data", menuName = "FNI/Scene Data")]
    public class SceneData : ScriptableObject
    {
        public string sceneID;
        public List<CutData> cutDataList = new List<CutData>();
        public SceneData nextScene = null;

        public bool canRepeat = false;
        /// <summary>
        /// 해당 Scene Data에서 Camera View 이동 가능 여부
        /// </summary>
        public bool canCameraView = false;
        /// <summary>
        /// 해당 Scene Data에서 Skip 기능 사용 여부
        /// </summary>
        public bool canSkip = false;
    }

    [Serializable]
    public class CutData
    {
        public VisualType visualType;
        public bool isFold = true;
        public bool _isFold = true;
        public bool checkPoint_End = false;

        public NarrationOption narrationOption;
        public AnimationOption animationOption;
        public UIOption uiOption;
        public FadeOption fadeOption;
        public VideoOption videoOption;
        public LayOption layOption;
        public WaitOption waitOption;
        public ObjectOption objActiveOption;
        public SceneMgrOption sceneOption;
        public TimerOption timerOption;
        public XROption xrOption;
    }


    [Serializable]
    public class UIOption
    {
        public UI_FuncType uiType = UI_FuncType.None;
        public string objPath;
        public bool isActive;
        public SceneData nextScene = null;
        public UIAnimationOption uiAnimationOption = null;
        public Vector3 pos_UI;
        public Vector3 rot_UI;
        public int breatheCnt;
        public bool useHoldBreath;
        public bool isSame;
    }

    [Serializable]
    public class AnimationOption
    {
        public Anim_FuncType animType;
        public CharType charType;
        public AnimationClip clip;
        public bool isSame;
    }

    [Serializable]
    public class NarrationOption
    {
        public Sound_FuncType soundType;
        public NarrationType narrationType;
        public AudioSubtitleType subtitleType;
        public AudioClip clip;
        public bool isSameTime;
        public bool loop;
        public float volume = 1f;
        public float volumeFadeTime;
        public bool notRestoreBackground;
        public float waitSecond = 1;
        public bool useCheckPoint;
        public List<SubtitleCheckPoint> checkSubtitleList = new List<SubtitleCheckPoint>();
    }

    [Serializable]
    public class SubtitleCheckPoint
    {
        // Check 자막 번호
        public int checkSubtitleNum;
        public bool isChecked;
        public List<CutData> checkEventList = new List<CutData>();
    }

    [Serializable]
    public class FadeOption
    {
        public UICanvasType uiCanvasType;
        public float startAlpha;
        public float endAlpha;
        public float time;
        public bool useLoadingUI;
    }

    [Serializable]
    public class UIAnimationOption
    {
        public UIAnimationType aniType;
        public float time;
        public Vector3 startPos;
        public Vector3 endPos;
        public float startA;
        public float endA;
        public Sprite changeSprite;
        public string changeText;
    }


    [Serializable]
    /// <summary>
    /// 비디오 명령을 셋팅하고 집어 넣으면 됩니다.
    /// </summary>
    public class VideoOption
    {
        public VideoType videoType;
        public VideoSubtitleType subtitleType;
        public VideoState state;
        public SeekState seekState;
        public string urlPath;
        public double sTime;
        public double eTime;
        public double jumpLenth;
        public Vector3 rotation;
        public Vector3 position;
        public long seekFrame;
        public bool useSubtitle;
    }

    [Serializable]
    public class LayOption
    {
        public bool isOn;
    }

    [Serializable]
    public class WaitOption
    {
        public float waitTime;
    }

    [Serializable]
    public class ObjectOption
    {
        public Obj_FuncType ObjType = Obj_FuncType.None;
        public string rootParent;
        public string targetObjPath;
        public bool isActive;
        public Material skyMat;
        public Vector3 pos_Obj;
        public Vector3 rot_Obj;
        public float moveTime;
        public AnimationCurve curveMove;
        public bool moveOn;
        public float bezierPos;
        public int layerNum;
        public string componentName;
        public string targetParentPath;
        public string targetParentRoot;
        public CameraViewType cameraViewType;
    }

    [Serializable]
    public class SceneMgrOption
    {
        public SceneMgrType sceneMgrType;
        public string sceneName;
        public bool fogOn;
        public Color fogColor;
        public FogMode fogMode;
        public float startDistance;
        public float endDistance;
        public float density;
        public float haloStrength = 0.5f;
        public float flareStrength = 1f;
        public string lightPath;
        public Light light;
        public CameraClearFlags clearFlags;
        public Color backgroundColor;
        public VideoSubtitleType videoContentType;
        public SceneData downloadCompleted_Scene;
        public SceneData downloadUnCompleted_Scene;
#if UNITY_EDITOR
        public LightingDataAsset lightingDataAsset;
#endif
    }

    [Serializable]
    public class TimerOption
    {
        public TimerType timerType;
        public float endTime;
        public float limitTime;
        public string textPath;
        public string buttonPath;
    }

    [Serializable]
    public class XROption
    {
        public XR_FuncType xrType;
        public bool useFunction;
        public float hapticAmplitude;
        public float hapticDuration;
        public Color outlineColor;
    }


#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SceneData))]
    public class SceneDataEditor : Editor
    {
        private SceneData _sceneData;
        private SceneData SceneData
        {
            get
            {
                if (_sceneData == null)
                    _sceneData = base.target as SceneData;

                return _sceneData;
            }
        }

        /// <summary>
        /// 씬 오브젝트의 주소를 받아오기 위한 임시 변수
        /// </summary>
        Transform objTransform;
        /// <summary>
        /// 찾고자 하는 오브젝트의 최상위 부모젝트 (string 추출용)
        /// </summary>
        Transform rootParent;

        Transform _objTransform;
        Transform _rootParent;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.LabelField("<컷 데이터 목록>", EditorStyles.boldLabel, GUILayout.MaxWidth(100));

            SceneData.sceneID = EditorGUILayout.TextField("Scene ID", SceneData.sceneID);

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("반복 가능 여부", GUILayout.MaxWidth(180));
                SceneData.canRepeat = EditorGUILayout.Toggle(SceneData.canRepeat);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("카메라 시점 이동 가능 여부", GUILayout.MaxWidth(180));
                SceneData.canCameraView = EditorGUILayout.Toggle(SceneData.canCameraView);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Skip 기능 사용 가능 여부", GUILayout.MaxWidth(180));
                SceneData.canSkip = EditorGUILayout.Toggle(SceneData.canSkip);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);
                {
                    SceneData.cutDataList = ListController(SceneData.cutDataList);

                    if (SceneData.cutDataList.Count == 0)
                        EditorGUILayout.Space();

                    for (int cnt = 0; cnt < SceneData.cutDataList.Count; cnt++)
                    {
                        if (SceneData.cutDataList.Count <= cnt) return;

                        CutData cutData = SceneData.cutDataList[cnt];

                        if (cutData == null) return;
                        if (cnt == 0)
                            EditorGUILayout.Space();

                        if (cutData.visualType == VisualType.Line)
                            GUI.color = Color.red;
                        else if (cnt % 2 == 0)
                            GUI.color = Color.white;
                        else
                            GUI.color = Color.cyan;

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(10));
                                {
                                    cutData.isFold = EditorGUILayout.Toggle(cutData.isFold, EditorStyles.foldout, GUILayout.MaxWidth(10));
                                    EditorGUILayout.LabelField("", GUILayout.MaxWidth(2));
                                }
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.MaxWidth(110));
                                {
                                    ListController(SceneData.cutDataList, cnt, false);

                                    //EditorGUILayout.LabelField($"[{cnt}] Cut", GUILayout.MaxWidth(50));
                                    //Change_CutCnt(_sceneData.cutDataList, cnt);

                                }
                                EditorGUILayout.EndHorizontal();

                                cutData.visualType = (VisualType)EditorGUILayout.EnumPopup(cutData.visualType);
                            }
                            EditorGUILayout.EndHorizontal();

                            if (SceneData.cutDataList.Count <= cnt) return;
                            if (SceneData.cutDataList[cnt].isFold)
                            {
                                switch (cutData.visualType)
                                {
                                    case VisualType.Line:
                                        //GUI.color = Color.red;
                                        //EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                                        //EditorGUILayout.LabelField(" ");
                                        //EditorGUILayout.EndHorizontal();
                                        //GUI.color = Color.white;
                                        break;
                                    case VisualType.Narration:
                                        Narration(cutData);
                                        break;
                                    case VisualType.Wait:
                                        WaitTime(cutData);
                                        break;
                                    case VisualType.UI:
                                        UI(cutData, cnt);
                                        break;
                                    case VisualType.FadeInOut:
                                        FadeInOut(cutData);
                                        break;
                                    case VisualType.Animation:
                                        Animation(cutData);
                                        break;
                                    case VisualType.Video:
                                        Video(cutData);
                                        break;
                                    case VisualType.Lay:
                                        Lay(cutData);
                                        break;
                                    case VisualType.Object:
                                        Object(cutData);
                                        break;
                                    case VisualType.SceneMgr:
                                        SceneMgr(cutData);
                                        break;
                                    case VisualType.Timer:
                                        SetTimer(cutData);
                                        break;
                                    case VisualType.XRManager:
                                        XRManager(cutData);
                                        break;
                                    default:
                                        break;
                                }

                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUILayout.EndVertical();


                EditorGUILayout.LabelField("자동 다음 Sequence");
                EditorGUILayout.BeginVertical(EditorStyles.objectFieldThumb);
                {
                    SceneData.nextScene = (SceneData)EditorGUILayout.ObjectField(SceneData.nextScene, typeof(SceneData), false);
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                {
                    EditorGUILayout.LabelField("Wait 총 시간 : ", GUILayout.MaxWidth(80));
                    EditorGUILayout.LabelField(TotalWaitTime().ToString() + " 초");
                }
                EditorGUILayout.EndHorizontal();

            }

            //여기까지 검사해서 필드에 변화가 있으면
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObjects(targets, "Changed Update Mode");
                //변경이 있을 시 적용된다. 이 코드가 없으면 인스펙터 창에서 변화는 있지만 적용은 되지 않는다.
                EditorUtility.SetDirty(SceneData);
            }
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();
        }


        private void Object(CutData cData)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                //EditorGUILayout.BeginHorizontal();
                //{
                //    EditorGUILayout.LabelField("* VR3D_UI : UI의 배경 역할을 하는 오브젝트 (3D 영상을 재생하기 전 숨겨야 함)", GUILayout.MaxWidth(500));
                //}
                //EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("기능", GUILayout.MaxWidth(50));
                        cData.objActiveOption.ObjType = (Obj_FuncType)EditorGUILayout.EnumPopup(cData.objActiveOption.ObjType);
                    }
                    EditorGUILayout.EndHorizontal();

                    if (cData.objActiveOption.ObjType != Obj_FuncType.SkyMatChange &&
                        cData.objActiveOption.ObjType != Obj_FuncType.SetBezierPosition &&
                        cData.objActiveOption.ObjType != Obj_FuncType.ChangeCameraView)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("타겟 Object", GUILayout.MaxWidth(150));
                                objTransform = (Transform)EditorGUILayout.ObjectField(objTransform, typeof(Transform), true, GUILayout.MaxWidth(150));

                                if (objTransform != null)
                                {
                                    cData.objActiveOption.targetObjPath = ParentName(objTransform);
                                    cData.objActiveOption.rootParent = rootParent.name;
                                }
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("타겟의 최상위 부모 이름", GUILayout.MaxWidth(150));
                                cData.objActiveOption.rootParent = EditorGUILayout.TextField(cData.objActiveOption.rootParent);
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("타겟 경로", GUILayout.MaxWidth(150));
                                cData.objActiveOption.targetObjPath = EditorGUILayout.TextField(cData.objActiveOption.targetObjPath);
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();

                        objTransform = null;
                        rootParent = null;
                    }

                    //EditorGUILayout.Space();

                    switch (cData.objActiveOption.ObjType)
                    {
                        case Obj_FuncType.None:
                            break;
                        case Obj_FuncType.Active:
                            EditorGUILayout.BeginHorizontal();
                            {
                                string activeMent = "";
                                EditorGUILayout.LabelField("isActive", GUILayout.MaxWidth(60));
                                cData.objActiveOption.isActive = EditorGUILayout.Toggle(cData.objActiveOption.isActive, GUILayout.MaxWidth(30));
                                activeMent = cData.objActiveOption.isActive ? "On" : "Off";
                                EditorGUILayout.LabelField($"[{activeMent}]", GUILayout.MaxWidth(50));
                            }
                            EditorGUILayout.EndHorizontal();
                            break;
                        case Obj_FuncType.SkyMatChange:
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("Material", GUILayout.MaxWidth(50));
                                cData.objActiveOption.skyMat = EditorGUILayout.ObjectField(cData.objActiveOption.skyMat, typeof(Material), true, GUILayout.MaxWidth(150)) as Material;
                            }
                            EditorGUILayout.EndHorizontal();
                            break;
                        case Obj_FuncType.Transform:
                            cData.objActiveOption.pos_Obj = EditorGUILayout.Vector3Field("Position", cData.objActiveOption.pos_Obj);
                            cData.objActiveOption.rot_Obj = EditorGUILayout.Vector3Field("Rotation", cData.objActiveOption.rot_Obj);
                            break;
                        case Obj_FuncType.Transform_Move:
                            cData.objActiveOption.pos_Obj = EditorGUILayout.Vector3Field("End Position", cData.objActiveOption.pos_Obj);
                            //cData.objActiveOption.rot_Obj = EditorGUILayout.Vector3Field("Rotation", cData.objActiveOption.rot_Obj);
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("이동 시간", GUILayout.MaxWidth(220));
                                cData.objActiveOption.moveTime = EditorGUILayout.FloatField(cData.objActiveOption.moveTime, GUILayout.MaxWidth(50));
                            }
                            EditorGUILayout.EndHorizontal();
                            cData.objActiveOption.curveMove = EditorGUILayout.CurveField("Animation on Z", cData.objActiveOption.curveMove);
                            break;
                        case Obj_FuncType.MoveOn:
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("Move 활성화", GUILayout.MaxWidth(100));
                                cData.objActiveOption.moveOn = EditorGUILayout.Toggle(cData.objActiveOption.moveOn, GUILayout.MaxWidth(50));
                            }
                            EditorGUILayout.EndHorizontal();
                            break;
                        case Obj_FuncType.SetBezierPosition:
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("Bezier Position", GUILayout.MaxWidth(100));
                                cData.objActiveOption.bezierPos = EditorGUILayout.FloatField(cData.objActiveOption.bezierPos, GUILayout.MaxWidth(80));
                            }
                            EditorGUILayout.EndHorizontal();
                            break;
                        case Obj_FuncType.AllInactive:
                            EditorGUILayout.BeginHorizontal();
                            {
                                string activeMent = "";
                                EditorGUILayout.LabelField("isActive", GUILayout.MaxWidth(60));
                                cData.objActiveOption.isActive = EditorGUILayout.Toggle(cData.objActiveOption.isActive, GUILayout.MaxWidth(30));
                                activeMent = cData.objActiveOption.isActive ? "On" : "Off";
                                EditorGUILayout.LabelField($"[{activeMent}]", GUILayout.MaxWidth(50));
                            }
                            EditorGUILayout.EndHorizontal();
                            break;
                        case Obj_FuncType.ChangeLayer:
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("바꿀 Layer Number", GUILayout.MaxWidth(150));
                                cData.objActiveOption.layerNum = EditorGUILayout.IntField(cData.objActiveOption.layerNum, GUILayout.MaxWidth(80));
                            }
                            EditorGUILayout.EndHorizontal();
                            break;
                        case Obj_FuncType.EnabledComponent:
                            EditorGUILayout.BeginVertical();
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.LabelField("Target Component 이름", GUILayout.MaxWidth(150));
                                    cData.objActiveOption.componentName = EditorGUILayout.TextField(cData.objActiveOption.componentName, GUILayout.MaxWidth(150));
                                }
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                {
                                    string activeMent = "";
                                    EditorGUILayout.LabelField("isActive", GUILayout.MaxWidth(60));
                                    cData.objActiveOption.isActive = EditorGUILayout.Toggle(cData.objActiveOption.isActive, GUILayout.MaxWidth(30));
                                    activeMent = cData.objActiveOption.isActive ? "On" : "Off";
                                    EditorGUILayout.LabelField($"[{activeMent}]", GUILayout.MaxWidth(50));
                                }
                                EditorGUILayout.EndHorizontal();
                                GUI.color = Color.red;
                                EditorGUILayout.LabelField("※ Componenet는 [Behavior/ Collider] 상속 대상만 가능");
                                GUI.color = Color.white;
                            }
                            EditorGUILayout.EndVertical();
                            break;
                        case Obj_FuncType.SetParent:
                            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.LabelField("타겟 Parent", GUILayout.MaxWidth(150));
                                    objTransform = EditorGUILayout.ObjectField(objTransform, typeof(Transform), true, GUILayout.MaxWidth(150)) as Transform;

                                    if (objTransform != null)
                                    {
                                        cData.objActiveOption.targetParentPath = ParentName(objTransform);
                                        cData.objActiveOption.targetParentRoot = rootParent.name;
                                    }
                                }
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.LabelField("타겟 Parent의 최상위 부모", GUILayout.MaxWidth(150));
                                    cData.objActiveOption.targetParentRoot = EditorGUILayout.TextField(cData.objActiveOption.targetParentRoot);
                                }
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.LabelField("타겟 Parent 경로", GUILayout.MaxWidth(150));
                                    cData.objActiveOption.targetParentPath = EditorGUILayout.TextField(cData.objActiveOption.targetParentPath);
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            EditorGUILayout.EndVertical();

                            objTransform = null;
                            rootParent = null;
                            break;
                        case Obj_FuncType.ChangeCameraView:
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("카메라 시점", GUILayout.MaxWidth(80));
                                cData.objActiveOption.cameraViewType = (CameraViewType)EditorGUILayout.EnumPopup(cData.objActiveOption.cameraViewType);
                            }
                            EditorGUILayout.EndHorizontal();
                            break;
                    }

                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }

        private void SceneMgr(CutData cData)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("기능", GUILayout.MaxWidth(50));
                            cData.sceneOption.sceneMgrType = (SceneMgrType)EditorGUILayout.EnumPopup(cData.sceneOption.sceneMgrType);
                        }
                        EditorGUILayout.EndHorizontal();

                        //EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        //{
                        //    EditorGUILayout.BeginHorizontal();
                        //    {
                        //        EditorGUILayout.LabelField("타겟 Scene", GUILayout.MaxWidth(150));
                        //        objTransform = EditorGUILayout.ObjectField(objTransform, typeof(Transform), true, GUILayout.MaxWidth(150)) as Transform;

                        //        if (objTransform != null)
                        //        {
                        //            cData.objActiveOption.targetObjPath = ParentName(objTransform);
                        //            cData.objActiveOption.rootParent = rootParent.name;
                        //        }
                        //    }
                        //    EditorGUILayout.EndHorizontal();
                        //}
                        //EditorGUILayout.EndVertical();

                        switch (cData.sceneOption.sceneMgrType)
                        {
                            case SceneMgrType.Load:
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.LabelField("Scene Name", GUILayout.MaxWidth(150));
                                    cData.sceneOption.sceneName = EditorGUILayout.TextField(cData.sceneOption.sceneName);
                                }
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                                    {
                                        //EditorGUILayout.BeginHorizontal();
                                        //{
                                        //    EditorGUILayout.LabelField("Light Data Asset", GUILayout.MaxWidth(150));
                                        //    cData.sceneOption.lightingDataAsset = EditorGUILayout.ObjectField(cData.sceneOption.lightingDataAsset, typeof(LightingDataAsset), false, GUILayout.MaxWidth(150)) as LightingDataAsset;
                                        //}
                                        //EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.BeginHorizontal();
                                        {
                                            EditorGUILayout.LabelField("Light Object", GUILayout.MaxWidth(150));
                                            objTransform = EditorGUILayout.ObjectField(objTransform, typeof(Transform), true, GUILayout.MaxWidth(150)) as Transform;

                                            if (objTransform != null)
                                            {
                                                cData.sceneOption.lightPath = ParentName(objTransform);
                                            }
                                        }
                                        EditorGUILayout.EndHorizontal();

                                        EditorGUILayout.BeginHorizontal();
                                        {
                                            EditorGUILayout.LabelField("타겟 Light 이름(Find)", GUILayout.MaxWidth(150));
                                            cData.sceneOption.lightPath = EditorGUILayout.TextField(cData.sceneOption.lightPath);
                                        }
                                        EditorGUILayout.EndHorizontal();
                                    }
                                    EditorGUILayout.EndVertical();

                                    objTransform = null;
                                }
                                EditorGUILayout.EndHorizontal();
                                break;
                            case SceneMgrType.UnLoad:
                                EditorGUILayout.LabelField("Scene Name", GUILayout.MaxWidth(150));
                                cData.sceneOption.sceneName = EditorGUILayout.TextField(cData.sceneOption.sceneName);
                                break;
                            case SceneMgrType.FogSetting:
                                EditorGUILayout.BeginHorizontal();
                                {
                                    string activeMent = "";
                                    EditorGUILayout.LabelField("Fog On", GUILayout.MaxWidth(60));
                                    cData.sceneOption.fogOn = EditorGUILayout.Toggle(cData.sceneOption.fogOn, GUILayout.MaxWidth(30));
                                    activeMent = cData.sceneOption.fogOn ? "On" : "Off";
                                    EditorGUILayout.LabelField($"[{activeMent}]", GUILayout.MaxWidth(50));
                                }
                                EditorGUILayout.EndHorizontal();

                                if (cData.sceneOption.fogOn)
                                {
                                    cData.sceneOption.fogColor = EditorGUILayout.ColorField("Fog Color", cData.sceneOption.fogColor);
                                    cData.sceneOption.fogMode = (FogMode)EditorGUILayout.EnumPopup("Fog Mode", cData.sceneOption.fogMode);
                                    if (cData.sceneOption.fogMode == FogMode.Linear)
                                    {
                                        cData.sceneOption.startDistance = EditorGUILayout.FloatField("Start Distance", cData.sceneOption.startDistance);
                                        cData.sceneOption.endDistance = EditorGUILayout.FloatField("End Distance", cData.sceneOption.endDistance);
                                    }
                                    if (cData.sceneOption.fogMode == FogMode.Exponential)
                                        cData.sceneOption.density = EditorGUILayout.FloatField("Density", cData.sceneOption.density);

                                    cData.sceneOption.haloStrength = EditorGUILayout.Slider("Halo Strength", cData.sceneOption.haloStrength, 0, 1);
                                    cData.sceneOption.flareStrength = EditorGUILayout.Slider("Flare Strength", cData.sceneOption.flareStrength, 0, 1);
                                }
                                break;
                            case SceneMgrType.CameraSetting:
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.LabelField("Clear Flags", GUILayout.MaxWidth(100));
                                    cData.sceneOption.clearFlags = (CameraClearFlags)EditorGUILayout.EnumPopup(cData.sceneOption.clearFlags);
                                }
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.LabelField("Background", GUILayout.MaxWidth(100));
                                    cData.sceneOption.backgroundColor = EditorGUILayout.ColorField(cData.sceneOption.backgroundColor);
                                }
                                EditorGUILayout.EndHorizontal();

                                break;
                            case SceneMgrType.CheckVideoContent_Completed:
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.LabelField("진행 완료 체크할 콘텐츠", GUILayout.MaxWidth(150));
                                    cData.sceneOption.videoContentType = (VideoSubtitleType)EditorGUILayout.EnumPopup(cData.sceneOption.videoContentType);
                                }
                                EditorGUILayout.EndHorizontal();
                                break;
                            case SceneMgrType.CheckFileDownload:
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.LabelField("다운로드 완료시 진행 시퀀스", GUILayout.MaxWidth(200));
                                    cData.sceneOption.downloadCompleted_Scene = (SceneData)EditorGUILayout.ObjectField(cData.sceneOption.downloadCompleted_Scene, typeof(SceneData), false);
                                }
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.LabelField("다운로드 미완료시 진행 시퀀스", GUILayout.MaxWidth(200));
                                    cData.sceneOption.downloadUnCompleted_Scene = (SceneData)EditorGUILayout.ObjectField(cData.sceneOption.downloadUnCompleted_Scene, typeof(SceneData), false);
                                }
                                EditorGUILayout.EndHorizontal();
                                break;
                        }

                    }
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }

        private void SetTimer(CutData cData)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    if (cData.timerOption.timerType != TimerType.RecordTimer_Waiting)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("해당 Text 오브젝트", GUILayout.MaxWidth(150));
                                objTransform = EditorGUILayout.ObjectField(objTransform, typeof(Transform), true, GUILayout.MaxWidth(150)) as Transform;

                                if (objTransform != null)
                                {
                                    cData.timerOption.textPath = ParentName(objTransform);
                                }
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("UI 경로", GUILayout.MaxWidth(150));
                                cData.timerOption.textPath = EditorGUILayout.TextField(cData.timerOption.textPath);

                            }
                            EditorGUILayout.EndHorizontal();

                            // 임시 오브젝트 초기화
                            objTransform = null;

                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("시간", GUILayout.MaxWidth(150));
                                cData.timerOption.endTime = EditorGUILayout.FloatField(cData.timerOption.endTime, GUILayout.MaxWidth(50));
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();

                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("기능", GUILayout.MaxWidth(50));
                                cData.timerOption.timerType = (TimerType)EditorGUILayout.EnumPopup(cData.timerOption.timerType);
                            }
                            EditorGUILayout.EndHorizontal();

                            if (cData.timerOption.timerType == TimerType.Timer_Button)
                            {
                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.LabelField("해당 Button 오브젝트", GUILayout.MaxWidth(150));
                                    _objTransform = EditorGUILayout.ObjectField(_objTransform, typeof(Transform), true, GUILayout.MaxWidth(150)) as Transform;

                                    if (_objTransform != null)
                                    {
                                        cData.timerOption.buttonPath = ParentName(_objTransform);
                                    }
                                }
                                EditorGUILayout.EndHorizontal();

                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.LabelField("Button 경로", GUILayout.MaxWidth(150));
                                    cData.timerOption.buttonPath = EditorGUILayout.TextField(cData.timerOption.buttonPath);
                                }
                                EditorGUILayout.EndHorizontal();

                                // 임시 오브젝트 초기화
                                _objTransform = null;

                                EditorGUILayout.BeginHorizontal();
                                {
                                    EditorGUILayout.LabelField("제한 시간", GUILayout.MaxWidth(150));
                                    cData.timerOption.limitTime = EditorGUILayout.FloatField(cData.timerOption.limitTime, GUILayout.MaxWidth(50));
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        EditorGUILayout.EndVertical();
                    }
                    else
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("기능", GUILayout.MaxWidth(50));
                            cData.timerOption.timerType = (TimerType)EditorGUILayout.EnumPopup(cData.timerOption.timerType);
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.LabelField("제한시간 녹음이 완료될때까지 대기합니다.");

                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();

        }

        private void XRManager(CutData cData)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("기능", GUILayout.MaxWidth(50));
                        cData.xrOption.xrType = (XR_FuncType)EditorGUILayout.EnumPopup(cData.xrOption.xrType);
                    }
                    EditorGUILayout.EndHorizontal();

                    switch (cData.xrOption.xrType)
                    {
                        case XR_FuncType.ChangeControllerMat:
                            EditorGUILayout.BeginHorizontal();
                            {
                                string activeMent = "";
                                EditorGUILayout.LabelField("하이라이트 기능 사용", GUILayout.MaxWidth(130));
                                cData.xrOption.useFunction = EditorGUILayout.Toggle(cData.xrOption.useFunction, GUILayout.MaxWidth(30));
                                activeMent = cData.xrOption.useFunction ? "On" : "Off";
                                EditorGUILayout.LabelField($"[{activeMent}]", GUILayout.MaxWidth(50));
                            }
                            EditorGUILayout.EndHorizontal();
                            break;
                        case XR_FuncType.SendHapticInpulse:
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("진동 세기 (0 ~ 1)", GUILayout.MaxWidth(100));
                                cData.xrOption.hapticAmplitude = EditorGUILayout.Slider(cData.xrOption.hapticAmplitude, 0, 1);
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("진동 시간 (초)", GUILayout.MaxWidth(100));
                                cData.xrOption.hapticDuration = EditorGUILayout.FloatField(cData.xrOption.hapticDuration);
                            }
                            EditorGUILayout.EndHorizontal();
                            break;
                        case XR_FuncType.OutlineColorChange:
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("Outline Color", GUILayout.MaxWidth(200));
                                cData.xrOption.outlineColor = EditorGUILayout.ColorField(cData.xrOption.outlineColor);
                            }
                            EditorGUILayout.EndHorizontal();
                            break;
                        case XR_FuncType.UsingStopButton:
                            EditorGUILayout.BeginHorizontal();
                            {
                                string activeMent = "";
                                EditorGUILayout.LabelField("콘텐츠 정지 버튼 사용", GUILayout.MaxWidth(130));
                                cData.xrOption.useFunction = EditorGUILayout.Toggle(cData.xrOption.useFunction, GUILayout.MaxWidth(30));
                                activeMent = cData.xrOption.useFunction ? "On" : "Off";
                                EditorGUILayout.LabelField($"[{activeMent}]", GUILayout.MaxWidth(50));
                            }
                            EditorGUILayout.EndHorizontal();
                            break;
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }


        #region VisualType Editor            

        private void Lay(CutData cData)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("컨트롤러 Lay (체크가 ON)", GUILayout.MaxWidth(180));
                cData.layOption.isOn = EditorGUILayout.Toggle(cData.layOption.isOn);
            }
            EditorGUILayout.EndHorizontal();
        }


        private void Video(CutData cData)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("비디오 종류", GUILayout.MaxWidth(80));
                    cData.videoOption.videoType = (VideoType)EditorGUILayout.EnumPopup(cData.videoOption.videoType);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("기능", GUILayout.MaxWidth(80));
                    cData.videoOption.state = (VideoState)EditorGUILayout.EnumPopup(cData.videoOption.state);
                }
                EditorGUILayout.EndHorizontal();

                switch (cData.videoOption.state)
                {
                    case VideoState.Load:
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("url 경로", GUILayout.MaxWidth(80));
                            cData.videoOption.urlPath = EditorGUILayout.TextField(cData.videoOption.urlPath);
                        }
                        EditorGUILayout.EndHorizontal();
                        break;
                    case VideoState.Play:
                        EditorGUILayout.LabelField("영상을 재생합니다.");
                        //EditorGUILayout.BeginHorizontal();
                        //{
                        //    EditorGUILayout.LabelField("자막 사용", GUILayout.MaxWidth(80));
                        //    cData.videoOption.useSubtitle = EditorGUILayout.Toggle(cData.videoOption.useSubtitle);
                        //}
                        //EditorGUILayout.EndHorizontal();
                        //if (cData.videoOption.useSubtitle)
                        //{
                        //    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                        //    {
                        //        if (cData.videoOption.subtitleType == VideoSubtitleType.None)
                        //        {
                        //            EditorGUILayout.LabelField("진행할 콘텐츠 Type", GUILayout.MaxWidth(120));
                        //            GUI.color = Color.red;
                        //            cData.videoOption.subtitleType = (VideoSubtitleType)EditorGUILayout.EnumPopup(cData.videoOption.subtitleType);
                        //            GUI.color = Color.white;
                        //        }
                        //        else
                        //        {
                        //            EditorGUILayout.LabelField("진행할 콘텐츠 Type", GUILayout.MaxWidth(120));
                        //            cData.videoOption.subtitleType = (VideoSubtitleType)EditorGUILayout.EnumPopup(cData.videoOption.subtitleType);
                        //        }

                        //    }
                        //    EditorGUILayout.EndHorizontal();
                        //}
                        break;
                    case VideoState.Pause:
                        EditorGUILayout.LabelField("영상을 일시정지합니다.");
                        break;
                    case VideoState.Stop:
                        EditorGUILayout.LabelField("영상을 정지합니다.");
                        break;
                    case VideoState.Seek:
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("이동할 시간(s)", GUILayout.MaxWidth(100));
                            cData.videoOption.sTime = EditorGUILayout.DoubleField(cData.videoOption.sTime);
                        }
                        EditorGUILayout.EndHorizontal();
                        break;
                    case VideoState.SeekFrame:
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("이동할 프레임", GUILayout.MaxWidth(100));
                            cData.videoOption.seekFrame = EditorGUILayout.LongField(cData.videoOption.seekFrame);
                        }
                        EditorGUILayout.EndHorizontal();
                        break;
                    case VideoState.Repeat:
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("반복시작 시간(s)", GUILayout.MaxWidth(100));
                            cData.videoOption.sTime = EditorGUILayout.DoubleField(cData.videoOption.sTime);
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("반복종료 시간(s)", GUILayout.MaxWidth(100));
                            cData.videoOption.eTime = EditorGUILayout.DoubleField(cData.videoOption.eTime);
                        }
                        EditorGUILayout.EndHorizontal();
                        break;
                    case VideoState.UnRepeat:
                        EditorGUILayout.LabelField("구간반복을 종료합니다.");
                        break;
                    case VideoState.SeekAndRepeat:
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("Seek 시작 시간", GUILayout.MaxWidth(100));
                            cData.videoOption.sTime = EditorGUILayout.DoubleField(cData.videoOption.sTime);
                        }
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("반복 종료 시간", GUILayout.MaxWidth(100));
                            cData.videoOption.eTime = EditorGUILayout.DoubleField(cData.videoOption.eTime);
                        }
                        EditorGUILayout.EndHorizontal();
                        break;
                    case VideoState.Jump:
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("점프 값", GUILayout.MaxWidth(100));
                            cData.videoOption.jumpLenth = EditorGUILayout.DoubleField(cData.videoOption.jumpLenth);
                        }
                        EditorGUILayout.EndHorizontal();
                        break;
                    case VideoState.Loop:
                        EditorGUILayout.LabelField("영상을 반복합니다.");
                        break;
                    case VideoState.UnLoop:
                        EditorGUILayout.LabelField("영상반복을 종료합니다.");
                        break;
                    case VideoState.Rotation:
                        EditorGUILayout.BeginHorizontal();
                        {
                            cData.videoOption.rotation = EditorGUILayout.Vector3Field("VR3D rotation", cData.videoOption.rotation);
                        }
                        EditorGUILayout.EndHorizontal();
                        break;
                    case VideoState.Position:
                        EditorGUILayout.BeginHorizontal();
                        {
                            cData.videoOption.position = EditorGUILayout.Vector3Field("VR3D position", cData.videoOption.position);
                        }
                        EditorGUILayout.EndHorizontal();
                        break;
                    case VideoState.Restart:
                        EditorGUILayout.LabelField("영상을 처음부터 다시 재생합니다.");
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("자막 사용", GUILayout.MaxWidth(80));
                            cData.videoOption.useSubtitle = EditorGUILayout.Toggle(cData.videoOption.useSubtitle);
                        }
                        EditorGUILayout.EndHorizontal();
                        break;
                    case VideoState.WaitingPopup:
                        EditorGUILayout.LabelField("콘텐츠 진행 팝업이 나올때까지 대기한 이후에 설정 시간이 지나면 영상을 정지합니다.");
                        break;
                    case VideoState.WaitingEndVideo:
                        EditorGUILayout.LabelField("영상이 끝날때까지 [Main - Cut Loop]를 대기합니다.");
                        break;
                    default:
                        break;
                }
            }
            EditorGUILayout.EndVertical();

        }

        private void Narration(CutData cData)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("기능", GUILayout.MaxWidth(50));
                        cData.narrationOption.soundType = (Sound_FuncType)EditorGUILayout.EnumPopup(cData.narrationOption.soundType);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                if (cData.narrationOption.soundType != Sound_FuncType.PlaySubtitle)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("대상", GUILayout.MaxWidth(50));
                            cData.narrationOption.narrationType = (NarrationType)EditorGUILayout.EnumPopup(cData.narrationOption.narrationType);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();

                }

                if (cData.narrationOption.narrationType == NarrationType.BackgroundSound)
                    cData.narrationOption.isSameTime = true;

                //if (cData.narrationOption.soundType == Sound_FuncType.SetVolume && cData.narrationOption.narrationType == NarrationType.Ambience)
                //{
                //    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                //    {

                //    }
                //    EditorGUILayout.EndVertical();

                //}

                switch (cData.narrationOption.soundType)
                {
                    case Sound_FuncType.PlaySound:
                        EditorGUILayout.BeginHorizontal();
                        {
                            string sameText = cData.narrationOption.narrationType == NarrationType.BackgroundSound ? "동시 진행 (true)" : "동시 진행";
                            EditorGUILayout.LabelField(sameText, GUILayout.MaxWidth(100));
                            cData.narrationOption.isSameTime = EditorGUILayout.Toggle(cData.narrationOption.isSameTime);
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("볼륨 (0 - 1)", GUILayout.MaxWidth(100));
                            cData.narrationOption.volume = EditorGUILayout.Slider(cData.narrationOption.volume, 0, 1);
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("루프", GUILayout.MaxWidth(100));
                            cData.narrationOption.loop = EditorGUILayout.Toggle(cData.narrationOption.loop);
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        {
                            cData.narrationOption.clip = EditorGUILayout.ObjectField(cData.narrationOption.clip, typeof(AudioClip), false) as AudioClip;

                            if (cData.narrationOption.clip != null)
                            {
                                // 소수점 3째 자리까지만 반환
                                EditorGUILayout.LabelField("  " + Math.Truncate(cData.narrationOption.clip.length * 1000) / 1000 + " 초", GUILayout.MaxWidth(80));
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                        break;
                    case Sound_FuncType.SetVolume:
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("볼륨 (0 - 1)", GUILayout.MaxWidth(80));
                            cData.narrationOption.volume = EditorGUILayout.Slider(cData.narrationOption.volume, 0, 1);
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("볼륨 Fade 시간", GUILayout.MaxWidth(100));
                            cData.narrationOption.volumeFadeTime = EditorGUILayout.FloatField(cData.narrationOption.volumeFadeTime, GUILayout.MaxWidth(50));
                        }
                        EditorGUILayout.EndHorizontal();
                        break;
                    case Sound_FuncType.PlaySubtitle:
                        EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                        {
                            EditorGUILayout.LabelField("진행할 콘텐츠 Type", GUILayout.MaxWidth(120));
                            cData.narrationOption.subtitleType = (AudioSubtitleType)EditorGUILayout.EnumPopup(cData.narrationOption.subtitleType);
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("자막 간 시간 (초)", GUILayout.MaxWidth(100));
                            cData.narrationOption.waitSecond = EditorGUILayout.FloatField(cData.narrationOption.waitSecond, GUILayout.MaxWidth(50));
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("볼륨 (0 - 1)", GUILayout.MaxWidth(100));
                            cData.narrationOption.volume = EditorGUILayout.Slider(cData.narrationOption.volume, 0, 1);
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginVertical(cData.narrationOption.useCheckPoint ? EditorStyles.helpBox : EditorStyles.whiteLabel);
                        {
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("Check Point 사용", GUILayout.MaxWidth(110));
                                cData.narrationOption.useCheckPoint = EditorGUILayout.Toggle(cData.narrationOption.useCheckPoint, GUILayout.MaxWidth(40));

                                if (cData.narrationOption.useCheckPoint)
                                    cData.narrationOption.checkSubtitleList = ListController(cData.narrationOption.checkSubtitleList);
                            }
                            EditorGUILayout.EndHorizontal();

                            if (cData.narrationOption.useCheckPoint)
                            {
                                EditorGUILayout.Space();
                                for (int cnt = 0; cnt < cData.narrationOption.checkSubtitleList.Count; cnt++)
                                {
                                    if (cData.narrationOption.checkSubtitleList.Count <= cnt) return;

                                    SubtitleCheckPoint checkPoint = cData.narrationOption.checkSubtitleList[cnt];
                                    if (checkPoint == null) return; // 0번째 Null 에러 없애기

                                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                                    {
                                        EditorGUILayout.BeginHorizontal();
                                        {
                                            GUI.color = Color.green;
                                            EditorGUILayout.LabelField($"[{cnt}] Check 자막 번호", GUILayout.MaxWidth(120));
                                            GUI.color = Color.white;
                                            checkPoint.checkSubtitleNum = EditorGUILayout.IntField(checkPoint.checkSubtitleNum, GUILayout.MaxWidth(30));

                                            if (cnt != 0 && checkPoint.checkSubtitleNum <= cData.narrationOption.checkSubtitleList[cnt - 1].checkSubtitleNum)
                                                checkPoint.checkSubtitleNum = 0;

                                            checkPoint.checkEventList = ListController(checkPoint.checkEventList);

                                        }
                                        EditorGUILayout.EndHorizontal();

                                        for (int i = 0; i < checkPoint.checkEventList.Count; i++)
                                        {
                                            if (checkPoint.checkEventList.Count <= i) return;

                                            CutData checkPoint_Event = checkPoint.checkEventList[i];
                                            if (checkPoint_Event == null) return;

                                            if (i == 0)
                                                EditorGUILayout.Space();

                                            if (checkPoint_Event.visualType == VisualType.Line)
                                                GUI.color = Color.red;
                                            //else if (i % 2 == 0)
                                            //    GUI.color = Color.white;
                                            else
                                                GUI.color = Color.yellow;

                                            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                                            {
                                                EditorGUILayout.BeginHorizontal();
                                                {
                                                    EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(10));
                                                    {
                                                        checkPoint_Event._isFold = EditorGUILayout.Toggle(checkPoint_Event._isFold, EditorStyles.foldout, GUILayout.MaxWidth(10));
                                                        EditorGUILayout.LabelField("", GUILayout.MaxWidth(2));
                                                    }
                                                    EditorGUILayout.EndHorizontal();
                                                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.MaxWidth(110));
                                                    {
                                                        ListController(checkPoint.checkEventList, i, false);

                                                        //EditorGUILayout.LabelField($"[{cnt}] Cut", GUILayout.MaxWidth(50));
                                                        //Change_CutCnt(_sceneData.cutDataList, cnt);

                                                    }
                                                    EditorGUILayout.EndHorizontal();

                                                    checkPoint_Event.visualType = (VisualType)EditorGUILayout.EnumPopup(checkPoint_Event.visualType);
                                                }
                                                EditorGUILayout.EndHorizontal();

                                                if (checkPoint.checkEventList.Count <= i) return;
                                                if (checkPoint.checkEventList[i]._isFold)
                                                {
                                                    switch (checkPoint_Event.visualType)
                                                    {
                                                        case VisualType.Line:
                                                            //GUI.color = Color.red;
                                                            //EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                                                            //EditorGUILayout.LabelField(" ");
                                                            //EditorGUILayout.EndHorizontal();
                                                            //GUI.color = Color.white;
                                                            break;
                                                        case VisualType.Narration:
                                                            Narration(checkPoint_Event);
                                                            break;
                                                        case VisualType.Wait:
                                                            WaitTime(checkPoint_Event);
                                                            break;
                                                        case VisualType.UI:
                                                            UI(checkPoint_Event, i);
                                                            break;
                                                        case VisualType.FadeInOut:
                                                            FadeInOut(checkPoint_Event);
                                                            break;
                                                        case VisualType.Animation:
                                                            Animation(checkPoint_Event);
                                                            break;
                                                        case VisualType.Video:
                                                            Video(checkPoint_Event);
                                                            break;
                                                        case VisualType.Lay:
                                                            Lay(checkPoint_Event);
                                                            break;
                                                        case VisualType.Object:
                                                            Object(checkPoint_Event);
                                                            break;
                                                        case VisualType.SceneMgr:
                                                            SceneMgr(checkPoint_Event);
                                                            break;
                                                        case VisualType.Timer:
                                                            SetTimer(checkPoint_Event);
                                                            break;
                                                        case VisualType.XRManager:
                                                            XRManager(checkPoint_Event);
                                                            break;
                                                        default:
                                                            break;
                                                    }

                                                }
                                                if (i == checkPoint.checkEventList.Count - 1)
                                                {
                                                    EditorGUILayout.BeginHorizontal();
                                                    {
                                                        EditorGUILayout.LabelField("Check End 자동 체크 사용 여부", GUILayout.MaxWidth(200));
                                                        checkPoint_Event.checkPoint_End = EditorGUILayout.Toggle(checkPoint_Event.checkPoint_End);

                                                    }
                                                    EditorGUILayout.EndHorizontal();
                                                }
                                            }
                                            EditorGUILayout.EndHorizontal();

                                        }

                                        //EditorGUILayout.BeginHorizontal();
                                        //{
                                        //    EditorGUILayout.LabelField("Check 여부", GUILayout.MaxWidth(100));

                                        //    string checkString = cData.narrationOption.checkSubtitleList[cnt].isChecked ? "True" : "False";
                                        //    EditorGUILayout.LabelField(checkString, GUILayout.MaxWidth(100));

                                        //    //checkPoint.isChecked = EditorGUILayout.Toggle(checkPoint.isChecked);
                                        //}
                                        //EditorGUILayout.EndHorizontal();
                                    }
                                    EditorGUILayout.EndVertical();
                                }

                            }
                        }
                        EditorGUILayout.EndVertical();

                        //if (cData.narrationOption.useCheckPoint)
                        //{
                        //    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        //    {
                        //        EditorGUILayout.BeginHorizontal();
                        //        {

                        //        }
                        //        EditorGUILayout.EndHorizontal();
                        //    }
                        //    EditorGUILayout.EndVertical();
                        //}
                        break;
                }


                if (cData.narrationOption.soundType == Sound_FuncType.PlaySound && cData.narrationOption.narrationType == NarrationType.Manager)
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("배경음 Restore 미사용", GUILayout.MaxWidth(130));
                        cData.narrationOption.notRestoreBackground = EditorGUILayout.Toggle(cData.narrationOption.notRestoreBackground);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();

        }

        private void FadeInOut(CutData cData)
        {
            cData.fadeOption.uiCanvasType = (UICanvasType)EditorGUILayout.EnumPopup(cData.fadeOption.uiCanvasType);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("[0 ~ 1] Start Alpha : ", GUILayout.MaxWidth(120));
                    cData.fadeOption.startAlpha = EditorGUILayout.FloatField(cData.fadeOption.startAlpha, GUILayout.MaxWidth(30));
                    if (cData.fadeOption.startAlpha < 0 || cData.fadeOption.startAlpha > 1)
                        cData.fadeOption.startAlpha = Mathf.Clamp(cData.fadeOption.startAlpha, 0, 1);

                    EditorGUILayout.LabelField(" End Alpha : ", GUILayout.MaxWidth(80));
                    cData.fadeOption.endAlpha = EditorGUILayout.FloatField(cData.fadeOption.endAlpha, GUILayout.MaxWidth(30));
                    if (cData.fadeOption.endAlpha < 0 || cData.fadeOption.endAlpha > 1)
                        cData.fadeOption.endAlpha = Mathf.Clamp(cData.fadeOption.endAlpha, 0, 1);

                    EditorGUILayout.LabelField(" Fade Time : ", GUILayout.MaxWidth(80));
                    cData.fadeOption.time = EditorGUILayout.FloatField(cData.fadeOption.time);
                }
                EditorGUILayout.EndHorizontal();

                if (cData.fadeOption.uiCanvasType == UICanvasType.FadeInOutCanvas)
                {
                    //EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    {
                        if (cData.fadeOption.endAlpha == 1) // Fade Out
                        {
                            string activeMent = "";
                            EditorGUILayout.LabelField("Use Loading UI", GUILayout.MaxWidth(100));
                            cData.fadeOption.useLoadingUI = EditorGUILayout.Toggle(cData.fadeOption.useLoadingUI, GUILayout.MaxWidth(30));
                            activeMent = cData.fadeOption.useLoadingUI ? "On" : "Off";
                            EditorGUILayout.LabelField($"[{activeMent}]", GUILayout.MaxWidth(50));
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }


        private void WaitTime(CutData cData)
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            {
                EditorGUILayout.LabelField("Wait", GUILayout.MaxWidth(50));
                cData.waitOption.waitTime = EditorGUILayout.FloatField(cData.waitOption.waitTime);
                if (cData.waitOption.waitTime == 0)
                    cData.waitOption.waitTime = 1;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void UI(CutData cData, int cutNum = 0)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                if (cData.uiOption.uiType != UI_FuncType.Breathe &&
                    cData.uiOption.uiType != UI_FuncType.ActiveBreatheCanvas &&
                    cData.uiOption.uiType != UI_FuncType.ActiveVideoCheckButton)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("해당 UI 오브젝트", GUILayout.MaxWidth(150));
                            objTransform = EditorGUILayout.ObjectField(objTransform, typeof(Transform), true, GUILayout.MaxWidth(150)) as Transform;

                            if (objTransform != null)
                            {
                                cData.uiOption.objPath = ParentName(objTransform);
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("UI 경로", GUILayout.MaxWidth(150));
                            cData.uiOption.objPath = EditorGUILayout.TextField(cData.uiOption.objPath);

                            //GUI.color = Color.red;
                            //if (cData.uiOption.objPath == "1st" && cData.uiOption.uiType == UI_FuncType.AllInactive)
                            //{
                            //    Debug.Log(SceneData.name);
                            //    cData.uiOption.objPath = cData.uiOption.objPath.Replace("1st", "UICanvas/1st");
                            //    EditorUtility.SetDirty(SceneData);
                            //    cData.uiOption.objPath = EditorGUILayout.TextField(cData.uiOption.objPath);
                            //}
                            //GUI.color = Color.white;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();

                    // 임시 오브젝트 초기화
                    objTransform = null;
                    rootParent = null;

                }

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("기능", GUILayout.MaxWidth(50));
                        cData.uiOption.uiType = (UI_FuncType)EditorGUILayout.EnumPopup(cData.uiOption.uiType);
                    }
                    EditorGUILayout.EndHorizontal();

                    switch (cData.uiOption.uiType)
                    {
                        case UI_FuncType.None:
                            break;
                        case UI_FuncType.AddScene_Button:
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("클릭 시 이동 씬", GUILayout.MaxWidth(130));
                                cData.uiOption.nextScene = EditorGUILayout.ObjectField(cData.uiOption.nextScene, typeof(SceneData), false) as SceneData;
                            }
                            EditorGUILayout.EndHorizontal();
                            break;
                        case UI_FuncType.Active:
                            EditorGUILayout.BeginHorizontal();
                            {
                                string activeMent = "";
                                EditorGUILayout.LabelField("isAcitve", GUILayout.MaxWidth(60));
                                cData.uiOption.isActive = EditorGUILayout.Toggle(cData.uiOption.isActive, GUILayout.MaxWidth(30));
                                activeMent = cData.uiOption.isActive ? "On" : "Off";
                                EditorGUILayout.LabelField($"[{activeMent}]", GUILayout.MaxWidth(50));
                            }
                            EditorGUILayout.EndHorizontal();
                            break;
                        case UI_FuncType.Animation:
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("애니메이션 타입 : ", GUILayout.MaxWidth(100));
                                cData.uiOption.uiAnimationOption.aniType = (UIAnimationType)EditorGUILayout.EnumPopup(cData.uiOption.uiAnimationOption.aniType);
                            }
                            EditorGUILayout.EndHorizontal();

                            switch (cData.uiOption.uiAnimationOption.aniType)
                            {
                                case UIAnimationType.Move:
                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        EditorGUILayout.LabelField("Time : ", GUILayout.MaxWidth(100));
                                        cData.uiOption.uiAnimationOption.time = EditorGUILayout.FloatField(cData.uiOption.uiAnimationOption.time);
                                    }
                                    EditorGUILayout.EndHorizontal();

                                    cData.uiOption.uiAnimationOption.startPos = EditorGUILayout.Vector3Field("startPos", cData.uiOption.uiAnimationOption.startPos);
                                    cData.uiOption.uiAnimationOption.endPos = EditorGUILayout.Vector3Field("endPos", cData.uiOption.uiAnimationOption.endPos);

                                    break;
                                case UIAnimationType.ImageFadeInOut:
                                case UIAnimationType.TextFadeInOut:
                                case UIAnimationType.CanvasFadeInOut:
                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        EditorGUILayout.LabelField("Time : ", GUILayout.MaxWidth(100));
                                        cData.uiOption.uiAnimationOption.time = EditorGUILayout.FloatField(cData.uiOption.uiAnimationOption.time);
                                    }
                                    EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        EditorGUILayout.LabelField("start alpha : ", GUILayout.MaxWidth(100));
                                        cData.uiOption.uiAnimationOption.startA = EditorGUILayout.FloatField(cData.uiOption.uiAnimationOption.startA);

                                        EditorGUILayout.LabelField("end alpha : ", GUILayout.MaxWidth(100));
                                        cData.uiOption.uiAnimationOption.endA = EditorGUILayout.FloatField(cData.uiOption.uiAnimationOption.endA);
                                    }
                                    EditorGUILayout.EndHorizontal();
                                    break;
                                case UIAnimationType.ChangeImage:
                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        EditorGUILayout.LabelField("교환이미지 : ", GUILayout.MaxWidth(100));
                                        cData.uiOption.uiAnimationOption.changeSprite = EditorGUILayout.ObjectField(cData.uiOption.uiAnimationOption.changeSprite, typeof(Sprite), false) as Sprite;
                                    }
                                    EditorGUILayout.EndHorizontal();
                                    break;
                                case UIAnimationType.ChangeText:
                                    EditorGUILayout.BeginHorizontal();
                                    {
                                        EditorGUILayout.LabelField("바꿀Text : ", GUILayout.MaxWidth(100));
                                        cData.uiOption.uiAnimationOption.changeText = EditorGUILayout.TextField(cData.uiOption.uiAnimationOption.changeText);
                                    }
                                    EditorGUILayout.EndHorizontal();
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case UI_FuncType.Transform:
                            cData.uiOption.pos_UI = EditorGUILayout.Vector3Field("Position", cData.uiOption.pos_UI);
                            cData.uiOption.rot_UI = EditorGUILayout.Vector3Field("Rotation", cData.uiOption.rot_UI);
                            break;
                        case UI_FuncType.AddFunc_Button:
                            break;
                        case UI_FuncType.ActiveBreatheCanvas:
                            EditorGUILayout.BeginHorizontal();
                            {
                                string activeMent = "";
                                EditorGUILayout.LabelField("isAcitve", GUILayout.MaxWidth(60));
                                cData.uiOption.isActive = EditorGUILayout.Toggle(cData.uiOption.isActive, GUILayout.MaxWidth(30));
                                activeMent = cData.uiOption.isActive ? "On" : "Off";
                                EditorGUILayout.LabelField($"[{activeMent}]", GUILayout.MaxWidth(50));
                            }
                            EditorGUILayout.EndHorizontal();
                            break;
                        case UI_FuncType.Breathe:
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("숨쉬기 횟수", GUILayout.MaxWidth(100));
                                cData.uiOption.breatheCnt = EditorGUILayout.IntField(cData.uiOption.breatheCnt, GUILayout.MaxWidth(30));
                            }
                            EditorGUILayout.EndHorizontal();

                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("동시 진행", GUILayout.MaxWidth(100));
                                cData.uiOption.isSame = EditorGUILayout.Toggle(cData.uiOption.isSame);
                            }
                            EditorGUILayout.EndHorizontal();
                            EditorGUILayout.BeginHorizontal();
                            {
                                EditorGUILayout.LabelField("호흡 참기 사용", GUILayout.MaxWidth(100));
                                cData.uiOption.useHoldBreath = EditorGUILayout.Toggle(cData.uiOption.useHoldBreath);
                            }
                            EditorGUILayout.EndHorizontal();
                            break;
                        case UI_FuncType.ActiveVideoCheckButton:
                            EditorGUILayout.LabelField("영상 콘텐츠 완료 여부를 체크하여 [Main Menu]의 버튼을 활성화합니다.");
                            break;
                    }
                }
                EditorGUILayout.EndVertical();

            }
            EditorGUILayout.EndVertical();
        }
        private void Animation(CutData cData)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("기능", GUILayout.MaxWidth(50));
                        cData.animationOption.animType = (Anim_FuncType)EditorGUILayout.EnumPopup(cData.animationOption.animType);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                if (cData.animationOption.animType == Anim_FuncType.PlayAnimation)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    {
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("대상", GUILayout.MaxWidth(50));
                            cData.animationOption.charType = (CharType)EditorGUILayout.EnumPopup(cData.animationOption.charType);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("동시 진행", GUILayout.MaxWidth(80));
                        cData.animationOption.isSame = EditorGUILayout.Toggle(cData.animationOption.isSame);
                    }
                    EditorGUILayout.EndHorizontal();

                    float clipLength = 0;
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("클립", GUILayout.MaxWidth(70));

                        cData.animationOption.clip = EditorGUILayout.ObjectField(cData.animationOption.clip, typeof(AnimationClip), false) as AnimationClip;
                        if (cData.animationOption.clip != null)
                        {
                            // 소수점 3째 자리까지만 반환
                            EditorGUILayout.LabelField("  " + Math.Truncate(cData.animationOption.clip.length * 1000) / 1000 + " 초", GUILayout.MaxWidth(80));

                            clipLength = cData.animationOption.clip.length;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }

        #endregion

        #region ListController 
        private List<T> ListController<T>(List<T> list, bool isRight = true)
        {
            if (list == null)
                list = new List<T>();

            if (isRight)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
            }
            else
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(120));
            {

                EditorGUILayout.LabelField($"Total[{list.Count}]", GUILayout.MaxWidth(60));


                if (GUILayout.Button("R", GUILayout.Width(20)))
                {
                    if (list.Count != 0)
                    {
                        if (EditorUtility.DisplayDialog("경고", "초기화 하겠습니까?\n복구 불가능", "네", "아니오"))
                            list = new List<T>();
                    }
                    else
                        EditorUtility.DisplayDialog("경고", "초기화 할 데이터가 없습니다.", "닫기");
                }
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    list.Add(default);
                }
                if (GUILayout.Button("-", GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog("경고", "갯수를 줄이겠습니까?\n줄이면 값이 사라집니다.", "네", "아니오"))
                    {
                        if (list.Count != 0)
                            list.RemoveAt(list.Count - 1);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            return list;
        }

        private List<T> ListController<T>(List<T> list, int num, bool isRight = true)
        {
            if (list == null)
                list = new List<T>();
            if (list.Count == 0)
                list.Add(default);

            GUI.color = Color.yellow;
            if (isRight)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
            }
            else
            {
                EditorGUILayout.BeginHorizontal(GUILayout.MaxWidth(60));
                if (GUILayout.Button("D", GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog("경고", "삭제 하겠습니까??\n복구가 불가능합니다.", "네", "아니오"))
                        list.RemoveAt(num);
                }

                if (GUILayout.Button("C", GUILayout.Width(20)))
                {
                    list.Insert(num + 1, default);
                }

                //if (GUILayout.Button("↔", GUILayout.Width(20)))
                //{

                //}

                EditorGUI.BeginDisabledGroup(!(0 < num));
                {
                    if (GUILayout.Button("△", GUILayout.Width(20)))
                    {
                        list.Insert(num - 1, list[num]);
                        list.RemoveAt(num + 1);
                    }
                }
                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(!(num < list.Count - 1));
                {
                    if (GUILayout.Button("▽", GUILayout.Width(20)))
                    {
                        list.Insert(num + 2, list[num]);
                        list.RemoveAt(num);
                    }
                }
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.LabelField($"[{num}] Cut", GUILayout.MaxWidth(50));

            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;

            return list;
        }

        private void Change_CutCnt(int cnt)
        {
            //if (_num != 0 && _num < num)
            //{
            //    list.Insert(_num, list[num]);
            //    Debug.Log(num + " - 위로");
            //    list.RemoveAt(num + 1);
            //}
            //else if (_num > num)
            //{
            //    list.Insert(_num, list[num]);
            //    Debug.Log(num + " - 아래로");
            //    list.RemoveAt(num);
            //}
            //else // 0일떄
            //{
            //    Debug.Log("영 입니다.");
            //}

        }
        #endregion

        #region Other
        /// <summary>
        /// 모든 부모를 찾아 해당 오브젝트의 씬 내 주소를 반환합니다.
        /// </summary>
        /// <param name="me"></param>
        /// <returns></returns>
        private string ParentName(Transform me)
        {
            string name = me.name;

            rootParent = me.root;
            if (me.parent != null && me.parent.name != rootParent.name)
                name = ParentName(me.parent) + "/" + name;

            return name;
        }

        private float TotalWaitTime()
        {
            float TotalWaitTime = 0;

            for (int cnt = 0; cnt < SceneData.cutDataList.Count; cnt++)
            {
                if (SceneData.cutDataList.Count <= cnt) return TotalWaitTime;

                CutData cutData = SceneData.cutDataList[cnt];

                if (cutData.visualType == VisualType.Wait)
                {
                    TotalWaitTime += cutData.waitOption.waitTime;

                }
            }

            return TotalWaitTime;
        }


        #endregion
    }

#endif

}