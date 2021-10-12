using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace DumpApplic
{
    public class Patterns
    {
        string tapePattern = "^RS\\d{4}L[1-9]{1}$";
        string dailyTapePattern = "^DAILY\\sTAPE\\s[1-7]{1}$";
        string weeklyTapePattern = "^WEEK\\s[1-4]{1}\\s[A-Z]{2,}\\sTAPE\\s[1-9]{1,2}$";
        string monthlyTapePattern = "^[A-Z]{3,9}\\sMONTHEND\\s[A-Z]{2,}\\sTAPE\\s[1-9]{1,2}$";
        string yearlyTapePattern = "^YEARLY\\s[A-Z]{2,}\\sTAPE\\s[1-9]{1,2}$";
        string usernamePattern = "[a-z]{5}$";
        string systemPattern = "[A-Z]{2,}$";

        public bool matchTape(string text)
        {
            MatchCollection mc = Regex.Matches(text, tapePattern);
            if (mc.Count > 0)
                return true;
            else
                return false;
        }
        public bool matchDailyTape(string text)
        {
            MatchCollection mc = Regex.Matches(text, dailyTapePattern);
            if (mc.Count > 0)
                return true;
            else
                return false;
        }
        public bool matchWeeklyTape(string text)
        {
            MatchCollection mc = Regex.Matches(text, weeklyTapePattern);
            if (mc.Count > 0)
                return true;
            else
                return false;
        }
        public bool matchMonthlyTape(string text)
        {
            MatchCollection mc = Regex.Matches(text, monthlyTapePattern);
            if (mc.Count > 0)
                return true;
            else
                return false;
        }
        public bool matchUsername(string text)
        {
            MatchCollection mc = Regex.Matches(text, usernamePattern);
            if (mc.Count > 0)
                return true;
            else
                return false;
        }
        public bool matchSystem(string text)
        {
            MatchCollection mc = Regex.Matches(text, systemPattern);
            if (mc.Count > 0)
                return true;
            else
                return false;
        }
        public bool matchYearlyTape(string text)
        {
            MatchCollection mc = Regex.Matches(text, yearlyTapePattern);
            if (mc.Count > 0)
                return true;
            else
                return false;
        }

    }

    
}
