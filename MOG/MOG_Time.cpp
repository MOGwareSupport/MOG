//--------------------------------------------------------------------------------
//	MOG_DosUtils.h
//	
//	
//--------------------------------------------------------------------------------

#include "StdAfx.h"

#include "MOG_Time.h"
//#include "MOG_StringUtils.h"
#include "MOG_Tokens.h"


// *********************************************************
// *********************************************************
MOG_Time::MOG_Time()
{
	MonthNames = new ArrayList;
	DayNames = new ArrayList;
	mValid = false;

	MonthNames->Add(S"");
	MonthNames->Add(S"January");
	MonthNames->Add(S"February");
	MonthNames->Add(S"March");
	MonthNames->Add(S"April");
	MonthNames->Add(S"May");
	MonthNames->Add(S"June");
	MonthNames->Add(S"July");
	MonthNames->Add(S"August");
	MonthNames->Add(S"September");
	MonthNames->Add(S"October");
	MonthNames->Add(S"November");
	MonthNames->Add(S"December");

	DayNames->Add(S"Sunday");
	DayNames->Add(S"Monday");
	DayNames->Add(S"Tuesday");
	DayNames->Add(S"Wednesday");
	DayNames->Add(S"Thursday");
	DayNames->Add(S"Friday");
	DayNames->Add(S"Saturday");

	mDay = 0;
	mDayOfWeek = 0;
	mHour = 0;
	mMilliseconds = 0;
	mMinute = 0;
	mMonth = 0;
	mSecond = 0;
	mYear = 0;
	mTicks = 0;

	UpdateTime();
}

MOG_Time::MOG_Time(String *timestamp)
{
	MonthNames = new ArrayList;
	DayNames = new ArrayList;
	mValid = false;

	MonthNames->Add(S"");
	MonthNames->Add(S"January");
	MonthNames->Add(S"February");
	MonthNames->Add(S"March");
	MonthNames->Add(S"April");
	MonthNames->Add(S"May");
	MonthNames->Add(S"June");
	MonthNames->Add(S"July");
	MonthNames->Add(S"August");
	MonthNames->Add(S"September");
	MonthNames->Add(S"October");
	MonthNames->Add(S"November");
	MonthNames->Add(S"December");

	DayNames->Add(S"Sunday");
	DayNames->Add(S"Monday");
	DayNames->Add(S"Tuesday");
	DayNames->Add(S"Wednesday");
	DayNames->Add(S"Thursday");
	DayNames->Add(S"Friday");
	DayNames->Add(S"Saturday");

	mDay = 0;
	mDayOfWeek = 0;
	mHour = 0;
	mMilliseconds = 0;
	mMinute = 0;
	mMonth = 0;
	mSecond = 0;
	mYear = 0;
	mTicks = 0;

	if (timestamp->Length == 15 ||
		timestamp->Length == 12)
	{
		SetTimeStamp(timestamp);
	}
}

MOG_Time::MOG_Time(DateTime time)
{
	MonthNames = new ArrayList;
	DayNames = new ArrayList;
	mValid = false;

	MonthNames->Add(S"");
	MonthNames->Add(S"January");
	MonthNames->Add(S"February");
	MonthNames->Add(S"March");
	MonthNames->Add(S"April");
	MonthNames->Add(S"May");
	MonthNames->Add(S"June");
	MonthNames->Add(S"July");
	MonthNames->Add(S"August");
	MonthNames->Add(S"September");
	MonthNames->Add(S"October");
	MonthNames->Add(S"November");
	MonthNames->Add(S"December");

	DayNames->Add(S"Sunday");
	DayNames->Add(S"Monday");
	DayNames->Add(S"Tuesday");
	DayNames->Add(S"Wednesday");
	DayNames->Add(S"Thursday");
	DayNames->Add(S"Friday");
	DayNames->Add(S"Saturday");

	mDay = 0;
	mDayOfWeek = 0;
	mHour = 0;
	mMilliseconds = 0;
	mMinute = 0;
	mMonth = 0;
	mSecond = 0;
	mYear = 0;
	mTicks = 0;

	FromDateTime(time);
}

