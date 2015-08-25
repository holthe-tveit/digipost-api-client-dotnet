﻿using System;
using Digipost.Api.Client.Domain.SendMessage;
using Digipost.Api.Client.Tests.Integration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digipost.Api.Client.Tests.Unittest.DtoTests
{
    [TestClass]
    public class MessageTests
    {
        [TestClass]
        public class ConstructorMethod : MessageTests
        {
            [TestMethod]
            public void ConstructWithRecipientAndPrimaryDocument()
            {
                //Arrange
                Message message = new Message(
                    recipient: DomainUtility.GetRecipientWithDigipostId(), 
                    primaryDocument: DomainUtility.GetDocument()
                    );                

                //Act

                //Assert
                Assert.IsNotNull(message.PrimaryDocument);
                Assert.IsNull(message.SenderId);
            }

            [TestMethod]
            public void ConstructWithSenderId()
            {
                //Arrange
                Message message = new Message(
                    recipient: DomainUtility.GetRecipientWithDigipostId(),
                    primaryDocument: DomainUtility.GetDocument(),
                    senderId: "123456"
                );                
                
                //Act

                //Assert
                Assert.IsNotNull(message.PrimaryDocument);
                Assert.IsNotNull(message.SenderId);
                Assert.IsNotNull(message.Attachments);
                Assert.AreEqual("123456",message.SenderId);
            }
        }

        [TestClass]
        public class DeliveryTimeSpecifiedMethod : MessageTests
        {
            [TestMethod]
            public void DeliveryTimeNotSpecifiedGivesFalse()
            {
                //Arrange
                Message message = new Message(
                    recipient: DomainUtility.GetRecipientWithDigipostId(),
                    primaryDocument: DomainUtility.GetDocument()
                    );               
                
                //Act

                //Assert
                Assert.IsFalse(message.DeliveryTimeSpecified);
            }

            [TestMethod]
            public void DeliveryTimeSpecifiedGivesTrue()
            {
                //Arrange
                Message message = new Message(
                    recipient: DomainUtility.GetRecipientWithDigipostId(),
                    primaryDocument: DomainUtility.GetDocument()
                    );
                message.DeliveryTime = DateTime.Today;

                //Act

                //Assert
                Assert.IsTrue(message.DeliveryTimeSpecified);
            }

        }
    }
}
