﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace VueNote.Core.Util
{
    public static class CryptoHelper
    {
        /* =======================
         * 加密算法，参照代码：https://github.com/henkmollema/CryptoHelper/blob/master/src/CryptoHelper/Crypto.cs
         * =======================
         */

        private static readonly int PBKDF2IterCount = 10000;
        private static readonly int PBKDF2SubkeyLength = 256 / 8; // 256 bits
        private static readonly int SaltSize = 128 / 8; // 128 bits
        private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

        /// <summary>
        /// Returns a hashed representation of the specified <paramref name="password"/>.
        /// </summary>
        public static string HashPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            // Produce a version 3 (see comment above) text hash.
            var salt = new byte[SaltSize];
            _rng.GetBytes(salt);
            var subkey = KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, PBKDF2IterCount, PBKDF2SubkeyLength);

            var outputBytes = new byte[13 + salt.Length + subkey.Length];

            // Write format marker.
            outputBytes[0] = 0x01;

            // Write hashing algorithm version.
            WriteNetworkByteOrder(outputBytes, 1, (uint)KeyDerivationPrf.HMACSHA256);

            // Write iteration count of the algorithm.
            WriteNetworkByteOrder(outputBytes, 5, (uint)PBKDF2IterCount);

            // Write size of the salt.
            WriteNetworkByteOrder(outputBytes, 9, (uint)SaltSize);

            // Write the salt.
            Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);

            // Write the subkey.
            Buffer.BlockCopy(subkey, 0, outputBytes, 13 + SaltSize, subkey.Length);

            string hashedPassword = Convert.ToBase64String(outputBytes);
            return hashedPassword;
        }

        /// <summary>
        /// Determines whether the specified RFC 2898 hash and password are a cryptographic match.
        /// </summary>
        /// <param name="plainPassword">The plaintext password to cryptographically compare with hashedPassword.</param>
        /// <param name="hashedPassword">The previously-computed RFC 2898 hash value as a base-64-encoded string.</param>
        /// <returns>true if the hash value is a cryptographic match for the password; otherwise, false.</returns>
        /// <remarks>
        /// <paramref name="hashedPassword" /> must be of the format of HashPassword (salt + Hash(salt+input).
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="hashedPassword" /> or <paramref name="plainPassword" /> is null.
        /// </exception>
        public static bool VerifyPassword(string plainPassword, string hashedPassword)
        {
            if (hashedPassword == null)
            {
                throw new ArgumentNullException(nameof(hashedPassword));
            }
            if (plainPassword == null)
            {
                throw new ArgumentNullException(nameof(plainPassword));
            }

            var decodedHashedPassword = Convert.FromBase64String(hashedPassword);

            if (decodedHashedPassword.Length == 0)
            {
                return false;
            }

            try
            {
                // Verify hashing format.
                if (decodedHashedPassword[0] != 0x01)
                {
                    // Unknown format header.
                    return false;
                }

                // Read hashing algorithm version.
                var prf = (KeyDerivationPrf)ReadNetworkByteOrder(decodedHashedPassword, 1);

                // Read iteration count of the algorithm.
                var iterCount = (int)ReadNetworkByteOrder(decodedHashedPassword, 5);

                // Read size of the salt.
                var saltLength = (int)ReadNetworkByteOrder(decodedHashedPassword, 9);

                // Verify the salt size: >= 128 bits.
                if (saltLength < 128 / 8)
                {
                    return false;
                }

                // Read the salt.
                var salt = new byte[saltLength];
                Buffer.BlockCopy(decodedHashedPassword, 13, salt, 0, salt.Length);

                // Verify the subkey length >= 128 bits.
                var subkeyLength = decodedHashedPassword.Length - 13 - salt.Length;
                if (subkeyLength < 128 / 8)
                {
                    return false;
                }

                // Read the subkey.
                var expectedSubkey = new byte[subkeyLength];
                Buffer.BlockCopy(decodedHashedPassword, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

                // Hash the given password and verify it against the expected subkey.
                var actualSubkey = KeyDerivation.Pbkdf2(plainPassword, salt, prf, iterCount, subkeyLength);
                return ByteArraysEqual(actualSubkey, expectedSubkey);
            }
            catch
            {
                // This should never occur except in the case of a malformed payload, where
                // we might go off the end of the array. Regardless, a malformed payload
                // implies verification failed.
                return false;
            }
        }

        private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
        {
            return ((uint)(buffer[offset + 0]) << 24)
                | ((uint)(buffer[offset + 1]) << 16)
                | ((uint)(buffer[offset + 2]) << 8)
                | ((uint)(buffer[offset + 3]));
        }

        private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
        {
            buffer[offset + 0] = (byte)(value >> 24);
            buffer[offset + 1] = (byte)(value >> 16);
            buffer[offset + 2] = (byte)(value >> 8);
            buffer[offset + 3] = (byte)(value >> 0);
        }

        // Compares two byte arrays for equality.
        // The method is specifically written so that the loop is not optimized.
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static bool ByteArraysEqual(byte[] a, byte[] b)
        {
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            if (a == null || b == null || a.Length != b.Length)
            {
                return false;
            }

            var areSame = true;
            for (var i = 0; i < a.Length; i++)
            {
                areSame &= (a[i] == b[i]);
            }
            return areSame;
        }
    }
}
