using System;
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

        public Request(){}

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
        /// Региона
        /// </summary>
        public string Region;
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
        public decimal FeesPart;
        public decimal FeesClient;
        public decimal FeesClientLocal;
        public string  FeesClientCurr;
        /// <summary>
        /// Сумма получения из КЦ
        /// </summary>
        public decimal FeesPartRet;
    }

    /// <summary>
    /// Перевод
    /// </summary>
    public class Transfer
    {
        /// <summary>
        /// Дата перевода
        /// </summary>
        public DateTime Date;
        /// <summary>
        /// Номер перевода
        /// </summary>
        public string Reference;
    }

    public enum ContactTransferStatus
    {
        New = 100,
        Payed = 0,
        Send = 3, 
        ReadyToPay = 101,
        /// <summary>
        ///  Готов к выплате
        /// </summary>
        ReadyToPayOut = 5,
        /// <summary>
        ///  Выплачено
        /// </summary>
        PayOut = 5

    }

    public enum ContactTransferDirection
    {
        /// <summary>
        /// Входящий перевод
        /// </summary>
        Incoming,
        /// <summary>
        /// Исходящий перевод
        /// </summary>
        Outgoing
    }


    public class ContactTransfer : Transfer
    {
        public ContactTransferDirection Direction;
        
        /// <summary>
        /// Сумма перевода
        /// </summary>
        public decimal Amount;
        /// <summary>
        /// Валюта перевода. Пример RUR - рубли
        /// </summary>
        public string Currency;

        /// <summary>
        /// Код точки отправления
        /// </summary>
        public string SendPoint;
        /// <summary>
        /// Код точки получения
        /// </summary>
        public string PickupPoint;

        /// <summary>
        /// Статус перевода
        /// </summary>
        public ContactTransferStatus Status;

        /// <summary>
        /// Комиссии с перевода
        /// </summary>
        public Fees Fee = new Fees();
        /// <summary>
        /// Отправитель перевода
        /// </summary>
        public ContactPerson Sender = new ContactPerson();

        /// <summary>
        /// Получатель перевода
        /// </summary>
        public ContactPerson Resiver = new ContactPerson();

        /// <summary>
        /// Получение из person ФИО клиента вида: Иванов И.И. или Иванов И. (нет отч.) или Иванов (только фамилия)
        /// </summary>
        /// <param name="person">Отправитель или Получатель ContactPerson</param>
        /// <returns>ФИЮ. Пример Иванов И.И.</returns>
        public string GetShortFullName(ContactPerson person)
        {
            var fullname = "";

            if (person.Name != null)
            {
                fullname = person.Name.ToLower();
                fullname = fullname[0].ToString().ToUpper() + fullname.Substring(1).ToLower();
            }

            if (person.LastName != null)
                fullname += " "  + person.LastName.ToUpper().Substring(0, 1) + ".";

            if (person.SurName != null)
                fullname += person.SurName.ToUpper().Substring(0, 1) + ".";

            return fullname;
        }
    }

}