String *MOG_Time::FormatTimestamp(String *timeStamp, String *format)
{
	MOG_Time *time = new MOG_Time(timeStamp);
	return time->FormatString(format);
}


// *********************************************************
// Valid tokens that can be used with MOG_Time::FormatString()
// Any token with a '.' in it's name indicates an optional descriptor
//	{Year.2}		= 02		TOKEN_Year_2		
//	{Year.4}		= 2002		TOKEN_Year_4		
//	{month}			= january	TOKEN_month			
//	{Month}			= January	TOKEN_Month			
//	{MONTH}			= JANUARY	TOKEN_MONTH			
//	{month.1}		= 1			TOKEN_Month_1		
//	{month.2}		= 01		TOKEN_Month_2		
//	{month.3}		= jan		TOKEN_month_3		
//	{Month.3}		= Jan		TOKEN_Month_3		
//	{MONTH.3}		= JAN		TOKEN_MONTH_3		
//	{day}			= monday	TOKEN_day			
//	{Day}			= Monday	TOKEN_Day			
//	{DAY}			= MONDAY	TOKEN_DAY			
//	{day.1}			= 6			TOKEN_Day_1 		
//	{day.2}			= 06		TOKEN_Day_2 		
//	{day.3}			= mon		TOKEN_day_3 		
//	{Day.3}			= Mon		TOKEN_Day_3 		
//	{DAY.3}			= MON		TOKEN_DAY_3 		
//	{24hour.1}		= 5			TOKEN_24Hour_1 		
//	{24hour.2}		= 05		TOKEN_24Hour_2 		
//	{hour.1}		= 5			TOKEN_Hour_1 		
//	{hour.2}		= 05		TOKEN_Hour_2 		
//	{minute.1}		= 1			TOKEN_Minute_1 		
//	{minute.2}		= 01		TOKEN_Minute_2 		
//	{second.1}		= 1			TOKEN_Second_1 		
//	{second.2}		= 01		TOKEN_Second_2 	
//	{millisecond}	= 001		TOKEN_Millisecond 			
//	{ampm}			= am		TOKEN_ampm 			
//	{AMPM}			= AM		TOKEN_AMPM 			
// *********************************************************
String *MOG_Time::FormatString(String *baseFormat)
{
	String *str = "";
	String *seeds = "";
	String *format = baseFormat;
	if (format->Length == 0)
	{
		format = String::Concat(TOKEN_Month_1, S"/", TOKEN_Day_1, S"/", TOKEN_Year_4, S" ", TOKEN_Hour_1, S":", TOKEN_Minute_2, S":", TOKEN_Second_2, S" ", TOKEN_AMPM);
	}

	// Get the system defined time tokens
	String *timeTokens = MOG_Tokens::GetTimeTokenSeeds();
	
	// Split these tokens up
	String* delimStr = S"{},";
	Char delimiter[] = delimStr->ToCharArray();		
	String *parts[] = timeTokens->Split(delimiter);

	// We are now going to loop through these tokens and make valid seeds out of them
	for (int i = 0; i < parts->Count; i++)
	{
		if (parts[i]->Length)
		{
			String *token = String::Concat(S"{", parts[i], S"}");
			String *value = GetValue(parts[i]);
			String *seed = String::Concat(token, S"=", value);

			seeds = String::Concat(seeds, S",", seed);
		}		
	}

	// Ok, return the formated string given these tokens
	return MOG_Tokens::GetFormattedString(format, seeds);
}

