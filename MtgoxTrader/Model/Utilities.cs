//****************************************************************************             
//
// @File: Utilities.cs
// @owner: iamapi 
//    
// Notes:
//	
// @EndHeader@
//****************************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Security.Authentication;


namespace MtGoxTrader.Model
{   
    public class MtGoxNetTools
    {
        string UserAgent = "TAYPE International/0.1 MtGox API Client"; 

        /// <summary>
        /// Perform an authenticated post to MtGox client API
        /// </summary>        
        public string DoAuthenticatedAPIPost(string url, string apiKey, string apiSecret, string moreargs = null)
        {
            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(apiSecret))
                throw new ArgumentException("Cannot call private api's without api key and secret");
            var parameters = "nonce=" + DateTime.Now.Ticks.ToString();
            if (!string.IsNullOrEmpty(moreargs))
                parameters += "&" + moreargs;
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            webRequest.UserAgent = UserAgent;
            webRequest.Accept = "application/json";
            webRequest.Headers["Rest-Key"] = apiKey;
            webRequest.Headers["Rest-Sign"] = EncodeParamsToSecret(parameters, apiSecret);
            byte[] byteArray = Encoding.UTF8.GetBytes(parameters);
            webRequest.ContentLength = byteArray.Length;
            using (Stream dataStream = webRequest.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                using (Stream str = webResponse.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(str))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
        }

        /// <summary>
        /// Get HMACSHA512 hash of specified post parameters using the apiSecret as the key
        /// </summary>       
        protected string EncodeParamsToSecret(string parameters, string apiSecret)
        {
            var hmacsha512 = new HMACSHA512(Convert.FromBase64String(apiSecret));
            var byteArray = hmacsha512.ComputeHash(Encoding.UTF8.GetBytes(parameters));
            return Convert.ToBase64String(byteArray);
        }

    }

    /// <summary>
    /// Utility class to parse JSON and return .NET objects    
    /// </summary>
    sealed class DynamicJsonConverter : JavaScriptConverter
    {
        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            if (dictionary == null)
                throw new ArgumentNullException("dictionary");

            return type == typeof(object) ? new DynamicJsonObject(dictionary) : null;
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Type> SupportedTypes
        {
            get { return new ReadOnlyCollection<Type>(new List<Type>(new[] { typeof(object) })); }
        }

        private sealed class DynamicJsonObject : DynamicObject
        {
            private readonly IDictionary<string, object> _dictionary;

            public DynamicJsonObject(IDictionary<string, object> dictionary)
            {
                if (dictionary == null)
                    throw new ArgumentNullException("dictionary");
                _dictionary = dictionary;
            }

            public override string ToString()
            {
                var sb = new StringBuilder("{");
                ToString(sb);
                return sb.ToString();
            }

            private void ToString(StringBuilder sb)
            {
                var firstInDictionary = true;
                foreach (var pair in _dictionary)
                {
                    if (!firstInDictionary)
                        sb.Append(",");
                    firstInDictionary = false;
                    var value = pair.Value;
                    var name = pair.Key;
                    if (value is string)
                    {
                        sb.AppendFormat("{0}:\"{1}\"", name, value);
                    }
                    else if (value is IDictionary<string, object>)
                    {
                        new DynamicJsonObject((IDictionary<string, object>)value).ToString(sb);
                    }
                    else if (value is ArrayList)
                    {
                        sb.Append(name + ":[");
                        var firstInArray = true;
                        foreach (var arrayValue in (ArrayList)value)
                        {
                            if (!firstInArray)
                                sb.Append(",");
                            firstInArray = false;
                            if (arrayValue is IDictionary<string, object>)
                                new DynamicJsonObject((IDictionary<string, object>)arrayValue).ToString(sb);
                            else if (arrayValue is string)
                                sb.AppendFormat("\"{0}\"", arrayValue);
                            else
                                sb.AppendFormat("{0}", arrayValue);

                        }
                        sb.Append("]");
                    }
                    else
                    {
                        sb.AppendFormat("{0}:{1}", name, value);
                    }
                }
                sb.Append("}");
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                if (!_dictionary.TryGetValue(binder.Name, out result))
                {
                    result = null;
                    return true;
                }

                var dictionary = result as IDictionary<string, object>;
                if (dictionary != null)
                {
                    result = new DynamicJsonObject(dictionary);
                    return true;
                }

                var arrayList = result as ArrayList;
                if (arrayList != null && arrayList.Count > 0)
                {
                    if (arrayList[0] is IDictionary<string, object>)
                        result = new List<object>(arrayList.Cast<IDictionary<string, object>>().Select(x => new DynamicJsonObject(x)));
                    else
                        result = new List<object>(arrayList.Cast<object>());
                }

                return true;
            }
        }

    }
    
