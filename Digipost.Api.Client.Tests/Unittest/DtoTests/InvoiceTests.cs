﻿using System;
using System.IO;
using Digipost.Api.Client.Domain.SendMessage;
using Digipost.Api.Client.Tests.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Digipost.Api.Client.Tests.Unittest.DtoTests
{
    [TestClass]
    public class InvoiceTests
    {
        [TestClass]
        public class ConstructorMethod : InvoiceTests
        {
            [TestMethod]
            public void CreateFromContentBytes()
            {
                //Arrange
                DateTime dueDate = DateTime.Now.AddDays(20);
                Invoice invoice = new Invoice("An invoice", "txt", new byte[]{1,2,3}, 125, "13452564677", dueDate, "1234567890kid");

                //Act

                //Assert
                Assert.AreEqual("13452564677", invoice.Account);
                Assert.AreEqual(125, invoice.Amount);
                Assert.AreEqual(dueDate,invoice.Duedate);
                Assert.AreEqual("1234567890kid", invoice.Kid);
            }

            [TestMethod]
            public void CreateFromPath()
            {
                //Arrange
                DateTime dueDate = DateTime.Now.AddDays(20);
                FakeInvoice invoice = new FakeInvoice("An invoice", "txt", "c://imaginary/file", 125, "13452564677", dueDate, "1234567890kid");

                //Act

                //Assert
                Assert.AreEqual("13452564677", invoice.Account);
                Assert.AreEqual(125, invoice.Amount);
                Assert.AreEqual(dueDate, invoice.Duedate);
                Assert.AreEqual("1234567890kid", invoice.Kid);
            }

            [TestMethod]
            public void CreateFromStream()
            {
                //Arrange
                DateTime dueDate = DateTime.Now.AddDays(20);
                Invoice invoice = new Invoice("An invoice", "txt", Stream.Null, 125, "13452564677", dueDate, "1234567890kid");

                //Act

                //Assert
                Assert.AreEqual("13452564677", invoice.Account);
                Assert.AreEqual(125, invoice.Amount);
                Assert.AreEqual(dueDate, invoice.Duedate);
                Assert.AreEqual("1234567890kid", invoice.Kid);
            }

        }

       




    }
}