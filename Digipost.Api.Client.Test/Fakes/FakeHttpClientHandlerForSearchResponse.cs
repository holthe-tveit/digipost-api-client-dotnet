﻿using System.Net.Http;

namespace Digipost.Api.Client.Test.Fakes
{
    internal class FakeHttpClientHandlerForSearchResponse : FakeHttpClientHandlerResponse
    {
        public override HttpContent GetContent()
        {
            return new StringContent(
                "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>" +
                "<recipients xmlns=\"http://api.digipost.no/schema/v6\">" +
                "<recipient>" +
                "<firstname>Marit</firstname>" +
                "<middlename></middlename>" +
                "<lastname>Johansen</lastname>" +
                "<digipost-address>marit.johansen#11111</digipost-address>" +
                "<mobile-number>*****000</mobile-number>" +
                "<organisation-name></organisation-name>" +
                "<address>" +
                "<street>Gata</street>" +
                "<house-number>1</house-number>" +
                "<house-letter>A</house-letter>" +
                "<additional-addressline></additional-addressline>" +
                "<zip-code>0101</zip-code>" +
                "<city>Oslo</city>" +
                "</address>" +
                "<link rel=\"https://qa2.api.digipost.no/relations/self\" uri=\"https://qa2.api.digipost.no/recipients/marit.johansen%111111\" media-type=\"application/vnd.digipost-v6+xml\"/>" +
                "</recipient>" +
                "<recipient>" +
                "<firstname>Marit</firstname>" +
                "<middlename>Olivia</middlename>" +
                "<lastname>Johansen</lastname>" +
                "<digipost-address>marit.olivia.johansen#1234</digipost-address>" +
                "<mobile-number>*****000</mobile-number>" +
                "<organisation-name></organisation-name>" +
                "<address>" +
                "<street>Bokstad</street>" +
                "<house-number>123</house-number>" +
                "<house-letter>D</house-letter>" +
                "<additional-addressline></additional-addressline>" +
                "<zip-code>8865</zip-code>" +
                "<city>Otta</city>" +
                "</address>" +
                "<link rel=\"https://qa2.api.digipost.no/relations/self\" uri=\"https://qa2.api.digipost.no/recipients/marit.johansen%12311\" media-type=\"application/vnd.digipost-v6+xml\"/>" +
                "</recipient>" +
                "</recipients>");
        }
    }
}