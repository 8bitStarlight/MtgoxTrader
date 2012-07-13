//****************************************************************************             
//
// @File: Config.cs
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
using MtGoxTrader.MtGoxAPIClient;
using MtGoxTrader.Model;

namespace MtGoxTrader.Trader
{
    [DataContract]
    public class Config
    {
        [DataMember]
        public string Key { get; set; }
        [DataMember]
        public string Secret{ get; set; }
        [DataMember]
        public MtGoxCurrencySymbol Currency { get; set; }
    }

    public class ConfigHelper
    {
        public static void SaveToFile(Config config, string fileName)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(Config));
                serializer.WriteObject(memStream, config);

                // Encrypt the memory stream.
                byte[] encryptedData = Encryption.EncryptData(memStream.ToArray());

                using (FileStream stream =
                    new FileStream(fileName, FileMode.Create))
                {
                    stream.Write(encryptedData, 0, encryptedData.Length);
                }
            }
        }
        public static Config LoadFromFile(string fileName)
        {
            Config config = new Config();
            byte[] fileContents = File.ReadAllBytes(fileName);

            byte[] decryptedData = Encryption.DecryptData(fileContents);

            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(decryptedData, 0, decryptedData.Length);

                stream.Position = 0;

                DataContractSerializer serializer = new DataContractSerializer(typeof(Config));

                config = (Config)serializer.ReadObject(stream);
            }
            return config;
        }
    }
}
