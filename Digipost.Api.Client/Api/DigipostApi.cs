﻿using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using ApiClientShared;
using ApiClientShared.Enums;
using Digipost.Api.Client.Action;
using Digipost.Api.Client.Domain;
using Digipost.Api.Client.Domain.Exceptions;

namespace Digipost.Api.Client.Api
{
    internal class DigipostApi : IDigipostApi
    {
        public DigipostApi(ClientConfig clientConfig, X509Certificate2 businessCertificate)
        {
            ClientConfig = clientConfig;
            BusinessCertificate = businessCertificate;
        }

        public DigipostApi(ClientConfig clientConfig, string thumbprint)
        {
            ClientConfig = clientConfig;
            BusinessCertificate = CertificateUtility.SenderCertificate(thumbprint, Language.English);
        }

        private ClientConfig ClientConfig { get; set; }
        private X509Certificate2 BusinessCertificate { get; set; }

        public MessageDeliveryResult SendMessage(Message message)
        {
            var messageDelivery = SendMessageAsync(message);

            if (messageDelivery.IsFaulted)
            {
                if (messageDelivery.Exception != null) throw messageDelivery.Exception.InnerException;
            }

            return messageDelivery.Result;
        }

        public IdentificationResult Identify(Identification identification)
        {
            var identifyResponse = IdentifyAsync(identification);

            if (identifyResponse.IsFaulted)
            {
                if (identifyResponse.Exception != null) throw identifyResponse.Exception.InnerException;
            }
            return identifyResponse.Result;
        }

        public async Task<MessageDeliveryResult> SendMessageAsync(Message message)
        {
            const string uri = "messages";
            var result = await GenericSendAsync<MessageDeliveryResult>(message, uri);

            return result;
        }

        public Task<IdentificationResult> IdentifyAsync(Identification identification)
        {
            const string uri = "identification";
            return GenericSendAsync<IdentificationResult>(identification, uri);
        }

        private async Task<T> GenericSendAsync<T>(RequestContent message, string uri)
        {
            var action = DigipostActionFactory.CreateClass(message.GetType(), ClientConfig, BusinessCertificate, uri);
            var response = action.SendAsync(message).Result;
            var responseContent = await ReadResponse(response);


            try
            {
                return SerializeUtil.Deserialize<T>(responseContent);
            }
            catch (InvalidOperationException exception)
            {
                var error = SerializeUtil.Deserialize<Error>(responseContent);
                throw new ClientResponseException("Failed to deserialize response object." +
                                                  "Check inner Error object for more information.", error, exception);
            }
        }

        private static async Task<string> ReadResponse(HttpResponseMessage requestResult)
        {
            var contentResult = await requestResult.Content.ReadAsStringAsync();
            return contentResult;
        }
    }
}