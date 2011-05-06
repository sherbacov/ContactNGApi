﻿using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ContactApi
{
    [XmlRoot("RESPONSE")]
    public class Response
    {
        [XmlAttribute("RE")]
        public int ReturnCode;
        /// <summary>
        /// Текст ошибки для IT-специалиста
        /// </summary>
        [XmlAttribute("ERR_TEXT")]
        public string ErrorText;
        /// <summary>
        /// Текст ошибки для пользователя
        /// </summary>
        [XmlAttribute("VIEW_ERR_TEXT")]
        public string ViewErrorText;
    }

    [Serializable]
    [XmlRoot("REQUEST")]
    public class Request
    {
        public string DOC_ID;
        
        public string ACTION;

        public string OBJECT_CLASS;

        public string Intergation;

        public int RETURN_TRN_DATA;

        public int NOEXCEPTION;

        public int OFAC_SENDER;

        public int OFAC_REC;

        public string INT_SOFT_ID;

        public string POINT_CODE;

        public int USER_ID;
        
        public string USER_LOGIN;

        public string LANG;

        public string CLIENT_IP;
        public string SERVER_IP;
        public string REMOTE_CLIENT_IP;
        public string REQUEST_ID;
        public string REQUEST_STEP;

        public Request()
        {
        }

        protected Request(SerializationInfo info, StreamingContext context)
        {
            
            
            //n1 = info.GetInt32("i");
            //n2 = info.GetInt32("j");
            //str = info.GetString("k");
        }

    }

    [Serializable]
    [XmlRoot("RESPONSE")]
    public class ContactResponse
    {
        public int ID;
        public int SIGN_IT;
        public int RE;
        public int STATE;
        public string GLOBAL_VERSION;
        public string GLOBAL_VERSION_SERVER;
        public string REQUEST_ID;
        public int REQUEST_STEP;

        public ContactResponse()
        {
        }
    }

    
//  <sZipCode /> 
//  <sRegion /> 
//  <sPhone /> 
//  <sIDexpireDate /> 
//  <sCountry>AZ</sCountry> 
//  <sCity /> 
//  <sAddress /> 


//  <sIDtype /> 
//  <sIDnumber /> 
//  <sIDdate /> 
//  <sIDwhom /> 
//  <sCountryC>AB</sCountryC> 


 //<bIDtype>ПАСПОРТ ГР.РФ</bIDtype> 
  //<bIDnumber>8812733302</bIDnumber> 
  //<bIDdate>20010414</bIDdate> 
  //<bIDwhom>МВД РФ</bIDwhom> 
  //<bIDexpireDate>20160831</bIDexpireDate> 


    public class PersonId
    {
        public string IdType;
        public string IdNumber;
        public DateTime IdDate;
        public string IdWhom;
        public DateTime IdExpireDate;
    }

    public class PersonAddress
    {
        /// <summary>
        /// Код региона
        /// </summary>
        public int Region;
        /// <summary>
        ///  Адрес клиента
        /// </summary>
        public string Address;
        /// <summary>
        ///  Индекс клиента
        /// </summary>
        public string ZipCode;
        /// <summary>
        ///  Город
        /// </summary>
        public string City;
        /// <summary>
        ///  Страна, код ISO 2 символа
        /// </summary>
        public string Country;
    }

    public class Person
    {
        public string Name;
        public string LastName;
        public string SurName;

        public DateTime Birthday;
    }

    public class PhysicalPerson : Person
    {
        /// <summary>
        /// Адрес Физического лица
        /// </summary>
        public PersonAddress Address = new PersonAddress();
        /// <summary>
        /// Документ Физического лица
        /// </summary>
        public PersonId Id = new PersonId();
        /// <summary>
        /// Номер телефона
        /// </summary>
        public string Phone;
        /// <summary>
        /// Резидент - 1, НеРезидент - 0 
        /// </summary>
        public bool Resident;
    }

    public class ContactPerson :  PhysicalPerson
    {
        

    }


    public class Fees
    {
        public decimal Rate;
        public decimal FeesClient;
        public decimal FeesClientLocal;
        public string  FeesClientCurr;  
    }

    /// <summary>
    /// Перевод
    /// </summary>
    public class Transfer
    {
        public DateTime Date;
        public string Reference;
    }

    public enum ContactTransferStatus
    {
        New = 100,
        Payed = 0,
        Send = 3, 
        ReadyToPay = 101
    }

    public enum ContactTransferDirection
    {
        Incoming,
        Outgoing
    }


    public class ContactTransfer : Transfer
    {
        public ContactTransferDirection Direction;
        
        public decimal Amount;
        public string Currency;

        public string SendPoint;
        public string PickupPoint;

        public ContactTransferStatus Status;

        public Fees Fee = new Fees();

        public ContactPerson Sender = new ContactPerson();
        public ContactPerson Resiver = new ContactPerson();
    }


    [XmlRoot("TMoneyOrderObject")]
    public class MoneyOrderObject
    {
        

    }
}