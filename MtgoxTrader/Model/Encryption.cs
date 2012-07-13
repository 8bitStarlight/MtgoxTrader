//****************************************************************************             
//
// @File: Encryption.cs
// @owner: iamapi 
//    
// Notes:
//	
// @EndHeader@
//****************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace MtGoxTrader.Model
{
    public class Encryption
    {
        /// <summary> 
        /// Encrypt using LocalMachine profile. 
        /// </summary>
        public static string DecryptData(string encryptedData)
        {
            try
            {
                encryptedData = encryptedData.Replace('\"', ' ');
                encryptedData = encryptedData.Replace('\'', ' ');
                System.Text.Encoding enc = System.Text.Encoding.ASCII;
                byte[] decryptedData = ProtectedData.Unprotect(Convert.FromBase64String(encryptedData), null, DataProtectionScope.LocalMachine);
                return enc.GetString(decryptedData);
            }
            catch (CryptographicException e)
            {
                return null;
            }
        }

        public static byte[] DecryptData(byte[] encryptedData)
        {
            try
            {
                byte[] decryptedData = ProtectedData.Unprotect(encryptedData, null, DataProtectionScope.LocalMachine);
                return decryptedData;

            }
            catch (CryptographicException e)
            {
                return null;
            }
        }

        /// <summary> 
        /// Decrypt using LocalMachine profile. 
        /// </summary>
        public static string EncryptData(string plainData)
        {
            try
            {
                System.Text.Encoding enc = System.Text.Encoding.ASCII;
                byte[] encryptedData = EncryptData(enc.GetBytes(plainData));
                return Convert.ToBase64String(encryptedData);
            }
            catch (CryptographicException e)
            {
                return null;
            }
        }

        public static byte[] EncryptData(byte[] plainByteData)
        {

            try
            {
                byte[] encryptedData = ProtectedData.Protect(plainByteData, null, DataProtectionScope.LocalMachine);
                return encryptedData;
            }
            catch (CryptographicException e)
            {
                return null;
            }
        }

    }
}
