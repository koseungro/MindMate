/// 작성자: 조효련
/// 작성일: 2019-11-27
/// 수정일: 
/// 저작권: Copyright(C) FNI Co., LTD. 
/// 수정이력
/// 

using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Android;


/// <summary>
/// 안드로이드 권한은 https://developer.android.com/guide/topics/security/permissions?hl=ko 페이지를 참고함.
/// 6.0(SDK 23)이상일 경우 앱이 시작할 때 뭍는 것이 아니라 필요할 때 물어하는 것으로 변경되었음.
/// 사용법 : AndroidNativePlugin.CheckExternalStorageWritePermission(null);
/// </summary>
/// 
namespace FNI.Common.Utils
{
    public class AndroidNativePlugin
    {
        #region 멤버 변수
        private static AndroidJavaObject currentActivity = null;
        private static AndroidJavaClass unityPlayer = null;
        private static AndroidJavaObject context = null;

        private static AndroidJavaObject toast = null;

        private static bool isCheckPermissionRoutine = false;
        #endregion


        #region 속성
        private static AndroidJavaObject CurrentActivity
        {
            get
            {
                if (currentActivity == null)
                    InitAndroidNative();

                return currentActivity;
            }
        }

        private static AndroidJavaClass UnityPlayer
        {
            get
            {
                if (unityPlayer == null)
                    InitAndroidNative();

                return unityPlayer;
            }
        }

        private static AndroidJavaObject Context
        {
            get
            {
                if (context == null)
                    InitAndroidNative();

                return context;
            }
        }
        #endregion


        #region 경로

        // 에디터 상에서는 Assets 폴더와 같은 레벨에 folderName 폴더 하위의 fileName을 가진 파일을
        // 안드로이드의 경우 외장메모리의 folderName 폴더 하위의 fileName을 가진 파일 경로를 반환하는 함수
        public static string ExternalStoragePath(string folderName, string fileName)
        {
            string path = string.Empty;
            if (Application.isEditor)
            {
                StringBuilder sb = new StringBuilder("");
                sb.Append(@"file:///");
                sb.Append(Application.dataPath);
                sb.Replace("Assets", folderName);
                sb.Append("/");
                sb.Append(fileName);

                path = sb.ToString();
            }
            else
            {
                // 안드로이드 공용 폴더 사용
                string rootPath = Application.persistentDataPath.Substring(0, Application.persistentDataPath.IndexOf("Android", StringComparison.Ordinal));
                path = Path.Combine(Path.Combine(rootPath, folderName), fileName);
            }

            return path;
        }
        #endregion


        #region public 함수
        // 플러그인 초기화
        private static void InitNative()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            InitAndroidNative();
#else
            Debug.Log("OpenAppSetting::");
#endif
        }

        // 현재 앱 설정창 표시하기
        public static void OpenAppSetting()
        {
            try
            {
#if UNITY_ANDROID && !UNITY_EDITOR
                OpenAppSettingAndroidNative();
#else
                Debug.Log("OpenAppSetting::");
#endif
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        // 토스트 메세지 표시하기
        public static void ShowToast(string message)
        { 
#if UNITY_ANDROID && !UNITY_EDITOR
            ShowToastAndroidNative(message);
#else
            Debug.Log("ShowToast::" + message);
#endif
        }

        // 토스트 메세지 숨기기
        public static void CancelToast()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            CancelToastAndroidNative();
#else
            Debug.Log("CancelToast::");
#endif
        }

        // 알림창(확인) 표시하기
        public static void ShowAlert(string title, string message)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            ShowAlertAndroidNative(title, message);
#else
            Debug.Log("ShowAlert::" + message);
#endif
        }

