#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class AutoSave : EditorWindow
{
	[MenuItem("AutoSave/Auto Save Time")]
	private static void SetSaveTime(float time)
	{
		if (time < 5)
			time = 5;
		saveTime = time;
	}
	private static float saveTime = 5;
	private static DateTime nextSaveTime;
	private static bool check = false;

	[MenuItem("AutoSave/Auto Save Time")]
	static void Init()
	{
		Debug.Log("AutoSave/Init");
		AutoSave window = (AutoSave)EditorWindow.GetWindow(typeof(AutoSave), true, "Set Auto Save Time");
		window.Show();
	}
    void OnGUI()
	{
		EditorGUILayout.Space();

		EditorGUILayout.BeginVertical();
		{
			EditorGUILayout.BeginHorizontal();
			{ 
				EditorGUI.BeginChangeCheck();

				saveTime = EditorGUILayout.Slider("SaveTime", saveTime, 5, 60);
				EditorGUILayout.LabelField("m", GUILayout.Width(15));

				if (EditorGUI.EndChangeCheck())
				{
					check = true;
				}
			}
			EditorGUILayout.EndHorizontal();

			if (check && GUILayout.Button("Save"))
			{
				check = false;
				PlayerPrefs.SetFloat("SaveTime", saveTime);
			}

		}
		EditorGUILayout.EndVertical();
	}

	// Static constructor that gets called when unity fires up.
	static AutoSave()
	{
		saveTime = PlayerPrefs.GetFloat("SaveTime");
		if (saveTime < 5)
		{
			saveTime = 5;
			PlayerPrefs.SetFloat("SaveTime", saveTime);
		}
#if UNITY_2017_2_OR_NEWER
		EditorApplication.playModeStateChanged += delegate {
			// If we're about to run the scene...
			if (!EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying) return;

			// Save the scene and the assets.
			Debug.Log("Auto-saving all open scenes... ");
			EditorSceneManager.SaveOpenScenes();
			AssetDatabase.SaveAssets();
		};
#else
		EditorApplication.playmodeStateChanged += delegate {
			// If we're about to run the scene...
			if (!EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying) return;

			// Save the scene and the assets.
			Debug.Log("Auto-saving all open scenes... ");
			EditorSceneManager.SaveOpenScenes();
			AssetDatabase.SaveAssets();
		};
#endif

		// Also, every five minutes.
		nextSaveTime = DateTime.Now.AddMinutes(5);
		EditorApplication.update += Update;
	}

	private static void Update()
	{
		if (nextSaveTime > DateTime.Now)
			return;

		nextSaveTime = DateTime.Now.AddMinutes(5);

		if (EditorApplication.isPlaying == true)
			return;

		Debug.Log("AutoSave Scenes: " + DateTime.Now.ToShortTimeString());

		EditorSceneManager.SaveOpenScenes();
		AssetDatabase.SaveAssets();
	}
}
#endif