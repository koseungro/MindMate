#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;

/// <summary>
/// Window 기반의 App의 포커스를 최상단으로 끌어 옵니다.
/// </summary>
public class TopmostForWindows : MonoBehaviour
{
    #region WIN32API

    #region Flags
    public struct HWND
    {
        /// <summary>
        /// Z 순서의 맨 아래에 창을 배치합니다.
        /// hWnd 매개 변수가 최상위 창을 식별하는 경우
        /// 창이 최상위 상태를 잃고 다른 모든 창의 맨 아래에 배치됩니다.
        /// </summary>
        public static readonly System.IntPtr BOTTOM = new System.IntPtr(1);
        /// <summary>
        /// 창을 최상위가 아닌 모든 창 위에 배치합니다 (즉, 모든 최상위 창 뒤에).
        /// 창이 이미 최상위 창이 아닌 경우이 플래그는 효과가 없습니다.
        /// </summary>
        public static readonly System.IntPtr NOT_TOPMOST = new System.IntPtr(-2);
        /// <summary>
        /// Z 순서의 맨 위에 창을 배치합니다.
        /// </summary>
        public static readonly System.IntPtr TOP = new System.IntPtr(0);
        /// <summary>
        /// 창을 최상위가 아닌 모든 창 위에 놓습니다.
        /// 창은 비활성화 된 경우에도 맨 위 위치를 유지합니다.
        /// </summary>
        public static readonly System.IntPtr TOPMOST = new System.IntPtr(-1);
    }
    public struct SWP
    {
        /// <summary>
        /// 현재 크기를 유지합니다 ( cx 및 cy 매개 변수 무시 ).
        /// </summary>
        public const System.UInt32 NOSIZE = 0x0001;
        /// <summary>
        /// 현재 위치를 유지합니다 ( X 및 Y 매개 변수 무시 ).
        /// </summary>
        public const System.UInt32 NOMOVE = 0x0002;
        /// <summary>
        /// 현재 Z 순서를 유지합니다 ( hWndInsertAfter 매개 변수 무시 ).
        /// </summary>
        public const System.UInt32 NOZORDER = 0x0004;
        /// <summary>
        /// 변경 사항을 다시 그리지 않습니다. 
        /// 이 플래그가 설정되면 어떤 종류의 다시 그리기도 발생하지 않습니다. 
        /// 이는 클라이언트 영역, 비 클라이언트 영역 (제목 표시 줄 및 스크롤 막대 포함) 및 
        /// 창 이동의 결과로 드러나지 않은 상위 창의 모든 부분에 적용됩니다. 
        /// 이 플래그가 설정되면 응용 프로그램은 다시 그려야하는 
        /// 창과 부모 창의 모든 부분을 명시 적으로 무효화하거나 다시 그려야합니다.
        /// </summary>
        public const System.UInt32 NOREDRAW = 0x0008;
        /// <summary>
        /// 창을 활성화하지 않습니다. 이 플래그가 설정되지 않은 경우 
        /// 창이 활성화되고 최상위 그룹 또는 최상위 그룹이 아닌 그룹의 맨 위로 이동합니다 
        /// ( hWndInsertAfter 매개 변수 설정에 따라 다름 ).
        /// </summary>
        public const System.UInt32 NOACTIVATE = 0x0010;
        /// <summary>
        /// 창 주위에 프레임 (창의 클래스 설명에 정의 됨)을 그립니다.
        /// </summary>
        public const System.UInt32 DRAWFRAME = 0x0020;
        /// <summary>
        /// SetWindowLong 함수를 사용하여 설정된 새 프레임 스타일을 적용 합니다. 
        /// 창 크기가 변경되지 않더라도 창에 WM_NCCALCSIZE 메시지를 보냅니다 . 
        /// 이 플래그를 지정하지 않으면 창 크기가 변경 될 때만 WM_NCCALCSIZE 가 전송됩니다.
        /// </summary>
        public const System.UInt32 FRAMECHANGED = 0x0020;
        /// <summary>
        /// 창을 표시합니다.
        /// </summary>
        public const System.UInt32 SHOWWINDOW = 0x0040;
        /// <summary>
        /// 창을 숨 깁니다.
        /// </summary>
        public const System.UInt32 HIDEWINDOW = 0x0080;
        /// <summary>
        /// 클라이언트 영역의 전체 내용을 버립니다. 
        /// 이 플래그를 지정하지 않으면 클라이언트 영역의 유효한 내용이 저장되고 
        /// 창의 크기가 조정되거나 위치가 변경된 후 클라이언트 영역으로 다시 복사됩니다.
        /// </summary>
        public const System.UInt32 NOCOPYBITS = 0x0100;
        /// <summary>
        /// <see cref="NOOWNERZORDER"/> 플래그 와 동일합니다 .
        /// </summary>
        public const System.UInt32 NOREPOSITION = 0x0200;
        /// <summary>
        /// Z 순서에서 소유자 창의 위치를 변경하지 않습니다.
        /// </summary>
        public const System.UInt32 NOOWNERZORDER = 0x0200;
        /// <summary>
        /// 창이 WM_WINDOWPOSCHANGING 메시지 를받지 못하도록합니다 .
        /// </summary>
        public const System.UInt32 NOSENDCHANGING = 0x0400;
        /// <summary>
        /// WM_SYNCPAINT 메시지 생성을 방지 합니다.
        /// </summary>
        public const System.UInt32 DEFERERASE = 0x2000;
        /// <summary>
        /// 호출 스레드와 창을 소유 한 스레드가 서로 다른 입력 큐에 연결되면
        /// 시스템은 창을 소유 한 스레드에 요청을 게시합니다.
        /// 이것은 다른 스레드가 요청을 처리하는 동안 호출 스레드가 실행을 차단하는 것을 방지합니다.
        /// </summary>
        public const System.UInt32 ASYNCWINDOWPOS = 0x4000;
    }
    private struct SW
    {
        /// <summary>
        /// 창을 숨기고 다른 창을 활성화합니다.
        /// </summary>
        public const int HIDE = 0;
        /// <summary>
        /// 창을 활성화하고 표시합니다. 
        /// 창이 최소화되거나 최대화되면 시스템은 창을 원래 크기와 위치로 복원합니다. 
        /// 응용 프로그램은 창을 처음 표시 할 때이 플래그를 지정해야합니다.
        /// </summary>
        public const int SHOWNORMAL = 1;
        /// <summary>
        /// 창을 활성화하고 최소화 된 창으로 표시합니다.
        /// </summary>
        public const int SHOWMINIMIZED = 2;
        /// <summary>
        /// 창을 활성화하고 최대화 된 창으로 표시합니다.
        /// </summary>
        public const int SHOWMAXIMIZED = 3;
        /// <summary>
        /// 가장 최근의 크기와 위치에 창을 표시합니다. 
        /// 이 값은 창이 활성화되지 않는다는 점을 제외하면 SW_SHOWNORMAL 과 유사합니다 .
        /// </summary>
        public const int SHOWNOACTIVATE = 4;
        /// <summary>
        /// 창을 활성화하고 현재 크기와 위치에 표시합니다.
        /// </summary>
        public const int SHOW = 5;
        /// <summary>
        /// 지정된 창을 최소화하고 Z 순서로 다음 최상위 창을 활성화합니다.
        /// </summary>
        public const int MINIMIZE = 6;
        /// <summary>
        /// 창을 최소화 된 창으로 표시합니다. 
        /// 이 값은 창이 활성화되지 않는다는 점을 제외하면 SW_SHOWMINIMIZED 와 유사합니다 .
        /// </summary>
        public const int SHOWMINNOACTIVE = 7;
        /// <summary>
        /// 창을 현재 크기와 위치로 표시합니다.
        /// 이 값은 창이 활성화되지 않는다는 점을 제외하면 SW_SHOW 와 유사합니다 .
        /// </summary>
        public const int SHOWNA = 8;
        /// <summary>
        /// 창을 활성화하고 표시합니다. 
        /// 창이 최소화되거나 최대화되면 시스템은 창을 원래 크기와 위치로 복원합니다. 
        /// 애플리케이션은 최소화 된 창을 복원 할 때이 플래그를 지정해야합니다.
        /// </summary>
        public const int RESTORE = 9;
        /// <summary>
        /// 애플리케이션을 시작한 프로그램이 
        /// CreateProcess 함수에 전달한 STARTUPINFO 구조에 지정된 SW_ 값을 기반으로 
        /// 표시 상태를 설정합니다 .
        /// </summary>
        public const int SHOWDEFAULT = 10;
        /// <summary>
        /// 창을 소유 한 스레드가 응답하지 않는 경우에도 창을 최소화합니다.
        /// 이 플래그는 다른 스레드에서 창을 최소화 할 때만 사용해야합니다.
        /// </summary>
        public const int FORCEMINIMIZE = 11;
    }
    #endregion

