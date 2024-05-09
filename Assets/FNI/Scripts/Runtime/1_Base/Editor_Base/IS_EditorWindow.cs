/// 작성자: 백인성
/// 작성일: 2022-07-05
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using FNI.Common.Utils;

using System;
using System.Collections;
using System.Collections.Generic;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;

namespace FNI.IS.Editor.Window
{
	public class IS_EditorWindow<T> : EditorWindow where T : EditorWindow
	{
		public static Vector2 WindowSize => curWindow == null ? new Vector2() : curWindow.position.size;
		public static float Margin = 4;

		protected static T curWindow;

		protected static bool InitWindow(string title)
		{
			bool finded = FindWindow();

			curWindow.titleContent = new GUIContent() { text = title };
			curWindow.Show();

			return finded;
		}

		protected static void SetWindowSize(Vector2 size, Vector2 min = new Vector2(), Vector2 max = new Vector2())
		{
			if (curWindow == null) return;

			float posX = (Screen.currentResolution.width - size.x) / 2;
			float posY = (Screen.currentResolution.height - size.y) / 2;

			curWindow.position = new Rect(posX, posY, size.x, size.y);

			if (min != Vector2.zero)
				curWindow.minSize = min;
			if (max != Vector2.zero)
				curWindow.maxSize = max;
		}

		protected static bool FindWindow()
		{
			if (curWindow == null)
			{
				curWindow = GetWindow(typeof(T), true) as T;
				return true;
			}
			return false;
		}

		private void OnDestroy()
		{
			curWindow = null;
		}
	}
}
#endif