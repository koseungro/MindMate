using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using FNI.XRST;

namespace FNI.IO
{
	public class FNI_INI
	{
		private string iniPath;

		public FNI_INI(string path)
		{
			this.iniPath = path;
		}
		[DllImport("kernel32.dll")]
		private static extern int GetPrivateProfileString(
			String section,
			String key,
			String def,
			StringBuilder retVal,
			int size,
			String filePath);
		[DllImport("kernel32.dll")]
		private static extern long WritePrivateProfileString(
			String section,
			String key,
			String val,
			String filePath);
		/// <summary>
		/// INI파일의 값을 읽어 옵니다. 한글안됨
		/// </summary>
		/// <param name="Section">대분류</param>
		/// <param name="Key">소분류</param>
		/// <param name="IniPath">INI파일 읽어올 경로</param>
		/// <returns></returns>
		public String GetString(String Section, String Key, String IniPath)
		{
			StringBuilder temp = new StringBuilder(255);
			int i = GetPrivateProfileString(Section, Key, "", temp, 255, IniPath);
			this.iniPath = IniPath;
			return temp.ToString();
		}
		/// <summary>
		/// INI파일의 값을 읽어 옵니다. 한글 안됨
		/// </summary>
		/// <param name="Section">대분류</param>
		/// <param name="Key">소분류</param>
		/// <returns></returns>
		public String GetString(String Section, String Key)
		{
			StringBuilder temp = new StringBuilder(255);
			int i = GetPrivateProfileString(Section, Key, "", temp, 255, iniPath);
			return temp.ToString();
		}
		/// <summary>
		/// INI파일의 값을 읽어 옵니다. 한글 안됨
		/// </summary>
		/// <param name="Section">대분류</param>
		/// <param name="Key">소분류</param>
		/// <returns></returns>
		public bool GetString(String Section, String Key, out string value)
		{
			StringBuilder temp = new StringBuilder(255);
			int i = GetPrivateProfileString(Section, Key, "", temp, 255, iniPath);
			value = temp.ToString();

			return value != "";
		}

		/// <summary>
		/// INI파일의 값을 읽어 옵니다. 한글 안됨
		/// </summary>
		/// <param name="Section">대분류</param>
		/// <param name="Key">소분류</param>
		/// <returns></returns>
		public bool GetFloat(String Section, String Key, out float value)
		{
			return float.TryParse(GetString(Section, Key), out value);
		}
		/// <summary>
		/// INI파일의 값을 읽어 옵니다. 한글 안됨
		/// </summary>
		/// <param name="Section">대분류</param>
		/// <param name="Key">소분류</param>
		/// <returns></returns>
		public bool GetInt(String Section, String Key, out int value)
		{
			return int.TryParse(GetString(Section, Key), out value);
		}
		/// <summary>
		/// INI파일의 값을 저장 합니다. 한글 안됨
		/// </summary>
		/// <param name="Section">대분류</param>
		/// <param name="Key">소분류</param>
		/// <param name="Value">소분류의 값</param>
		/// <param name="IniPath">INI파일 저장할 경로</param>
		public void SetValue(String Section, String Key, String Value, String IniPath)
		{
			this.iniPath = IniPath;
			WritePrivateProfileString(Section, Key, Value, IniPath);
		}
		/// <summary>
		/// INI파일의 값을 저장 합니다. 한글 안됨
		/// </summary>
		/// <param name="Section">대분류</param>
		/// <param name="Key">소분류</param>
		/// <param name="Value">소분류의 값</param>
		public void SetValue(String Section, String Key, String Value)
        {
            WritePrivateProfileString(Section, Key, Value, iniPath);
        }
		/// <summary>
		/// INI파일의 값을 저장 합니다. 한글 안됨
		/// </summary>
		/// <param name="Section">대분류</param>
		/// <param name="Key">소분류</param>
		/// <param name="Value">소분류의 값</param>
		public void SetValue(String Section, String Key, int Value)
        {
            WritePrivateProfileString(Section, Key, Value.ToString(), iniPath);
        }
		/// <summary>
		/// INI파일의 값을 저장 합니다. 한글 안됨
		/// </summary>
		/// <param name="Section">대분류</param>
		/// <param name="Key">소분류</param>
		/// <param name="Value">소분류의 값</param>
		public void SetValue(String Section, String Key, float Value)
        {
            WritePrivateProfileString(Section, Key, Value.ToString(), iniPath);
        }
    }
}

