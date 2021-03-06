﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.Types;
using Common.Services.Payment.Interfaces;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
namespace Common.Services.Payment.Tests.Gateways.AuthorizeNet
{
    [TestClass]
    public class AuthorizeNetCIMCreditCardGatewayTest
    {
        public class Container
        {

            IUnityContainer _UnityContainer;
            static Container _Instance;

            private Container()
            {
                _UnityContainer = new UnityContainer();
                _UnityContainer.LoadConfiguration();

            }
            public static Container Instance
            {
                get
                {
                    if (_Instance == null)
                        _Instance = new Container();
                    return Container._Instance;
                }
                set { Container._Instance = value; }
            }
            public IAddressType GetNewAddressType()
            {
                return _UnityContainer.Resolve<IAddressType>();
            }
            public IPaymentGatewaySettings GetNewPaymentGatewaySettings()
            {
                return _UnityContainer.Resolve<IPaymentGatewaySettings>();
            }
            public IPaymentCardData GetNewPaymentCardData()
            {
                return _UnityContainer.Resolve<IPaymentCardData>();
            }
            public ICustomerData GetNewCustomerData()
            {
                return _UnityContainer.Resolve<ICustomerData>();
            }
            public IPaymentData GetNewPaymentData()
            {
                return _UnityContainer.Resolve<IPaymentData>();
            }
            public ITransactionData GetNewTransactionData()
            {
                return _UnityContainer.Resolve<ITransactionData>();
            }
            public IProfileCreditCardGateway GetNewProfileCreditCardGateway()
            {
                return _UnityContainer.Resolve<IProfileCreditCardGateway>();
            }
        }
        static IPaymentGatewaySettings GetPaymentGatewaySettings()
        {
            var result = _Container.GetNewPaymentGatewaySettings();
            result.TestMode = true;
            result.Username = "9E4n3PdG";
            result.Password = "8RMF6ZjL2D4z8d75";
            return result;
        }
        static IPaymentData GetPaymentData()
        {
            var result = new PaymentData();
            result.CardData = new PaymentCardData();
            result.CardData.CardHolderName = "Test Cardholder";
            result.CardData.CardNumber = "4111111111111111";
            result.CardData.CardType = PaymentCardType.Visa;
            result.CardData.ExpirationMonth = 1;
            result.CardData.ExpirationYear = 2020;
            result.CardData.SecurityCode = "123";

            result.Customer = new CustomerData();
            result.Customer.Address = new AddressType();
            result.Customer.Address.AddressLine1 = "123 Test Street";
            result.Customer.Address.City = "Testville";
            result.Customer.Address.Country = "US";
            result.Customer.Address.PostalCode = "32323";
            result.Customer.FirstName = "Test";
            result.Customer.LastName = "Cardholder";
            result.Customer.EmailAddress = Guid.NewGuid().ToString().Replace("-",String.Empty) + "@sxddasdasddonotresolve.com";
            result.Transaction = new TransactionData();
            result.Transaction.Amount = (decimal)0.01;
            result.Transaction.PreviousTransactionReferenceNumber = "0";
            result.CardData.CardHolderFirstName = "Test";
            result.CardData.CardHolderLastName = "Cardholder";
            result.CardData.BillingAddress = result.Customer.Address;
            return result;
        }
        static IRecurringPaymentData GetRecurringPaymentData()
        {
            var result = new RecurringPaymentData();
            result.CardData = new PaymentCardData();
            result.CardData.CardHolderName = "Test Cardholder";
            result.CardData.CardNumber = "4111111111111111";
            result.CardData.CardType = PaymentCardType.Visa;
            result.CardData.ExpirationMonth = 1;
            result.CardData.ExpirationYear = 2020;
            result.CardData.SecurityCode = "123";

            result.Customer = new CustomerData();
            result.Customer.Address = new AddressType();
            result.Customer.Address.AddressLine1 = "123 Test Street";
            result.Customer.Address.City = "Testville";
            result.Customer.Address.Country = "US";
            result.Customer.Address.PostalCode = "32323";
            result.Customer.FirstName = "Test";
            result.Customer.LastName = "Cardholder";
            result.Customer.EmailAddress = Guid.NewGuid().ToString().Replace("-", String.Empty) + "@sxddasdasddonotresolve.com";
            result.Transaction = new TransactionData();
            result.Transaction.Amount = (decimal)0.01;
            result.Transaction.PreviousTransactionReferenceNumber = "0";
            result.CardData.CardHolderFirstName = "Test";
            result.CardData.CardHolderLastName = "Cardholder";
            result.CardData.BillingAddress = result.Customer.Address;

            result.Intervals = 12;
            result.RecurringIntervalPrice = (decimal)19.95;
            result.RecurringPaymentInterval = RecurringPaymentIntervals.Months;
            result.StartDate = System.DateTime.Now;
            
            return result;
        }
        #region Test Helper Methods
        static Container _Container;
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            _Container = Container.Instance;          
            
        }
        public static IPaymentData GetRefundTransactionPaymentData()
        {
            var result = _Container.GetNewPaymentData();
            result.Id = "3756118";
            result.Transaction = new TransactionData();
            result.Transaction.PreviousTransactionReferenceNumber = "2161773175";
            result.Customer = _Container.GetNewCustomerData();
            result.Transaction = _Container.GetNewTransactionData();
            result.Transaction.Amount = (decimal)0.01;
            result.Customer.CustomerId = "4255825";
            return result;
        }
        public static IGatewayProfile CreateProfile(IPaymentData data)
        {
            var target = _Container.GetNewProfileCreditCardGateway();
            return target.GetOrCreateCustomerProfile(data);
        }
        public static IGatewayProfile GetGatewayProfile(IGatewayProfile profile)
        {
            IProfileCreditCardGateway target = _Container.GetNewProfileCreditCardGateway();
            return target.GetCustomerProfile(profile.Id);
        }
        #endregion

