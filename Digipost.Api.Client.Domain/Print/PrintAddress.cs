﻿using System;
using System.ComponentModel;
using System.Xml.Serialization;
using Digipost.Api.Client.Domain.Exceptions;

namespace Digipost.Api.Client.Domain.Print
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(TypeName = "print-recipient", Namespace = "http://api.digipost.no/schema/v6")]
    [XmlRoot("print-recipient", Namespace = "http://api.digipost.no/schema/v6", IsNullable = true)]
    public abstract class PrintAddress
    {
        private object _address;

        /// <summary>
        ///     Sets the name of the recipient.(Also used for the return address)
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        ///     Sets the address of the recipient.(Also used for the return address) Choose between ForeignAddress or
        ///     NorwegianAddress.
        /// </summary>
        [XmlElement("foreign-address", typeof(ForeignAddress))]
        [XmlElement("norwegian-address", typeof(NorwegianAddress))]
        public object Address
        {
            get { return _address; }
            set
            {
                if (!(value is ForeignAddress) && !(value is NorwegianAddress))
                    throw new APIException("Invalid type of Address! Valid types are [NorwegianAddress,Foreignaddress]");

                _address = value;
            }
        }
    }
}