    /// <summary>
    /// 클래스 이름 및 창 이름이 지정된 문자열과 일치하는 최상위 창에 대한 핸들을 검색합니다. 
    /// 이 기능은 자식 창을 검색하지 않습니다. 
    /// 이 기능은 대소 문자를 구분하는 검색을 수행하지 않습니다.
    /// 지정된 자식 창으로 시작하여 자식 창을 검색하려면 FindWindowEx 함수를 사용하십시오.
    /// </summary>
    /// <param name="lpClassName">
    /// RegisterClass 또는 RegisterClassEx 함수에 대한 이전 호출로 생성 된 클래스 이름 또는 클래스 원자입니다.
    /// 원자는 lpClassName의 하위 단어에 있어야합니다. 상위 단어는 0이어야합니다. 
    /// lpClassName이 문자열을 가리키는 경우 창 클래스 이름을 지정합니다.
    /// 클래스 이름은 RegisterClass 또는 RegisterClassEx에 등록 된 모든 이름이 될 거나 사전 정의 된 제어 클래스 이름 중 하나 입니다.
    /// lpClassName이 NULL이면 제목이 lpWindowName 매개 변수와 일치하는 모든 창을 찾습니다.
    /// </param>
    /// <param name="lpWindowName">
    /// 창 이름 (창 제목). 이 매개 변수가 NULL이면 모든 창 이름이 일치합니다.
    /// </param>
    /// <returns>
    /// 함수가 성공하면 반환 값은 지정된 클래스 이름과 창 이름이있는 창에 대한 핸들입니다.
    /// 함수가 실패하면 반환 값은 NULL입니다.
    /// 확장 된 오류 정보를 얻으려면 GetLastError를 호출하십시오.
    /// </returns>
    [DllImport("user32.dll", SetLastError = true)]
    private static extern System.IntPtr FindWindow(String lpClassName, String lpWindowName);

