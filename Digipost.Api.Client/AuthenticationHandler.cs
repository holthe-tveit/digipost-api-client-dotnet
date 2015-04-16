﻿using System;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Digipost.Api.Client
{

    public class AuthenticationHandler : DelegatingHandler
    {
        private ClientConfig ClientConfig { get; set; }
        private string Url { get; set; }
        private X509Certificate2 PrivateCertificate { get; set; }

        public AuthenticationHandler(ClientConfig clientConfig, X509Certificate2 privateCertificate, string url, HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            ClientConfig = clientConfig;
            Url = url;
            PrivateCertificate = privateCertificate;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Logging.Log(TraceEventType.Information, " AuthenticationHandler > sendAsync() - Start!");
            const string method = "POST";
            var date = DateTime.UtcNow.ToString("R");
            var technicalSender = ClientConfig.TechnicalSenderId;
            var multipartContent = await request.Content.ReadAsByteArrayAsync();
            
            Logging.Log(TraceEventType.Information, " - Hashing byteStream of body content");
            var computeHash = ComputeHash(multipartContent);

            request.Headers.Add("X-Digipost-UserId", ClientConfig.TechnicalSenderId);
            request.Headers.Add("Date", date);
            request.Headers.Add("Accept", "application/vnd.digipost-v6+xml");
            request.Headers.Add("X-Content-SHA256", computeHash);
            request.Headers.Add("X-Digipost-Signature", ComputeSignature(method, Url, date, computeHash, technicalSender,PrivateCertificate));

            
            return await base.SendAsync(request, cancellationToken);
        }


        private static string ComputeHash(Byte[] inputBytes)
        {
            HashAlgorithm hashAlgorithm = new SHA256CryptoServiceProvider();
            var hashedBytes = hashAlgorithm.ComputeHash(inputBytes);

            return Convert.ToBase64String(hashedBytes);
        }

        private static string ComputeSignature(string method, string uri, string date, string sha256Hash, string userId, X509Certificate2 privateCertificate)
        {
            const string parameters = ""; //HttpUtility.UrlEncode(request.RequestUri.Query).ToLower();

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


            var rsa = privateCertificate.PrivateKey as RSACryptoServiceProvider;
            var privateKeyBlob = rsa.ExportCspBlob(true);
            var rsa2 = new RSACryptoServiceProvider();
            rsa2.ImportCspBlob(privateKeyBlob);

            var sha = SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(s));
            var signature = rsa2.SignHash(hash, CryptoConfig.MapNameToOID("SHA256"));

            return Convert.ToBase64String(signature);
        }

      
    }
}
