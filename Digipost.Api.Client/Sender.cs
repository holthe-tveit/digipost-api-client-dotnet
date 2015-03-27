﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Digipost.Api.Client;

namespace Digipost.Api.Client
{

    public class Sender
    {

        private static bool isHttps = true;
        public static string DocumentGuid = Guid.NewGuid().ToString();
        public static readonly string BaseAddress = isHttps ? "https://qa2.api.digipost.no/" : "http://qa2.api.digipost.no/";

        public static async Task<string> SendAsync(string forsendelseId, string digipostAdresse, string emne)
        {
            var loggingHandler = new LoggingHandler(new HttpClientHandler());

            using (var client = new HttpClient(loggingHandler))
            {
                client.BaseAddress = new Uri(BaseAddress);

                var method = "POST";
                var uri = "messages";
                var date = DateTime.UtcNow.ToString("R");
                var xmlMessage = GetXmlMessage();
                var hash = ComputeHash(xmlMessage);
                var userId = GetUserId();
                var boundary = Guid.NewGuid().ToString();

                client.DefaultRequestHeaders.Add("X-Digipost-UserId", userId);
                client.DefaultRequestHeaders.Add("Date", date);
                client.DefaultRequestHeaders.Add("Accept", "application/vnd.digipost-v6+xml");

                using (var content = new MultipartFormDataContent(boundary))
                {
                    var mediaTypeHeaderValue = new MediaTypeHeaderValue("multipart/mixed");
                    mediaTypeHeaderValue.Parameters.Add(new NameValueWithParametersHeaderValue("boundary",boundary));
                    content.Headers.ContentType = mediaTypeHeaderValue;

                    {
                        var messageContent = new StringContent(xmlMessage);
                        messageContent.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.digipost-v6+xml");
                        messageContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = "\"message\""
                        };
                        content.Add(messageContent);
                    }

                    {
                        var documentContent = new ByteArrayContent(GetDocument());
                        documentContent.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                        documentContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                        {
                            FileName = DocumentGuid
                        };
                        content.Add(documentContent);
                    }

                    var multipartContent = await content.ReadAsStringAsync();
                    var computeHash = ComputeHash(multipartContent);
                   
                    client.DefaultRequestHeaders.Add("X-Content-SHA256", computeHash);
                    client.DefaultRequestHeaders.Add("X-Digipost-Signature", ComputeSignature(method, uri, date, computeHash, userId));

                    try
                    {
                        var result = client.PostAsync(uri, content).Result;
                        Debug.WriteLine(await result.Content.ReadAsStringAsync());
                    }
                    catch (Exception e)
                    {
                        int i = 0;
                    }
                }


            }

            return null;
        }

        private static string ComputeHash(string input)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();
            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            Byte[] hashedBytes = hashAlgorithm.ComputeHash(inputBytes);

            return Convert.ToBase64String(hashedBytes);
        }

        private static string ComputeSignature(string method, string uri, string date, string sha256Hash, string userId)
        {
            var parameters = ""; //HttpUtility.UrlEncode(request.RequestUri.Query).ToLower();

            Debug.WriteLine("Canonical string generated by .NET Client:");
            Debug.WriteLine("===START===");

            var s = method.ToUpper() + "\n" +
                    "/" + uri.ToLower() + "\n" +
                    "date: " + date + "\n" +
                    "x-content-sha256: " + sha256Hash + "\n" +
                    "x-digipost-userid: " + userId + "\n" +
                    parameters + "\n";

            Debug.Write(s);
            Debug.WriteLine("===SLUTT===");


            var rsa = GetCert().PrivateKey as RSACryptoServiceProvider;
            var privateKeyBlob = rsa.ExportCspBlob(true);
            var rsa2 = new RSACryptoServiceProvider();
            rsa2.ImportCspBlob(privateKeyBlob);

            var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(s));
            var signature = rsa2.SignHash(hash, CryptoConfig.MapNameToOID("SHA256"));

            return Convert.ToBase64String(signature);
        }

        private static X509Certificate2 GetCert()
        {
            return
                new X509Certificate2(
                    @"Z:\aleksander sjafjell On My Mac\Development\Shared\sdp-data\testdata\rest\certificate.p12",
                    "Qwer12345", X509KeyStorageFlags.Exportable);
        }

        private static byte[] GetDocument()
        {
            var path = @"Z:\aleksander sjafjell On My Mac\Development\Shared\sdp-data\testdata\hoveddokument\Hoveddokument.txt";
            return File.ReadAllBytes(path);
        }

        private static string GetXmlMessage()
        {
            
            return
                string.Format("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><message xmlns=\"http://api.digipost.no/schema/v6\"><recipient>" +
                              "<personal-identification-number>01013300001</personal-identification-number></recipient>" +
                              "<primary-document><uuid>{0}</uuid><subject>Dokumentets emne</subject><file-type>txt</file-type><sms-notification><after-hours>1</after-hours></sms-notification><authentication-level>PASSWORD</authentication-level><sensitivity-level>NORMAL</sensitivity-level>" +
                              "</primary-document></message>", DocumentGuid);
        }

        private static string GetUserId()
        {
            return "106768801";
        }
    }
}