// Get the value of this token from our current time
String *MOG_Time::GetValue(String *token)
{
	int option = 0;
	String *str = "";

	int optionIndex = token->IndexOf(".");
	if (optionIndex != -1)
	{		
		String *number = token->Substring(optionIndex + 1, token->Length - (optionIndex+1));
		option = Convert::ToInt32(number);//option + 2));

		// Remove the option from our token
		token = token->Substring(0, optionIndex);
	}

	// Match the token
	if (token->ToLower()->Equals("year"))
	{
		// Extract the full 4-digit year
		String *year = Convert::ToString(mYear);
		if (year->Length < 4)
		{
             return "";
		}

		// 2-digit year option?
		if (option == 2)
		{
			// Extract the 2-digit year
			year = year->Substring(2);
		}
		//	Add the year
		str = String::Concat(str, year);
	}
	else if (String::Compare(token, "Month", true) == 0)
	{
		String *month = "";

		// 1-digit number option?
		if (option == 1)
		{
			month = Convert::ToString(mMonth);
			if (month->Length < 1)
			{
                return "";
			}
		}
		//	2-digit number option?
		else if (option == 2)
		{
			month = Convert::ToString(mMonth);
			if (month->Length < 1)
			{
                return "";
			}
			if (mMonth < 10)
				month = String::Concat("0", month);
		}
		//	3-digit text option?
		else if (option == 3)
		{			
			month = __try_cast<String*>(MonthNames->Item[mMonth]);
			if (month->Length < 3)
			{
                return "";
			}
			month = month->Substring(0,3);
		}
		//	Else, assume full text
		else
		{
			month = __try_cast<String*>(MonthNames->Item[mMonth]);
		}

		//	Check for lowercase option
		//glk: Changed this to String::Compare, since I couldn't get a lower or upper Month.  
		if (String::Compare(token->Substring(0, 1), S"m") == 0)
		{
			month = month->ToLower();
		}
		//	Check for uppercase option
		if (String::Compare(token->Substring(1, 1), S"O") == 0)
		{
			month = month->ToUpper();
		}

		//	Add the month
		str = String::Concat(str, month);
	}
	else if (String::Compare(token, "Day", true) == 0)
	{
		String *day = "";

		//	1-digit number option?
		if (option == 1)
		{
			day = Convert::ToString(mDay);			
		}
		//	2-digit number option?
		else if (option == 2)
		{
			day = Convert::ToString(mDay);
			if (mDay < 10)
				day = String::Concat("0",day);
		}
		//	3-digit text option?
		else if (option == 3)
		{
			day = __try_cast<String*>(DayNames->Item[mDayOfWeek]);
			if (day->Length >=3)
			{
				day = day->Substring(0,3);
			}
		}
		//	Else, assume full text
		else
		{
			day = __try_cast<String*>(DayNames->Item[mDayOfWeek]);
		}

		if (token->Length >=1)
		{
			//	Check for lowercase option
			if (String::Compare(token->Substring(0, 1), S"d") == 0)
			{
				day = day->ToLower();
			}
			//	Check for uppercase option
			if (String::Compare(token->Substring(1, 1), S"A") == 0)
			{
				day = day->ToUpper();
			}
		}

		//	Add the day
		str = String::Concat(str,day);
	}
	else if (String::Compare(token, "24Hour") == 0)
	{
		String *hour = Convert::ToString(mHour);
		
		//	2-digit option?
		if (option == 2 && mHour < 10)
		{
			hour = String::Concat("0", hour);
		}
		str = String::Concat(str, hour);
	}
	else if (String::Compare(token, "Hour") == 0)
	{
		int hour = (mHour > 12) ? mHour - 12 : mHour;
		//	2-digit option?
		if (option == 2 && hour < 10)
		{
			str = String::Concat(str, S"0", Convert::ToString(hour));
		}
		else
		{
			str = String::Concat(str, Convert::ToString(hour));
		}
	}
	else if (String::Compare(token, "Minute") == 0)
	{
		String *minute = Convert::ToString(mMinute);
		//	2-digit option?
		if (option == 2 && mMinute < 10)
		{
			minute = String::Concat("0", minute);
		}
		str = String::Concat(str, minute);
	}
	else if (String::Compare(token, "Second") == 0)
	{
		String *second = Convert::ToString(mSecond);
		//	2-digit option?
		if (option == 2 && mSecond < 10)
		{
			second = String::Concat("0", second);
		}
		str = String::Concat(str, second);
	}
	else if (String::Compare(token, "Millisecond") == 0)
	{
		String *milli = Convert::ToString(mMilliseconds);
		
		if (mMilliseconds < 10)
		{
			milli = String::Concat("00", milli);
		}
		else if (mMilliseconds < 100 )
		{
			milli = String::Concat("0", milli);
		}
		str = String::Concat(str, milli);
	}
	else if (String::Compare(token, "ampm", true) == 0)
	{
		String *ampm = (mHour >= 12) ? "pm" : "am";
		
		//	Check for uppercase option
		if (String::Compare(token->Substring(1, 1),"M") == 0)
		{
			ampm = ampm->ToUpper();
		}
		str = String::Concat(str, ampm);
	}
	
	return str;
}

