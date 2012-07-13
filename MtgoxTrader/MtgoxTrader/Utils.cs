//****************************************************************************             
//
// @File: Utils.cs
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
using System.IO;
using System.Runtime.Serialization;
using System.Security.Principal;
using MtGoxTrader.Model;

namespace MtGoxTrader.Trader
{
    public class Utils
    {
        /// <summary>
        /// Helper method to get error message including inner exception
        /// </summary>
        /// <param name="exception">The exception object</param>
        /// <returns>The detail message</returns>
        public static string GetDetailedException(Exception exception)
        {
            var message = new StringBuilder();

            while (exception != null)
            {
                message.AppendLine(exception.Message ?? string.Empty);
                exception = exception.InnerException;
            }

            return message.ToString();
        }

        /// <summary>
        /// Helper method to get stack trace including inner exception
        /// </summary>
        /// <param name="exception">The exception object</param>
        /// <returns>The stack trace</returns>
        public static string GetStackTrace(Exception exception)
        {
            var stackTrace = new StringBuilder();

            while (exception != null)
            {
                stackTrace.AppendLine(exception.StackTrace ?? string.Empty);
                exception = exception.InnerException;
            }

            return stackTrace.ToString();
        }

        public static void SaveToFile(object t, string fileName)
        {
            try
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    DataContractSerializer serializer = new DataContractSerializer(t.GetType());
                    //foreach (StopOrder order in orderList)
                    {
                        serializer.WriteObject(memStream, t);
                    }

                    // Encrypt the memory stream.
                    byte[] encryptedData = Encryption.EncryptData(memStream.ToArray());

                    // Write encrypted credentials to the file and ACL the file simultaneously.
                    using (FileStream stream =
                        new FileStream(fileName, FileMode.Create))
                    {
                        stream.Write(encryptedData, 0, encryptedData.Length);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static object LoadFromFile(string fileName, Type t)
        {
            object o = null;
            try
            {
                byte[] fileContents = File.ReadAllBytes(fileName);

                byte[] decryptedData = Encryption.DecryptData(fileContents);

                using (MemoryStream stream = new MemoryStream())
                {
                    stream.Write(decryptedData, 0, decryptedData.Length);

                    stream.Position = 0;

                    DataContractSerializer serializer = new DataContractSerializer(t);

                    o = serializer.ReadObject(stream);
                }
            }
            catch (Exception ex)
            {
            }
            return o;
        }
    }
}
