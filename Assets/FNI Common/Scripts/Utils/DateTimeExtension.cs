/// 작성자: 조효련
/// 작성일: 2022-01-27
/// 수정일: 
/// 저작권: Copyright(C) FNIKorea Co., Ltd.. (주)에프앤아이코리아

using FNI.Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;


namespace FNI.Common.Utils
{
    public static class DateTimeExtension
    {
        /// <summary> 내가속한 달의 첫번쨰일 ex)2019-03-07의 반환값은 2019-03-01 </summary>
        public static DateTime FirstDateOfMonth(this DateTime dateTime)
        {
            return dateTime.AddDays(1 - dateTime.Day);
        }
        /// <summary> 내가속한 주의 첫번째 일 이때 주의 첫번째 요일을 정해야한다. ex)2019-03-07의 반환
        public static DateTime FirstDateOfWeek(this DateTime dateTime, DayOfWeek ruleDayOfWeek = DayOfWeek.Monday)
        {
            return dateTime.AddDays(-((dateTime.DayOfWeek + 7 - ruleDayOfWeek) % 7));
        }
        /// <summary> 내가속한 주의 첫번째 일 이때 주의 첫번째 요일을 정해야한다. ex)2019-03-07의 반환
        public static DateTime LastDateOfWeek(this DateTime dateTime, DayOfWeek ruleDayOfWeek = DayOfWeek.Monday)
        {
            return dateTime.FirstDateOfWeek(ruleDayOfWeek).AddDays(6);
        }
    }

    public class CalendarHelper
    {
        public class CalendarDayInfo
        {
            /// <summary>
            /// 1일 이전의 빈 공간은 NULL
            /// </summary>
            public int? Day { get; set; }
            public DayOfWeek DayOfWeek { get; set; }
        }

        public class CalendarInfo
        {
            public int Month { get; set; }
            /// <summary>
            /// 달력 1일 이전의 빈 공간 데이터까지 가지고 있음
            /// </summary>
            public List<CalendarDayInfo> DayListUI = new List<CalendarDayInfo>();
            /// <summary>
            /// 기본 숫자(일)만 표현
            /// </summary>
            public List<CalendarDayInfo> DayList = new List<CalendarDayInfo>();
        }

        static int getdays(int year, int month)
        {
            int days = CultureInfo.CurrentCulture.
            Calendar.GetDaysInMonth(year, month);

            return days;
        }

        public static DateTime GetTodayDate()
        {
            return DateTime.Today;
        }

        public static DateTime GetFirstDay(int addMonth)
        {
            DateTime date = DateTime.Today.AddMonths(addMonth);
            return date.AddDays(1 - date.Day);
        }

        public static DateTime GetLastDay(int addMonth)
        {
            DateTime date = DateTime.Today.AddMonths(addMonth);
            return date.AddMonths(1).AddDays(0 - date.Day);
        }

        public class Day
        {
            public DateTime Date;
            public bool Enabled;
            public bool IsTargetMonth;
            public bool IsToday;
        }

        public static int GetDays(DayOfWeek day)
        {
            switch (day)
            {
                case DayOfWeek.Monday: return 1;
                case DayOfWeek.Tuesday: return 2;
                case DayOfWeek.Wednesday: return 3;
                case DayOfWeek.Thursday: return 4;
                case DayOfWeek.Friday: return 5;
                case DayOfWeek.Saturday: return 6;
                case DayOfWeek.Sunday: return 0;
            }

            return 0;
        }


        public static List<Day> BuildCalendar(DateTime targetDate)
        {
            CultureInfo myCI = new CultureInfo("ko-KO");

            List<Day> Days = new List<Day>();

            // 현재 선택된 년 월
            DateTime CurrentMonth = new DateTime(targetDate.Year, targetDate.Month, 1);

            int offset = DayOfWeekNumber(CurrentMonth.DayOfWeek);
            if (offset != 1)
                CurrentMonth = CurrentMonth.AddDays(-offset);

            // 6주 각 7일 = 42
            for (int box = 1; box <= 42; box++)
            {
                Day day = new Day { Date = CurrentMonth, Enabled = true, IsTargetMonth = targetDate.Month == CurrentMonth.Month };
                day.IsToday = CurrentMonth == DateTime.Today;
                Days.Add(day);

                CurrentMonth = CurrentMonth.AddDays(1);
            }

            return Days;
        }

        private static int DayOfWeekNumber(DayOfWeek dow)
        {
            return Convert.ToInt32(dow.ToString("D"));
        }

        public static CalendarInfo GetDaysOfMonth(int addMonth)
        {
            DateTime first = GetFirstDay(addMonth);
            var firstDayInfo = new CalendarDayInfo() { Day = first.Day, DayOfWeek = first.DayOfWeek };

            DateTime last = GetLastDay(addMonth);
            var lastDayInfo = new CalendarDayInfo() { Day = last.Day, DayOfWeek = last.DayOfWeek };

            var intDayOfWeek = (int)firstDayInfo.DayOfWeek;

            List<CalendarDayInfo> dayListUI = new List<CalendarDayInfo>();
            // 달력 1일 이전 공백
            for (int i = 0; i < intDayOfWeek; i++)
            {
                dayListUI.Add(new CalendarDayInfo() { DayOfWeek = (DayOfWeek)i });
            }
            // first day ~ last day
            for (int i = 0; i < last.Day; i++)
            {
                var date = first.AddDays(i);
                dayListUI.Add(new CalendarDayInfo() { Day = date.Day, DayOfWeek = date.DayOfWeek });
            }

            List<CalendarDayInfo> dayList = new List<CalendarDayInfo>();
            // first day ~ last day
            for (int i = 0; i < last.Day; i++)
            {
                var date = first.AddDays(i);
                dayList.Add(new CalendarDayInfo() { Day = date.Day, DayOfWeek = date.DayOfWeek });
            }

            CalendarInfo calendarInfo = new CalendarInfo();
            calendarInfo.Month = first.Month;
            calendarInfo.DayListUI = dayListUI;
            calendarInfo.DayList = dayList;

            return calendarInfo;
        }

        private static int GetWeekNum(string date, DayOfWeek ruleDayOfWeek = DayOfWeek.Monday)
        {
            DateTime dateTime = DateTime.Parse(date);//내 일자를 정한다.

            //ruleDayOfWeek를 기반으로 지정일자가 속한 week의 마지막 요일의 날짜를 구한다.
            DateTime lastDateOfWeek = dateTime.LastDateOfWeek(ruleDayOfWeek);

            //지정Date의 마지막요일이 다음달로 넘어간다면 무조건 1주차
            if (lastDateOfWeek.FirstDateOfMonth() > dateTime.FirstDateOfMonth())
            {
                return 1;
            }

            //그이외에는 지정Date의 마지막요일의 일자 - 이번달의 마지막요일의 일자 / 7 + 1
            return (dateTime.LastDateOfWeek().Day - dateTime.FirstDateOfMonth().LastDateOfWeek().Day) / 7 + 1;
        }

    }
}