// *********************************************************
void MOG_Time::UpdateTime()
{
	try
	{
		DateTime today;
		today = today.get_Now();

		mDay = today.Day;
		mHour = today.Hour;
		mDayOfWeek = today.DayOfWeek;
		mMilliseconds = today.Millisecond;
		mMinute = today.Minute;
		mMonth = today.Month;
		mSecond = today.Second;
		mYear = today.Year;
		mTicks = (int)today.Ticks;
		mValid = true;
	}
	catch(...)
	{
	}
}

// *********************************************************
void MOG_Time::AddHours(double hours)
{
	if (mValid)
	{
		DateTime today(mYear, mMonth, mDay, mHour, mMinute, mSecond, mMilliseconds);
		FromDateTime(today.AddHours(hours));
	}
}

// *********************************************************
void MOG_Time::SubtractHours(double hours)
{
	if (mValid)
	{
		DateTime today(mYear, mMonth, mDay, mHour, mMinute, mSecond, mMilliseconds);
		FromDateTime(today.AddHours(-hours));
	}
}

// *********************************************************
void MOG_Time::AddSeconds(double seconds)
{
	if (mValid)
	{
		DateTime today(mYear, mMonth, mDay, mHour, mMinute, mSecond, mMilliseconds);
		FromDateTime(today.AddSeconds(seconds));
	}
}

// *********************************************************
void MOG_Time::SubtractSeconds(double seconds)
{
	if (mValid)
	{
		DateTime today(mYear, mMonth, mDay, mHour, mMinute, mSecond, mMilliseconds);
		FromDateTime(today.AddSeconds(-seconds));
	}
}

// *********************************************************
void MOG_Time::AddMilliSeconds(double seconds)
{
	if (mValid)
	{
		DateTime today(mYear, mMonth, mDay, mHour, mMinute, mSecond, mMilliseconds);
		FromDateTime(today.AddMilliseconds(seconds));
	}
}

// *********************************************************
void MOG_Time::SubtractMilliSeconds(double seconds)
{
	if (mValid)
	{
		DateTime today(mYear, mMonth, mDay, mHour, mMinute, mSecond, mMilliseconds);
		FromDateTime(today.AddMilliseconds(-seconds));
	}
}


// *********************************************************
void MOG_Time::AddDay(int days)
{
	if (mValid)
	{		
		DateTime today(mYear, mMonth, mDay, mHour, mMinute, mSecond, mMilliseconds);
		FromDateTime(today.AddDays(days));
	}
}

// *********************************************************
void MOG_Time::SubtractDay(int days)
{
	if (mValid)
	{
		DateTime today(mYear, mMonth, mDay, mHour, mMinute, mSecond, mMilliseconds);
		FromDateTime(today.AddDays(-days));
	}
}

void MOG_Time::SubtractYears(int years)
{
	if (mValid)
	{
		mYear -= years;
	}
}

void MOG_Time::AddYears(int years)
{
	if (mValid)
	{
		mYear += years;
	}
}

// *********************************************************
int MOG_Time::Compare(const MOG_Time *right)
{
	if(mYear != right->mYear)
	{
		if(mYear < right->mYear)
			return -1;
		else
			return 1;
	}

	if(mMonth != right->mMonth)
	{
		if(mMonth < right->mMonth)
			return -1;
		else
			return 1;
	}

	if(mDay != right->mDay)
	{
		if(mDay < right->mDay)
			return -1;
		else
			return 1;
	}

	if(mHour != right->mHour)
	{
		if(mHour < right->mHour)
			return -1;
		else
			return 1;
	}

	if(mMinute != right->mMinute)
	{
		if(mMinute < right->mMinute)
			return -1;
		else
			return 1;
	}

	if(mSecond != right->mSecond)
	{
		if(mSecond < right->mSecond)
			return -1;
		else
			return 1;
	}

	if(mMilliseconds != right->mMilliseconds)
	{
		if(mMilliseconds < right->mMilliseconds)
			return -1;
		else
			return 1;
	}

	return 0;
}