    public class MtGoxSocketIOClient
    {
        private Uri mUrl;
        private SslStream mSslStream;
        private TcpClient mClient;
        private bool mHandshakeComplete;
        private Dictionary<string, string> mHeaders;

        public MtGoxSocketIOClient(Uri url)
        {
            mUrl = url;
            string protocol = mUrl.Scheme;
            if (!protocol.Equals("ws") && !protocol.Equals("wss"))
                throw new ArgumentException("Unsupported protocol: " + protocol);
        }

        public void SetHeaders(Dictionary<string, string> headers)
        {
            mHeaders = headers;
        }

        public void Connect()
        {
            string host = mUrl.DnsSafeHost;
            string path = mUrl.PathAndQuery;
            string origin = "http://" + host;
            mSslStream = CreateSocket(mUrl);
            StringBuilder extraHeaders = new StringBuilder();
            if (mHeaders != null)
            {
                foreach (KeyValuePair<string, string> header in mHeaders)
                    extraHeaders.Append(header.Key + ": " + header.Value + "\r\n");
            }
            string request = "GET " + path + " HTTP/1.1\r\n" +
                             "Upgrade: websocket\r\n" +
                             "Connection: Upgrade\r\n" +
                             "Host: " + host + "\r\n" +
                             "Sec-WebSocket-Version: 6\r\n" +
                             "Sec-WebSocket-Origin: https://" + host + "\r\n" +
                             "Sec-WebSocket-Extensions: deflate-stream\r\n" +
                             "Sec-WebSocket-Key: HSmrc0sMlYUkAGmm5OPpG2HaGWk=\r\n" + // note see: https://en.wikipedia.org/wiki/WebSocket on draft-ietf-hybi-thewebsocketprotocol-06 version of protocol for instructions on how to construct the key
                             extraHeaders.ToString() + "\r\n";
            byte[] sendBuffer = Encoding.UTF8.GetBytes(request);
            mSslStream.Write(sendBuffer, 0, sendBuffer.Length);
            mSslStream.Flush();
            string reply = ReadMessage(mSslStream);
            if (!reply.Equals("HTTP/1.1 101 Web Socket Protocol Handshake"))
                throw new IOException("Invalid handshake response");
            if (!reply.Equals("Upgrade: WebSocket"))
                throw new IOException("Invalid handshake response");
            if (!reply.Equals("Connection: Upgrade"))
                throw new IOException("Invalid handshake response");
            
            mHandshakeComplete = true;
        }

        private string ReadMessage(SslStream sslStream)
        {           
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            do
            {
                bytes = sslStream.Read(buffer, 0, buffer.Length);

                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
                if (messageData.ToString().IndexOf("<EOF>") != -1)
                {
                    break;
                }
            } while (bytes != 0);

            return messageData.ToString();
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            return false;
        }

        public void Send(string str)
        {
            if (!mHandshakeComplete)
                throw new InvalidOperationException("Handshake not complete");

            byte[] sendBuffer = Encoding.UTF8.GetBytes(str);

            mSslStream.WriteByte(0x00);
            mSslStream.Write(sendBuffer, 0, sendBuffer.Length);
            mSslStream.WriteByte(0xff);
            mSslStream.Flush();
        }

        public string Recv()
        {
            if (!mHandshakeComplete)
                throw new InvalidOperationException("Handshake not complete");

            StringBuilder recvBuffer = new StringBuilder();

            BinaryReader reader = new BinaryReader(mSslStream);
            byte b = reader.ReadByte();
            if ((b & 0x80) == 0x80)
            {
                int len = 0;
                do
                {
                    b = (byte)(reader.ReadByte() & 0x7f);
                    len += b * 128;
                } while ((b & 0x80) != 0x80);

                for (int i = 0; i < len; i++)
                    reader.ReadByte();
            }

            while (true)
            {
                b = reader.ReadByte();
                if (b == 0xff)
                    break;

                recvBuffer.Append(b);           
            }

            return recvBuffer.ToString();
        }

        public void Close()
        {
            mSslStream.Dispose();
            mClient.Close();
            mSslStream = null;
            mClient = null;
        }

        private SslStream CreateSocket(Uri url)
        {
            string scheme = url.Scheme;
            string host = url.DnsSafeHost;

            int port = url.Port;
            this.mClient = new TcpClient(host, port);
            SslStream sslClientStream = new SslStream(mClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
            try
            {
                sslClientStream.AuthenticateAsClient(host);
            }
            catch (AuthenticationException e)
            {
                if (e.InnerException != null)
                {
                }
                mClient.Close();
                return null;
            }
            return sslClientStream;
        }
    }
}
