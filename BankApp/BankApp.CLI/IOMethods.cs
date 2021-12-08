﻿using System;
using System.Collections.Generic;

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
        public static string GetLowerCaseWithoutSpaces(string message)
        {
            return GetLowerCase(message).Replace(" ", "");
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
            Console.WriteLine("Address Details");
            string ColonyName = GetString("Colony Name : ");
            int StreetNo = GetInteger("Street No : ");
            string HouseNo = GetString("House No : ");
            string DistrictName = GetString("District Name : ");
            string StateName = GetString("State Name : ");
            return $"{ColonyName}\nStreet No:{StreetNo}\n{HouseNo}\n{DistrictName}\n{StateName}";
        }
    }
}