    /// <summary>
    /// 자식, 팝업 또는 최상위 창의 크기, 위치 및 Z 순서를 변경합니다. 
    /// 이 창은 화면의 모양에 따라 정렬됩니다. 
    /// 최상위 창은 가장 높은 순위를 받고 Z 순서에서 첫 번째 창입니다.
    /// </summary>
    /// <param name="hWnd">창 핸들.</param>
    /// <param name="hWndInsertAfter">
    /// (<see cref="HWND"/>사용할 것.)
    /// Z 순서로 배치 된 창 앞에있는 창에 대한 핸들입니다. 
    /// 이 매개 변수는 창 핸들이거나 다음 값 중 하나 여야합니다.
    /// </param>
    /// <param name="x">클라이언트 좌표에서 창 왼쪽의 새 위치입니다.</param>
    /// <param name="y">클라이언트 좌표에서 창 상단의 새 위치입니다.</param>
    /// <param name="cx">창의 새 너비 (픽셀)입니다.</param>
    /// <param name="cy">창의 새 높이 (픽셀)입니다.</param>
    /// <param name="uFlags">창 크기 조정 및 위치 지정 플래그입니다. 이 매개 변수는 다음 값(<see cref="SWP"/>)의 조합 일 수 있습니다.</param>
    /// <returns>
    /// 함수가 성공하면 반환 값은 false가 아닙니다.
    /// 함수가 실패하면 반환 값은 false입니다.확장 된 오류 정보를 얻으려면 GetLastError를 호출하십시오.
    /// </returns>
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetWindowPos(System.IntPtr hWnd, System.IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    /// <summary>
    /// 지정된 창의 표시 상태를 설정합니다.
    /// </summary>
    /// <param name="handle">창 핸들.</param>
    /// <param name="nCmdShow">
    /// (<see cref="SW"/>를 사용하여 옵션 설정할 것)
    /// 창이 표시되는 방법을 제어합니다.
    /// 이 매개 변수는 애플리케이션 을 시작한 프로그램이 STARTUPINFO 구조를 제공하는 경우
    /// 애플리케이션이 처음으로 ShowWindow를 호출 할 때 무시 됩니다.
    /// 그렇지 않으면 ShowWindow 가 처음 호출 될 때 값 은 nCmdShow 매개 변수 의 WinMain 함수에서 얻은 값이어야합니다.
    /// 후속 호출에서이 매개 변수는 다음 값 중 하나 일 수 있습니다.
    /// </param>
    /// <returns>
    /// 창이 이전에 표시되었던 경우 반환 값은 false가 아닙니다.
    /// 창이 이전에 숨겨져있는 경우 반환 값은 false입니다.
    /// </returns>
    [DllImport("User32.dll")]
    public static extern bool ShowWindow(IntPtr handle, int nCmdShow);

    /// <summary>
    /// 키보드 포커스를 지정된 창으로 설정합니다.
    /// 창은 호출 스레드의 메시지 큐에 연결되어야합니다.
    /// </summary>
    /// <param name="hWnd">키보드 입력을받을 창의 핸들입니다. 이 매개 변수가 NULL이면 키 입력이 무시됩니다.</param>
    /// <returns></returns>
    [DllImport("User32.dll", SetLastError = true)]
    public static extern bool SetFocus(IntPtr hWnd);

    /// <summary>
    /// 지정된 창을 만든 스레드를 포 그라운드로 가져와 창을 활성화합니다.
    /// 키보드 입력은 창으로 전달되고 사용자를 위해 다양한 시각적 단서가 변경됩니다.
    /// 시스템은 전경 창을 만든 스레드에 다른 스레드보다 약간 더 높은 우선 순위를 할당합니다.
    /// </summary>
    /// <param name="hWnd">활성화되어 전경으로 가져와야하는 창에 대한 핸들입니다.</param>
    /// <returns>
    /// 창을 전경으로 가져온 경우 반환 값은 false가 아닙니다.
    /// 창이 전경으로 가져 오지 않은 경우 반환 값은 false입니다.
    /// </returns>
    [DllImport("USER32.DLL")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);
    #endregion

    /// <summary>
    /// 이 앱의 윈도우 값을 저장합니다.
    /// </summary>
    private System.IntPtr hWnd;
    /// <summary>
    /// 코루틴 제어용 변수
    /// </summary>
    private IEnumerator focus_Routine;

    private bool isExit = false;

    void Awake()
    {
        hWnd = FindWindow((string)null, Application.productName);

        AssignTopmostWindow(true);

        FocusReset();
    }

    private void OnApplicationQuit()
    {
        isExit = true;

        if (focus_Routine != null)
            StopCoroutine(focus_Routine);
    }

    /// <summary>
    /// 앱의 포커스를 잃어 버리면 포커스 세팅을 재실행 합니다.
    /// </summary>
    /// <param name="hasFocus">포커스 상태</param>
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus == false &&
            isExit == false)
        {
            FocusReset();
        }
    }

    /// <summary>
    /// 포커스 조정하는 코루틴문을 실행합니다.
    /// </summary>
    private void FocusReset()
    {
        if (focus_Routine != null)
            StopCoroutine(focus_Routine);
        focus_Routine = FocusReset_Routine();

        StartCoroutine(focus_Routine);
    }

    /// <summary>
    /// 포커스 조정 코루틴
    /// </summary>
    /// <returns></returns>
    IEnumerator FocusReset_Routine()
    {
        yield return new WaitForSeconds(0.2f);
        ShowWindow(hWnd, SW.SHOWNORMAL);
        yield return new WaitForSeconds(0.2f);
        SetFocus(hWnd);
        yield return new WaitForSeconds(0.2f);
        SetForegroundWindow(hWnd);
    }

    /// <summary>
    /// 창을 최대화 하면서 앞으로 가져옵니다.
    /// </summary>
    /// <returns></returns>
    public bool AssignBringWindowTop()
    {
        return ShowWindow(hWnd, SW.SHOWMAXIMIZED);
    }

    /// <summary>
    /// 전체 화면 모드, 포커스만 사용한다면 하지 않아도 됨
    /// </summary>
    /// <param name="makeTopmost"></param>
    /// <returns></returns>
    public bool AssignTopmostWindow(bool makeTopmost)
    {
        return SetWindowPos(hWnd, makeTopmost ? HWND.TOPMOST : HWND.NOT_TOPMOST, 0, 0, Screen.width, Screen.height, SWP.SHOWWINDOW);
    }
}
#endif