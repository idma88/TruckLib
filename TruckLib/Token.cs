﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TruckLib
{
    /// <summary>
    /// A string type used in Prism3D which packs up to 12 characters 
    /// of a limited character set into 8 bytes.
    /// </summary>
    public struct Token : IBinarySerializable
    {
        /// <summary>
        /// The character set.
        /// </summary>
        public static readonly char[] CharacterSet =
        { '\0', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b',
            'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o',
            'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '_'
        };

        public static readonly int MaxLength = 12;

        public ulong Value { get; set; }

        public string String
        {
            get => TokenToString(Value);
            set => Value = StringToToken(value);
        }

        /// <summary>
        /// Creates a new token.
        /// </summary>
        /// <param name="token"></param>
        public Token(ulong token)
        {
            Value = token;
        }

        /// <summary>
        /// Creates a new token from a string.
        /// </summary>
        /// <param name="str"></param>
        public Token(string str)
        {
            Value = StringToToken(str);
        }

        public override string ToString() => String;

        /// <summary>
        /// Converts a string to a token.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A token.</returns>
        public static ulong StringToToken(string input)
        {
            if (string.IsNullOrEmpty(input)) return 0;

            ulong token = 0;
            for (var i = 0; i < input.Length; i++)
            {
                token += PowUl(i) * (ulong)GetCharIndex(input.ToLower()[i]);
            }
            return token;
        }

        /// <summary>
        /// Converts a token to string.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>The input string.</returns>
        public static string TokenToString(ulong token)
        {
            // for empty tokens, return "" rather than "\0"
            if (token == 0) return "";

            // Determine length of string
            int length = 1;
            while (PowUl(length) < token)
            {
                length++;
            }

            // reverse the token, from last to first character
            var input = new char[length];
            for (var i = length; i > 0; i--)
            {
                // find the last character of the input
                // by dividing by 38^len and ignoring the remainder
                var powUl = PowUl(i - 1);
                var character = token / powUl;
                input[i - 1] += CharacterSet[character];

                // subtract that part from the token
                token -= (character * powUl);
            }

            var inputStr = new string(input);
            return inputStr;
        }

        /// <summary>
        /// Calculates Math.Pow(Letters.Length, num), except num is capped at
        /// the largest possible exponent for which the result won't overflow
        /// a ulong. So with 38 letters, the method will cap at 38^12 even
        /// if num is greater than 12.
        /// </summary>
        /// <param name="num">The exponent.</param>
        /// <returns>The result.</returns>
        static ulong PowUl(int num)
        {
            ulong res = 1;
            for (var i = 0; num > i; i++)
            {
                res *= (ulong)CharacterSet.Length;
            }
            return res;
        }

        /// <summary>
        /// Returns the index of a character.
        /// </summary>
        /// <param name="letter">The character.</param>
        /// <returns>Its index.</returns>
        static int GetCharIndex(char letter)
        {
            var index = Array.IndexOf(CharacterSet, letter);
            return index == -1 ? 0 : index;
        }

        public static bool IsValidToken(string str)
        {
            if (str.Length > MaxLength) return false;

            foreach(var c in str)
            {
                if(!CharacterSet.Contains(c)) return false;
            }
            return true;
        }

        public void ReadFromStream(BinaryReader r)
        {
            Value = r.ReadUInt64();
        }

        public void WriteToStream(BinaryWriter w)
        {
            w.Write(Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is null) return false;

            if (obj is Token token2)
            {
                return this.Value == token2.Value;
            }

            if (obj is ulong val)
            {
                return this.Value == val;
            }

            if (obj is string str)
            {
                return this.String == str;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static bool operator ==(Token token, object obj)
        {
            return token.Equals(obj);
        }

        public static bool operator !=(Token token, object obj)
        {
            return !token.Equals(obj);
        }

        public static implicit operator Token(string s)
        {
            return new Token(s);
        }
    }
}