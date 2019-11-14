//--------------------------------------------------------------------------------
//	MOG_Time.h
//	
//	
//--------------------------------------------------------------------------------

#pragma once

#include "MOG_Define.h"

namespace MOG {
	namespace TIME {

	public __gc class MOG_Time
	{
	private:
		String *GetValue(String *token);
		bool mValid;

	public:
		ArrayList *MonthNames;
		ArrayList *DayNames;
		int mYear;
		int mMonth;
		int mDayOfWeek;
		int mDay;
		int mHour;
		int mMinute;
		int mSecond;
		int mMilliseconds;
		int mTicks;

		MOG_Time();
		MOG_Time(String *timestamp);
		MOG_Time(DateTime timestamp);
		String *FormatString(String *options);

		static bool IsValidTimeStamp(String *timeStamp);
		static String *GetVersionTimestamp(void);
		static String *GetVersionTimestamp(DateTime dateTime);
		static DateTime GetDateTimeFromTimeStamp(String *timestamp);
		static String *FormatTimestamp(String *timeStamp, String *format);

		bool SetTimeStamp(String *timeStamp);
		DateTime ToDateTime(void);
		void FromDateTime(DateTime time);
		void UpdateTime(void);
		int Compare(const MOG_Time *right); // Returns -1 for less than, 0 for equal, and 1 for greater than

		void AddDay(int days);
		void SubtractDay(int days);
		void AddHours(double hours);
		void AddSeconds(double seconds);
		void AddMilliSeconds(double seconds);
		void AddYears(int years);
		void SubtractHours(double hours);
		void SubtractYears(int years);
		void SubtractSeconds(double seconds);
		void SubtractMilliSeconds(double seconds);
		
	};

	}
}

using namespace MOG::TIME;
