﻿using System;
using System.IO;
using System.Reflection;
using System.Xml;
using Digipost.Api.Client.Shared.Resources.Resource;

namespace Digipost.Api.Client.Resources.Xsd
{
    internal static class XsdResource
    {
        private static readonly ResourceUtility ApiResourceUtility = new ResourceUtility(typeof(XsdResource).Assembly, "Digipost.Api.Client.Resources.Xsd.Data");
        private static readonly ResourceUtility DataTypesResourceUtility = new ResourceUtility(Assembly.Load("Digipost.Api.Client.DataTypes"), "Digipost.Api.Client.DataTypes.Resources.XSD");

        public static XmlReader GetApiV7Xsd()
        {
            return XmlReader.Create(new MemoryStream(ApiResourceUtility.ReadAllBytes("api_v7.xsd")));
        }

        public static XmlReader GetDataTypesXsd()
        {
            return XmlReader.Create(new MemoryStream(DataTypesResourceUtility.ReadAllBytes("datatypes.xsd")));
        }
    }
}