//public class Sender2
//    {
//        public static readonly string BaseAddress = "https://qa2.api.digipost.no/";
//        public static async Task<string> SendAsync(string forsendelseId, string digipostAdresse, string emne)
//        {
//            var loggingHandler = new LoggingHandler(new HttpClientHandler());
//            using (var client = new HttpClient(loggingHandler))
//            {
//                client.BaseAddress = new Uri(BaseAddress);
//                var date = DateTime.UtcNow.ToString("R");
//                var hash = ComputeHash(GetXmlMessage());
//                var userId = GetUserId();
//                client.DefaultRequestHeaders.Add("X-Digipost-UserId", userId);
//                client.DefaultRequestHeaders.Add("Date", date);
//                client.DefaultRequestHeaders.Add("X-Content-SHA256", hash);
//                var request = new HttpRequestMessage(HttpMethod.Post, "messages")
//                {
//                    Content = new StringContent(GetXmlMessage(), Encoding.UTF8, "application/vnd.digipost-v6+xml")
//                };
//                client.DefaultRequestHeaders.Add("X-Digipost-Signature", ComputeSignature(request, date, hash, userId));
//                var response = await client.SendAsync(request);
//            }
//            return null;
//        }
//        private static string ComputeSignature(HttpRequestMessage request, string date, string sha256hash, string userId)
//        {
//            var parameters = "";//HttpUtility.UrlEncode(request.RequestUri.Query).ToLower();
//            Debug.WriteLine("Canonical string generated by .NET Client:");
//            Debug.WriteLine("===START===");
//            var s = request.Method.ToString().ToUpper() + "\n" +
//            "/" + request.RequestUri.ToString().ToLower() + "\n" +
//            "date: " + date + "\n" +
//            "x-content-sha256: " + sha256hash + "\n" +
//            "x-digipost-userid: " + userId + "\n" +
//            parameters + "\n";
//            Debug.Write(s);
//            Debug.WriteLine("===SLUTT===");
//            var rsa = GetCert().PrivateKey as RSACryptoServiceProvider;
//            var privateKeyBlob = rsa.ExportCspBlob(true);
//            var rsa2 = new RSACryptoServiceProvider();
//            rsa2.ImportCspBlob(privateKeyBlob);
//            var sha = SHA256.Create();
//            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(s));
//            var signature = rsa2.SignHash(hash, CryptoConfig.MapNameToOID("SHA256"));
//            return Convert.ToBase64String(signature);
//        }
//        private static X509Certificate2 GetCert()
//        {
//            return new X509Certificate2(@"Z:\aleksander sjafjell On My Mac\Development\Shared\sdp-data\testdata\rest\certificate.p12", "Qwer12345", X509KeyStorageFlags.Exportable);
//        }
//        private static string ComputeHash(string input)
//        {
//            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();
//            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);
//            Byte[] hashedBytes = hashAlgorithm.ComputeHash(inputBytes);
//            return Convert.ToBase64String(hashedBytes);
//        }
//        private static string GetXmlMessage()
//        {
//            return "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><message xmlns=\"http://api.digipost.no/schema/v6\"><recipient><personal-identification-number>01013300001</personal-identification-number></recipient><primary-document><uuid>37740f5c-3654-45b8-923f-be4fc8a56af5</uuid><subject>Dokumentets emne</subject><file-type>pdf</file-type><sms-notification><after-hours>1</after-hours></sms-notification><authentication-level>PASSWORD</authentication-level><sensitivity-level>NORMAL</sensitivity-level></primary-document></message>";
//        }
//        private static string GetUserId()
//        {
//            return "106768801";
//        }


//    }
//}
