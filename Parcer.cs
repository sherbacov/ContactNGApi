using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace ContactApi
{
    public class ParcerNewTrasferEventArgs : EventArgs
    {
        public ParcerNewTrasferEventArgs(Request req, ContactResponse response, ContactTransfer transfer)
        {
            Req = req;
            Response = response;
            Transfer = transfer;
        }

        public Request Req;
        public ContactResponse Response;
        public ContactTransfer Transfer;

    }
    
    public class Parcer
    {
        public IDictionary<string, ContactTransfer> Requests = new Dictionary<string, ContactTransfer>();

        public delegate void NewTransferEventHandler(object sender, ParcerNewTrasferEventArgs e);

        public event NewTransferEventHandler NewTransfer;

        public Response OnNewData(string data)
        {
            var reader = new XmlTextReader(new MemoryStream(Encoding.Default.GetBytes(data)));

            if (reader.Read()) {

                var xmldoc = new XmlDocument();
                xmldoc.LoadXml(data);

                switch (reader.Name.ToUpper())
                {
                    case "REQUEST":
                        Console.WriteLine("Пришел REQUEST");
                        var request = GetRequest(data);

                        if (request.OBJECT_CLASS == "TMoneyOrderObject")
                        {

                            var nodeList = xmldoc.SelectNodes("REQUEST");

                            if (nodeList != null && nodeList.Count == 1)
                            {
                                var transfer = new ContactTransfer
                                                   {
                                                       Direction =
                                                           request.ACTION.Contains("Outgoing")
                                                               ? ContactTransferDirection.Outgoing
                                                               : ContactTransferDirection.Incoming
                                                   };

                                FillTransfer(transfer, nodeList[0]);
                                Requests.Add(request.REQUEST_ID, transfer);
                                
                                OnNewTransfer(new ParcerNewTrasferEventArgs(request, null, transfer));
                                return new Response { ReturnCode = 0, ErrorText = "", ViewErrorText = "" };
                            }
                        }
                        break;
                    
                    case "RESPONSE":
                        Console.WriteLine("Пришел RESPONSE");

                        var response = GetResponse(xmldoc);

                        if (response.RE == 0)
                    {
                        var xml = new XmlDocument();
                        xml.LoadXml(data);

                        var nodeList = xml.SelectNodes("RESPONSE/TRN_DATA/RESPONSE");
                        
                        if (nodeList != null && nodeList.Count == 1)
                        {
                            var xmlRecord = nodeList[0];
                            var responseTransfer = GetResponse(xmlRecord, false);


                            if (!Requests.ContainsKey(response.REQUEST_ID))
                                return new Response { ReturnCode = 60, ErrorText = "Пришел ответ, но запроса нет в Базе. Код: 60", ViewErrorText = "Пришел ответ, но запроса нет в Базе. Код: 60" };

                            var transfer = Requests[response.REQUEST_ID];
                            // Обновить данные из заголовка
                            transfer.Status = (ContactTransferStatus)responseTransfer.STATE;
                            FillTransfer(transfer, xmlRecord);
                            OnNewTransfer(new ParcerNewTrasferEventArgs(null, responseTransfer, transfer));
                        }

                        return new Response { ReturnCode = 0, ErrorText = "", ViewErrorText = "" };
                    }


                        return new Response { ReturnCode = 110, ErrorText = "Данный тип запроса не поддерживается.", ViewErrorText = "Данный тип запроса не поддерживается." };
                    default:
                        return new Response { ReturnCode = 100, ErrorText = "Данный тип запроса не поддерживается.", ViewErrorText = "Данный тип запроса не поддерживается." };
                }
            }


            return new Response { ReturnCode = 120, ErrorText = "Данный запрос не поддерживается.", ViewErrorText = "Данный запрос не поддерживается." };
        }

        protected virtual void OnNewTransfer(ParcerNewTrasferEventArgs e)
        {
            if (NewTransfer != null)
                NewTransfer(this, e);
        }

        /// <summary>
        /// Процедура заполнения беревода без открытия счета
        /// </summary>
        /// <param name="transfer">Перевод</param>
        /// <param name="xmlRecord">XML запрос</param>
        private void FillTransfer(ContactTransfer transfer, XmlNode xmlRecord)
        {
            FillSection("trn", ref transfer, xmlRecord);
            FillSectionFees("trn", ref transfer.Fee, xmlRecord);
            FillSectionPerson("s", ref transfer.Sender, xmlRecord);
            FillSectionAddress("s", ref transfer.Sender.Address, xmlRecord);
            FillSectionIds("s", ref transfer.Sender.Id, xmlRecord);
            FillSectionPerson("b", ref transfer.Resiver, xmlRecord);
            FillSectionAddress("b", ref transfer.Resiver.Address, xmlRecord);
            FillSectionIds("b", ref transfer.Resiver.Id, xmlRecord);
        }

        public ContactResponse GetResponse(XmlNode requestXml, bool childNode = true)
        {
            var obj = new ContactResponse();

            var request = childNode ? requestXml.ChildNodes[0] : requestXml;


            foreach (var property in obj.GetType().GetFields())
            {
                if (request.Attributes != null)
                    foreach (var attribute in request.Attributes)
                    {
                        if (((XmlAttribute)attribute).Name == property.Name)
                            SetValueForProperty(property, obj, ((XmlAttribute)attribute).Value);                          
                    }
            }

            return obj;
        }

        public Request GetRequest(string data)
        {
            var requestXml = new XmlDocument();
            requestXml.LoadXml(data);
            var obj = new Request();

                  
            foreach (var property in obj.GetType().GetFields())
            {
                if (requestXml.ChildNodes[0].Attributes != null)
                    foreach (var attribute in requestXml.ChildNodes[0].Attributes)
                    {

                        if (((XmlAttribute) attribute).Name == property.Name)
                        {
                            if (property.FieldType == typeof(string))
                                property.SetValue(obj, ((XmlAttribute) attribute).Value);
                            else if (property.FieldType == typeof(int))
                                property.SetValue(obj, Convert.ToInt32(((XmlAttribute)attribute).Value));
                        }
                    }
            }

            return obj;
        }

        private void FillSection(string code, ref ContactTransfer transfer, XmlNode requestXml)
        {
            foreach (var property in transfer.GetType().GetFields())
                foreach (var childNode in
                    requestXml.ChildNodes.Cast<XmlNode>().Where(childNode => childNode.Name == (code + property.Name) && childNode.ChildNodes.Count > 0))
                        SetValueForProperty(property, transfer, childNode.ChildNodes[0].Value);
        }

        private void FillSectionPerson(string code, ref ContactPerson sender, XmlNode requestXml)
        {
            foreach (var property in sender.GetType().GetFields())
                foreach (var childNode in
                    requestXml.ChildNodes.Cast<XmlNode>().Where(childNode => childNode.Name == (code + property.Name) && childNode.ChildNodes.Count > 0))
                    SetValueForProperty(property, sender, childNode.ChildNodes[0].Value);
        }

        private void FillSectionAddress(string code, ref PersonAddress address, XmlNode requestXml)
        {
            foreach (var property in address.GetType().GetFields())
                foreach (var childNode in
                    requestXml.ChildNodes.Cast<XmlNode>().Where(childNode => childNode.Name == (code + property.Name) && childNode.ChildNodes.Count > 0))
                    SetValueForProperty(property, address, childNode.ChildNodes[0].Value);
        }

        private void FillSectionIds(string code, ref PersonId id, XmlNode requestXml)
        {
            foreach (var property in id.GetType().GetFields())
                foreach (var childNode in
                    requestXml.ChildNodes.Cast<XmlNode>().Where(childNode => childNode.Name == (code + property.Name) && childNode.ChildNodes.Count > 0))
                    SetValueForProperty(property, id, childNode.ChildNodes[0].Value);
        }

        private void FillSectionFees(string code, ref Fees fee, XmlNode requestXml)
        {
            foreach (var property in fee.GetType().GetFields())
                foreach (var childNode in
                    requestXml.ChildNodes.Cast<XmlNode>().Where(childNode => childNode.Name == (code + property.Name) && childNode.ChildNodes.Count > 0))
                    SetValueForProperty(property, fee, childNode.ChildNodes[0].Value);
        }

        public void SetValueForProperty(FieldInfo property, object obj, string value)
        {
            if (property.FieldType == typeof(string))
                property.SetValue(obj, (value));
            else if (property.FieldType == typeof(int))
                property.SetValue(obj, Convert.ToInt32(value));
            else if (property.FieldType == typeof(bool))
                property.SetValue(obj, value == "1");
            else if (property.FieldType == typeof(decimal))
                property.SetValue(obj, Convert.ToDecimal(value.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)));
            else if (property.FieldType == typeof(DateTime))
                property.SetValue(obj, new DateTime(
                                                           Convert.ToInt16(value.Substring(0, 4)),
                                                           Convert.ToInt16(value.Substring(4, 2)),
                                                           Convert.ToInt16(value.Substring(6, 2))
                                                        ));
        }
    } 
    
}