        // 알림창(확인, 취소) 표시하기
        public static void ShowDialog(string appId, string title, string message, string okButton = "OK", string cancelButton = "Cancel", Action<bool> callback = null)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            ShowDialogAndroidNative(appId, title, message, okButton, cancelButton, callback);
#else
            Debug.Log("ShowDialog::" + message);
#endif  
        }

        // 권한 체크하기
        public static async void CheckPermission(string permission, Action hasAuthorizedPermissionCallbock)
        {
            if (isCheckPermissionRoutine == false)
            {
                await CheckPermissionAsync(permission, hasAuthorizedPermissionCallbock);
            }
        }

        // 외장 메모리 읽기 및 쓰기 권한 체크하기
        public static async void CheckExternalStorageWritePermission(Action hasAuthorizedPermissionCallbock)
        {
            if (isCheckPermissionRoutine == false)
            {
                await CheckPermissionAsync(Permission.ExternalStorageWrite, hasAuthorizedPermissionCallbock);
            }
        }

        #endregion


        #region 안드로이드 실제 기기에서 실행되는 함수들

        // 플러그인 초기화 - 유니티 액티비티 가져오기
        private static void InitAndroidNative()
        {
            unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("CurrentActivity");
            context = CurrentActivity.Call<AndroidJavaObject>("getApplicationContext");
        }

        // 현재 앱 설정창 표시하기
        private static void OpenAppSettingAndroidNative()
        {
            string packageName = CurrentActivity.Call<string>("getPackageName");

            using (var uriClass = new AndroidJavaClass("android.net.Uri"))
            using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
            using (var intent = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject))
            {
                intent.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                intent.Call<AndroidJavaObject>("setFlags", 0x10000000);
                CurrentActivity.Call("startActivity", intent);
            }
        }

        // 토스트 메세지 표시하기
        private static void ShowToastAndroidNative(string message)
        {
            CurrentActivity.Call
            (
                "runOnUiThread",
                new AndroidJavaRunnable(() =>
                {
                    AndroidJavaClass Toast = new AndroidJavaClass("android.widget.Toast");
                    AndroidJavaObject javaString = new AndroidJavaObject("java.lang.String", message);

                    toast = Toast.CallStatic<AndroidJavaObject>
                    (
                        "makeText",
                        Context,
                        javaString,
                        Toast.GetStatic<int>("LENGTH_SHORT")
                    );

                    toast.Call("show");
                })
            );
        }

        // 토스트 메세지 숨기기
        private static void CancelToastAndroidNative()
        {
            CurrentActivity.Call
            (
                "runOnUiThread",
                new AndroidJavaRunnable(() =>
                {
                    if (toast != null)
                        toast.Call("cancel");
                })
            );
        }

        // 알림창(확인) 표시하기
        private static void ShowAlertAndroidNative(string title, string message)
        {
            var alertDialog = new AndroidJavaObject("android.app.AlertDialog$Builder", CurrentActivity);
            alertDialog.Call<AndroidJavaObject>("setTitle", title);
            alertDialog.Call<AndroidJavaObject>("setMessage", message);
            alertDialog.Call<AndroidJavaObject>("setPositiveButton", "확인", null);
            alertDialog.Call<AndroidJavaObject>("show");
        }

        // 알림창(확인, 취소) 표시하기
        private static void ShowDialogAndroidNative(string appId, string title, string message, string okButton = "OK", string cancelButton = "Cancel", Action<bool> callback = null)
        {
            var alertDialog = new AndroidJavaObject("android.app.AlertDialog$Builder", CurrentActivity);
            alertDialog.Call<AndroidJavaObject>("setTitle", title);
            alertDialog.Call<AndroidJavaObject>("setMessage", message);
            alertDialog.Call<AndroidJavaObject>("setPositiveButton", okButton, new AndroidOnClickListener((int which) => {

                // https://play.google.com/store/apps/details?id=lost.animal.main
                // Application.OpenURL("https://play.google.com/store/apps/details?id=" + appId);
                if (callback != null)
                {
                    callback.Invoke(true);
                }
            }));
            alertDialog.Call<AndroidJavaObject>("setNegativeButton", cancelButton, new AndroidOnClickListener((int which) => {
                if (callback != null)
                {
                    callback.Invoke(false);
                }
            }));
            alertDialog.Call<AndroidJavaObject>("show");
        }

        // 권한 체크하는 프로세스
        private static async Task CheckPermissionAsync(string permission, Action hasAuthorizedPermissionCallbock)
        {
            isCheckPermissionRoutine = true;

            await WaitSecondTimeAsync(1);

            // 권한을 가지고 있는지 체크하기
            if (Permission.HasUserAuthorizedPermission(permission) == false)
            {
                // 권한 창을 표시함
                Permission.RequestUserPermission(permission);

                // 0.2초의 딜레이 후 focus를 체크하기
                await WaitSecondTimeAsync(0.2f);
                while (!Application.isFocused)
                    await Task.Yield();

                // 없을 경우에는 앱 설정창 열기
                if (Permission.HasUserAuthorizedPermission(permission) == false)
                {
                    string message = string.Empty;
                    switch (permission)
                    {
                        // STORAGE
                        case Permission.ExternalStorageRead:
                        case Permission.ExternalStorageWrite: message = "동영상을 플레이하기 위해서 저장소 접근 권한이 필요합니다."; break;

                        // CAMERA
                        case Permission.Camera: message = "사진 찰영을 위하여 카메라 권한이 필요합니다."; break;

                        // MICROPHONE
                        case Permission.Microphone: message = "녹음을 위하여 마이크 권한이 필요합니다."; break;

                        // LOCATION
                        case Permission.FineLocation:
                        case Permission.CoarseLocation: message = "보다 정확한 위치를 가져오기 위해서 위치 정보 액세스 권한 권한이 필요합니다."; break;

                            // SMS

                            // SENSORS

                            // PHONE

                            // CONTACTS

                            // CALENDAR
                    }

                    ShowDialog("", "권한 필요", message, "확인", "취소", (value) => {
                        if (value)
                            OpenAppSetting();
                    });


                    isCheckPermissionRoutine = false;
                }
            }
            // 권한이 있을 경우 콜백함수 실행하기
            else
            {
                if (hasAuthorizedPermissionCallbock != null)
                    hasAuthorizedPermissionCallbock.Invoke();
            }

            isCheckPermissionRoutine = false;
        }

        private static async Task WaitSecondTimeAsync(float time) => await Task.Delay(TimeSpan.FromSeconds(time));

        #endregion


        // 안드로이드 클릭 콜백함수
        private class AndroidOnClickListener : AndroidJavaProxy
        {
            private Action<int> action = null;
            public AndroidOnClickListener(Action<int> action) : base("android.content.DialogInterface$OnClickListener")
            {
                this.action = action;
            }
            public void onClick(AndroidJavaObject dialog, int which)
            {
                this.action.Invoke(which);
            }
        }
    }
}