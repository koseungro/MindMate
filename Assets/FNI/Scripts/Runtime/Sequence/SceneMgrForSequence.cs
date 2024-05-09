using FNI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace FNI
{
    public class SceneMgrForSequence : MonoBehaviour, IVisualObject
    {
        /// <summary>
        /// ���� ������ üũ ��ư ����Ʈ
        /// </summary>
        public List<VideoContentChecker> checkButton_List = new List<VideoContentChecker>();

        public VisualType Type => VisualType.SceneMgr;

        private bool isFinish = false;
        public bool IsFinish { get => isFinish; }

        private Camera cam;

        private Light mainLight;
        public void Active(CutData option)
        {
            isFinish = false;

            switch (option.sceneOption.sceneMgrType)
            {
                case SceneMgrType.Load:
                    StartCoroutine(Load(option.sceneOption));
                    break;
                case SceneMgrType.UnLoad:
                    StartCoroutine(RemoveScene(option.sceneOption));
                    break;
                case SceneMgrType.FogSetting:
                    FogSetting(option.sceneOption);
                    break;
                case SceneMgrType.CameraSetting:
                    CameraSetting(option.sceneOption);
                    break;
                case SceneMgrType.CheckVideoContent_Completed:
                    CheckingVideo(option.sceneOption);
                    break;
                case SceneMgrType.CheckFileDownload:
                    CheckDownload_Completed(option.sceneOption);
                    break;
            }
        }

        public void Init()
        {
            cam = Camera.main;
            mainLight = GameObject.Find("Directional Light").GetComponent<Light>();
        }

        public void MyUpdate()
        {

        }

        /// <summary>
        /// ���� �ٿ�ε尡 �� �Ǿ��ִ��� Ȯ���� �� ��Ʈ�� ���� ������ �������� ���մϴ�.
        /// </summary>
        /// <param name="option"></param>
        public void CheckDownload_Completed(SceneMgrOption option)
        {
            if (VideoDownloadManager.Instance().SetDownloadCompleted())
                Main.Instance.SetNextScene(option.downloadCompleted_Scene);
            else
                Main.Instance.SetNextScene(option.downloadUnCompleted_Scene);

            isFinish = true;
        }

        /// <summary>
        /// �ش� ���� ������ �Ϸ� ���θ� üũ�ϰ� ��ư�� Sprite �̹����� ��ü�մϴ�.
        /// </summary>
        /// <param name="option"></param>
        private void CheckingVideo(SceneMgrOption option)
        {
            VideoContentChecker target = checkButton_List.Find(x => x.videoContentType == option.videoContentType);
            target.isCompleted = true;

            target.GetComponent<IS_Selectable_Suppoter>().animationValues[0].Sprite.Default = target.checkImage;

            isFinish = true;
        }

        private void CameraSetting(SceneMgrOption option)
        {
            cam.clearFlags = option.clearFlags;
            cam.backgroundColor = option.backgroundColor;

            isFinish = true;
        }


        private void FogSetting(SceneMgrOption option)
        {
            RenderSettings.fog = option.fogOn;
            RenderSettings.fogColor = option.fogColor;
            RenderSettings.fogMode = option.fogMode;

            RenderSettings.fogStartDistance = option.startDistance;
            RenderSettings.fogEndDistance = option.endDistance;
            RenderSettings.fogDensity = option.density;

            RenderSettings.haloStrength = option.haloStrength;
            RenderSettings.flareStrength = option.flareStrength;

            isFinish = true;
        }

        private IEnumerator RemoveScene(SceneMgrOption option)
        {
            AsyncOperation asyncLoad = SceneManager.UnloadSceneAsync(option.sceneName);

            while (!asyncLoad.isDone)
            {
                yield return null;
            }

            if (mainLight.gameObject.activeInHierarchy)
                RenderSettings.sun = mainLight;

            isFinish = true;
        }

        private IEnumerator Load(SceneMgrOption option)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(option.sceneName, LoadSceneMode.Additive);

            if (asyncLoad != null)
            {
                while (!asyncLoad.isDone)
                {
                    yield return null;
                }

                if (option.lightPath != "")
                {
                    option.light = GameObject.Find(option.lightPath).GetComponent<Light>();
                    RenderSettings.sun = option.light;
                }

#if UNITY_EDITOR
                //if (option.lightingDataAsset != null)
                //{
                //    Lightmapping.lightingDataAsset = option.lightingDataAsset; // ������Ʈ�� ���ο� Scene �߰��Ǹ� Lighting Data => Generate Lighting �ؾ���
                //}
                //else
                //    Debug.LogError($"{option.sceneName}�� Light Data Asset�� Null �Դϴ�.");
#endif

                //asyncLoad.allowSceneActivation = true;
                isFinish = true;
                Debug.Log($"[SceneMgrForSequence/Load] <color=yellow> [{option.sceneName}] </color> Load Done");
            }
            else
                Debug.Log($"[SceneMgrForSequence/Load] <color=yellow> [{option.sceneName}] </color> is Invalid");


        }
    }
}
