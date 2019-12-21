using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Extensions {
    public static class Extensions {
        public static string ToJson<TKey, TValue>(this Dictionary<TKey, TValue> dic) {
            return JsonConvert.SerializeObject(dic, Formatting.None);
        }
        public static bool IsEmpty(this string str) {
            return string.IsNullOrWhiteSpace(str);
        }
        public static bool IsNotEmpty(this string str) {
            return !str.IsEmpty();
        }
        /// вытаскивает из строки только числа и соединяет их в новую строку
        public static string ToNumericString(this string inputString) {
            return string.Join("", inputString.ToCharArray().Where(char.IsDigit));
        }
        /// создает числовую строку и добавляет + вначале, если она соответствует 
        /// длине для заданного типа телефонных номеров. Например для России это 11 
        /// чисел
        public static string ToPhoneNumber(this string str, int phoneLength = 11) {
            str = str.ToNumericString();
            if (str.Length != phoneLength) return string.Empty;
            return $"+{str}";
        }
        public static Dictionary<TKey, TValue> RemoveNullValues<TKey, TValue>(this Dictionary<TKey, TValue> dic) {
            foreach (var item in dic.Where(item => Equals(item.Value, default(TValue))).ToList()) {
                dic.Remove(item.Key);
            }
            return dic;
        } 
        public static string[] SplitToArray(this string inputString, char delimiter = ',') {
            return inputString.Split(delimiter)
                .Select(val => val.Trim())
                .Where(val => !val.IsEmpty()).ToArray();
        }
        /// создает ENUM из указанной строки, если это возможно и возвращет 
        /// либо значение соответствующее строки, либо дефолтное
        public static T ToEnum<T>(this string str) where T : struct, IConvertible, IComparable, IFormattable  {
            if (!typeof(T).IsEnum) {
                throw new ArgumentException("T must be enum");
            }

            T output = default(T);
            Enum.TryParse(str, out output);
            return output;
        }
    }
}