// *********************************************************
String *MOG_Time::GetVersionTimestamp(void)
{
	return GetVersionTimestamp(DateTime::Now);
}

// *********************************************************
String *MOG_Time::GetVersionTimestamp(DateTime dateTime)
{
	String *format = "yyMMddHHmmssfff";
	return dateTime.ToString(format);
}

// *********************************************************
DateTime MOG_Time::GetDateTimeFromTimeStamp(String *timestamp)
{
	int year = Convert::ToInt32(timestamp->Substring(0, 2)) +  2000;
	int month = Convert::ToInt32(timestamp->Substring(2, 2));
	int day = Convert::ToInt32(timestamp->Substring(4, 2));
	int hour = Convert::ToInt32(timestamp->Substring(6, 2));
	int min = Convert::ToInt32(timestamp->Substring(8, 2));
	int sec = Convert::ToInt32(timestamp->Substring(10, 2));
	int millisec = Convert::ToInt32(timestamp->Substring(12, 3));
	return DateTime(year, month, day, hour, min, sec, millisec);
}

// *********************************************************
bool MOG_Time::IsValidTimeStamp(String *timeStamp)
{
	// Make sure that the timestamp meets the length requirement
	if (timeStamp->Length == 12 ||
		timeStamp->Length == 15)
	{
		// Make sure there are no ASCII character
		try
		{
			// Convert it to a number
			Int64 value = Convert::ToUInt64(timeStamp);
			if (value != 0)
			{
				return true;
			}
		}
		catch (Exception *e)
		{
			e->ToString();
			MOG_ASSERT_THROW(false, MOG_Exception::MOG_EXCEPTION_InvalidData, e->ToString());
		}
	}
	return false;
}


// *********************************************************
bool MOG_Time::SetTimeStamp(String *timeStamp)
{
	// Make sure that we have a qualified timestamp?
	if (timeStamp->Length == 12 ||
		timeStamp->Length == 15)
	{
		// Load the time with the timeStamp
		mYear = Convert::ToInt32(timeStamp->Substring(0,2)) + 2000;
		mMonth = Convert::ToInt32(timeStamp->Substring(2,2));
		mDay = Convert::ToInt32(timeStamp->Substring(4,2));
		mHour = Convert::ToInt32(timeStamp->Substring(6,2));
		mMinute = Convert::ToInt32(timeStamp->Substring(8,2));
		mSecond = Convert::ToInt32(timeStamp->Substring(10,2));

		// Check if this is a full timestamp?
		if (timeStamp->Length == 15)
		{
			mMilliseconds = Convert::ToInt32(timeStamp->Substring(12,3));
		}

		mDayOfWeek = (int)ToDateTime().DayOfWeek;
		mTicks = 0;
		mValid = true;
	
		return true;
	}
	else if (timeStamp->Length == 0)
	{
		mDay = 0;
		mDayOfWeek = 0;
		mHour = 0;
		mMilliseconds = 0;
		mMinute = 0;
		mMonth = 0;
		mSecond = 0;
		mYear = 0;
		mTicks = 0;

		return true;
	}

	return false;
}

// *********************************************************
DateTime MOG_Time::ToDateTime(void)
{
	if (mValid)
	{
		DateTime today(mYear, mMonth, mDay, mHour, mMinute, mSecond, mMilliseconds);

		return today;
	}

	return NULL;
}

// *********************************************************
void MOG_Time::FromDateTime(DateTime time)
{
	mDay = time.Day;
	mHour = time.Hour;
	mDayOfWeek = time.DayOfWeek;
	mMilliseconds = time.Millisecond;
	mMinute = time.Minute;
	mMonth = time.Month;
	mSecond = time.Second;
	mYear = time.Year;
	mTicks = (int)time.Ticks;
}

