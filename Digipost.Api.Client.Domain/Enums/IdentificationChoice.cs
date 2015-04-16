﻿using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Digipost.Api.Client.Domain.Enums
{
    [GeneratedCode("xsd", "4.0.30319.33440")]
    [Serializable]
    [XmlType(Namespace = "http://api.digipost.no/schema/v6", IncludeInSchema = false)]
    public enum IdentificationChoice
    {
        [XmlEnum("digipost-address")] 
        DigipostAddress,

        [XmlEnum("name-and-address")] 
        NameAndAddress,

        [XmlEnum("organisation-number")] 
        OrganisationNumber,

        [XmlEnum("personal-identification-number")] 
        PersonalidentificationNumber
    }
}