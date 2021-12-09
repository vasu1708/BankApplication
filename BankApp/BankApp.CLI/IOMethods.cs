﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BankApp.Services
{
    public class IOMethods
    {
        public static void DisplayOutput(List<string> history)
        {
            DisplayOutputLine("<---TRANSACTION HISTORY--->");
            if (history.Count != 0)
                DisplayOutputLine("No Transactions yet!");
            else
            {
                DisplayOutputLine($"TransactionId SenderId ReceiverId Type Amount Time Avl.Bal");
                foreach (var Txn in history)
                {
                    DisplayOutputLine(Txn);
                }
            }
        }
        public static void DisplayOutput(string message)
        {
            Console.Write(message);
        }
        public static void DisplayOutputLine(string message)
        {
            Console.WriteLine(message);
        }
        public static string GetString(string message)
        {
            DisplayOutput(message);
            string input = Console.ReadLine();
            if (input.Length != 0)
                return input;
            DisplayOutputLine("input cannot be empty!");
            return GetString(message);
        }
        public static string GetLowerCase(string message)
        {
            return GetString(message).ToLower();
        }
        public static decimal GetDecimal(string message)
        {
            if (Decimal.TryParse(GetString(message), out decimal value))
                return value;
            DisplayOutputLine("Invalid input!");
            return GetDecimal(message);
        }
        public static int GetInteger(string message)
        {
            if (int.TryParse(GetString(message), out int value))
                return value;
            DisplayOutputLine("Invalid input!");
            return GetInteger(message);
        }
        public static T GetEnum<T>(string message)
            where T : struct
        {
            if (Enum.TryParse<T>(GetString(message), out T value))
                return value;
            DisplayOutputLine("Wrong option!");
            return GetEnum<T>(message);
        }
        public static string GetAddress()
        {
            DisplayOutputLine("Address Details");
            string ColonyName = GetName("Colony Name : ");
            int StreetNo = GetInteger("Street No : ");
            int HouseNo = GetInteger("House No : ");
            string DistrictName = GetName("District Name : ");
            string StateName = GetName("State Name : ");
            return $"{ColonyName}\nStreet No:{StreetNo}\n{HouseNo}\n{DistrictName}\n{StateName}";
        }
        public static string GetMobileNumber(string message)
        {
            string mobileNumber = GetString(message);
            Regex mobileNoPattern = new Regex(@"[0-9]{10}");
            if(mobileNoPattern.IsMatch(mobileNumber))
                return mobileNumber;
            DisplayOutputLine("Proper MobileNumber must be given!");
            return GetMobileNumber(message);
            
        }
        public static string GetDOB(string message)
        {
            string dob = GetString(message);
            Regex dobpattern = new Regex(@"^\d{2}-\d{2}-\d{4}$");
            if(dobpattern.IsMatch(dob))
            {
                dob = $"{dob.Substring(6)}-{dob.Substring(3, 2)}-{dob.Substring(0, 2)}";
                return dob;
            }
                
            DisplayOutputLine("Proper Date format must be given!");
            return GetDOB(message);
        }
        public static string GetName(string message)
        {
            string name = GetString(message);
            Regex NamePattern = new Regex(@"^[a-z\sA-Z]{4,}$");
            if (NamePattern.IsMatch(name))
                return name;
            DisplayOutputLine("Proper Name must be given!");
            return GetName(message);
        }
    }
}