        [TestMethod]
        public void AuthorizeTest_WhenValidData_CheckResultIsTrue()
        {
            //Arrange
            var target = Container.Instance.GetNewProfileCreditCardGateway();
            target.GatewaySettings = GetPaymentGatewaySettings();
            var paymentData = GetPaymentData();
            //Act
            var actual = target.Authorize(paymentData);
            //Assert
            Assert.IsTrue(actual);
        }      
        [TestMethod]
        public void CreateProfileTest_AssertProfileResponseLengthGreaterThanZero()
        {
            //Arrange
            IGatewayProfile actual = CreateProfile(GetPaymentData());
            //Assert
            Assert.IsTrue(int.Parse(actual.Id)>0);
        }
        [TestMethod]
        public void GetCustomerProfileTest_WhenProfileExists_AssertProfileIdsMatch()
        {
            //Arrange
            var customer = CreateProfile(GetPaymentData());
            //Act
            var actual = GetGatewayProfile(customer);
            //Assert
            Assert.IsTrue(customer.Id == actual.Id);
            
        }
        [TestMethod]
        public void AuthorizeTest_WhenDuplicate_CheckResponseStillOk()
        {
            //Arrange
            var target = _Container.GetNewProfileCreditCardGateway();
            var paymentData = GetPaymentData();
            paymentData.Transaction.Amount = (new System.Random().Next(1, 99) * (decimal)0.01);
            var expected = target.Authorize(paymentData);
            //PreQualify
            Assert.IsTrue(expected);
            paymentData.Transaction.Amount = paymentData.Transaction.Amount + (decimal)0.10;
            //Act
            var actual = target.Authorize(paymentData);
            //Assert
            Assert.IsTrue(actual);

           
        }
        [TestMethod]
        public void RefundTest_WhenUsingRefundableTransaction_CheckResponseOk()
        {
            //Arrange
            var target = _Container.GetNewProfileCreditCardGateway();
            var paymentData = GetRefundTransactionPaymentData();
            //Act
            var actual = target.Refund(paymentData);
            //Assert
            Assert.IsTrue(actual);
        }
        [TestMethod]
        public void AuthorizeTest_WhenValidData_CheckTransactionResultMessageCountGreaterThanZero()
        {
            //Arrange
            var target = _Container.GetNewProfileCreditCardGateway();
            var paymentData = GetPaymentData();
            //Act
            var actual = target.Authorize(paymentData);
            //Assert
            Assert.IsTrue(paymentData.Transaction.TransactionMessages.Count>0);
        }
        [TestMethod]
        public void RefundTest_WhenUsingRefundableTransaction_CheckTransactionResultMessageCountGreaterThanZero()
        {
            //Arrange
            var target = _Container.GetNewProfileCreditCardGateway();
            var paymentData = GetRefundTransactionPaymentData();
            //Act
            var actual = target.Refund(paymentData);
            //Assert
            Assert.IsTrue(paymentData.Transaction.TransactionMessages.Count > 0);
        }
        [TestMethod]
        public void CreateSubscriptionTest_WhenValidData_CheckResultIsTrue()
        {
            //Arrange
            var target = _Container.GetNewProfileCreditCardGateway() as ISubscriptionCreditCardGateway;
            var recurringPaymentData = GetRecurringPaymentData();
            recurringPaymentData.Customer.FirstName = "TestUser" + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 5);
            recurringPaymentData.Customer.LastName = "TestUserLast" + Guid.NewGuid().ToString().Replace("-", "").Substring(0, 5);
            //Act
            var actual = target.CreateSubscription(recurringPaymentData);
            //Assert
            Assert.IsTrue(actual);

        }
    }

   
}
