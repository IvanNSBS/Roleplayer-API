﻿using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Core.Meta
{
    public class MetaUtils
    {
        /// <summary>
        /// Checks if a class has all it's members equals to default. Useful to check if a meta entry was properly
        /// set up 
        /// </summary>
        /// <param name="data">Data to check if it's not default</param>
        /// <typeparam name="T">Type of data</typeparam>
        /// <returns>True if all data values are equal to default, false otherwise</returns>
        public static bool MembersAreAllDefault<T>(T data) where T : new()
        {
            string serialized = JsonConvert.SerializeObject(data);
            string df = JsonConvert.SerializeObject(new T());

            return String.Equals(serialized, df);
        }

        /// <summary>
        /// Utility function to check if every entry in a dictionary A is properly linked to a dictionary B.
        /// A getter function is used to get which A field should represent Dictionary B entry 
        /// </summary>
        /// <param name="a">The first dictionary</param>
        /// <param name="b">The dictionary to check the entries of the first</param>
        /// <param name="getter">Functions </param>
        /// <param name="errorMessage">Function to get an error message. First argument is dict the key that had errors</param>
        /// <typeparam name="T1">Data Type of first MetaData</typeparam>
        /// <typeparam name="T2">Data Type of second MetaData</typeparam>
        /// <returns>True if there was no mismatch, false otherwise</returns>
        public static bool CheckMetaDictLinking<T1, T2>(
            IReadOnlyDictionary<string, T1> a, IReadOnlyDictionary<string, T2> b, 
            Func<T1, string> getter, Func<string, string> errorMessage = null)
        {
            bool isLinkingValid = true;
            
            foreach (var pair in a)
            {
                bool val = b.ContainsKey(getter(pair.Value));
                isLinkingValid &= val;

                if(!val && errorMessage != null)
                    Debug.Log(errorMessage(pair.Key));
            }
            
            return isLinkingValid;
        }
        
        /// <summary>
        /// Utility function to check if every entry in a dictionary A is properly linked to a dictionary B.
        /// A getter function is used to get which A field should represent Dictionary B entry.
        /// A Validity Checker function is used to see if the linked value found is valid.  
        /// </summary>
        /// <param name="a">The first dictionary</param>
        /// <param name="b">The dictionary to check the entries of the first</param>
        /// <param name="getter">Functions </param>
        /// <param name="isValid">Function that checks if the value found for A and B is valid</param>
        /// <param name="errorMessage">Function to get an error message. First argument is dict the key that had errors</param>
        /// <typeparam name="T1">Data Type of first MetaData</typeparam>
        /// <typeparam name="T2">Data Type of second MetaData</typeparam>
        /// <returns>True if there was no mismatch or invalid link, false otherwise</returns>
        public static bool CheckMetaDictLinking<T1, T2>(
            IReadOnlyDictionary<string, T1> a, IReadOnlyDictionary<string, T2> b, 
            Func<T1, string> getter, Func<T1, T2, bool> isValid, 
            Func<string, string> errorMessage = null)
        {
            bool isLinkingValid = true;
            
            foreach (var pair in a)
            {
                bool val = b.ContainsKey(getter(pair.Value)) && isValid(pair.Value, b[getter(pair.Value)]);
                isLinkingValid &= val;
                
                if(!val && errorMessage != null)
                    Debug.Log(errorMessage(pair.Key));
            }
            
            return isLinkingValid;
        }
        
        /// <summary>
        /// Checks if every entry in a list passes a condition check.
        /// Used to see if a data present in a meta file has been properly loaded or setup
        /// </summary>
        /// <param name="data">List to check validity</param>
        /// <param name="checkValid">Function that will perform the check</param>
        /// <param name="errorMsg">Optional Function to print an error when an entry is invalid</param>
        /// <typeparam name="T1">Type of the data</typeparam>
        /// <returns>True if all entries pass the test, false otherwise</returns>
        public static bool AreAllMetaValuesValid<T1>(IReadOnlyDictionary<string, T1> data, Func<T1, bool> checkValid, 
            Func<string, string> errorMsg = null)
        {
            bool allValid = true;
            foreach (var value in data)
            {
                bool valid = checkValid(value.Value);
                allValid &= valid;
                if(!valid && errorMsg != null) Debug.Log(errorMsg(value.Key));
            }

            return allValid;
        }
    }
}