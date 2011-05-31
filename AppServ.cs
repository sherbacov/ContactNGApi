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
        
        [XmlAttribute]
        public string ACTION;

        [XmlAttribute]
        public string OBJECT_CLASS;

        [XmlIgnore]
        public string Intergation;

        [XmlIgnore]
        public int RETURN_TRN_DATA;

        [XmlIgnore]
        public int NOEXCEPTION;

        [XmlIgnore]
        public int OFAC_SENDER;

        [XmlIgnore]
        public int OFAC_REC;

        [XmlAttribute]
        public string INT_SOFT_ID;

        [XmlAttribute]
        public string POINT_CODE;

        [XmlIgnore]
        public int USER_ID;

        [XmlIgnore]
        public string USER_LOGIN;
        
        [XmlIgnore]
        public string LANG;

        [XmlIgnore]
        public string CLIENT_IP;
        
        [XmlIgnore]
        public string SERVER_IP;
        
        [XmlIgnore]
        public string REMOTE_CLIENT_IP;
        
        [XmlIgnore]
        public string REQUEST_ID;
        
        [XmlIgnore]
        public string REQUEST_STEP;

        [XmlAttribute]
        public string TYPE_VERSION;
        
        [XmlAttribute]
        public int VERSION;

        [XmlAttribute]
        public string ExpectSigned = "Yes";

        public Request(){}

        //protected Request(SerializationInfo info, StreamingContext context)
        //{
            //n1 = info.GetInt32("i");
            //n2 = info.GetInt32("j");
            //str = info.GetString("k");
        //}

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
        /// <summary>
        /// Получение из person ФИО клиента вида: Иванов И.И. или Иванов И. (нет отч.) или Иванов (только фамилия)
        /// </summary>
        /// <param name="person">Отправитель или Получатель ContactPerson</param>
        /// <returns>ФИЮ. Пример Иванов И.И.</returns>
        public string GetShortFullName()
        {
            var fullname = "";

            if (Name != null)
            {
                fullname = Name.ToLower();
                fullname = fullname[0].ToString().ToUpper() + fullname.Substring(1).ToLower();
            }

            if (LastName != null)
                fullname += " " + LastName.ToUpper().Substring(0, 1) + ".";

            if (SurName != null)
                fullname += SurName.ToUpper().Substring(0, 1) + ".";

            return fullname;
        }

    }


    public class Fees
    {
        public decimal Rate;
        public decimal FeesPart;
        /// <summary>
        /// Комиссия с клиента
        /// </summary>
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

    // ReSharper disable InconsistentNaming
    /// <summary>
    /// Точка отправления или получения
    /// </summary>
    public class ContactPoint
    {
        public int Bank_Id;
        public string PP_Code;
        public string Country;
        public string Country_Name;
        public string Bank_Name;
        public string Rec_Curr;
        public string Rec_Curr_Ids;
        public string Addr;
        public string Phone;
        public string Service_Name;
        public string City_Head;
        public int    Region;
        public string Region_Name;
        public string City_Bank;
        public string Country_Bank;
        public string Logo;
        public int Re;
    }
    // ReSharper restore InconsistentNaming


    
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
        /// Вид услуги. 2 - перевод без открытия счета
        /// </summary>
        public int Service;

        /// <summary>
        /// Код точки отправления
        /// </summary>
        public string SendPoint;
        /// <summary>
        /// Код точки получения
        /// </summary>
        public string PickupPoint;

        /// <summary>
        /// Точка получения
        /// </summary>
        public ContactPoint Pickup = new ContactPoint();

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

    }

}
