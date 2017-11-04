   /* 
    Licensed under the Apache License, Version 2.0
    
    http://www.apache.org/licenses/LICENSE-2.0
    */
using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace ReCServices.SOAPClass
{
    [XmlRoot(ElementName = "Respuesta")]
    public class Respuesta
    {
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "UnitPlate")]
    public class UnitPlate
    {
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Unitld")]
    public class Unitld
    {
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Latitude")]
    public class Latitude
    {
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Longitude")]
    public class Longitude
    {
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Altitude")]
    public class Altitude
    {
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Odometer")]
    public class Odometer
    {
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "SpeedGps")]
    public class SpeedGps
    {
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Course")]
    public class Course
    {
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Oil")]
    public class Oil
    {
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "Ignition")]
    public class Ignition
    {
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "DateGps")]
    public class DateGps
    {
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "idgps")]
    public class Idgps
    {
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "PanicButton")]
    public class PanicButton
    {
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "item")]
    public class Item
    {
        [XmlElement(ElementName = "Respuesta")]
        public Respuesta Respuesta { get; set; }
        [XmlElement(ElementName = "UnitPlate")]
        public UnitPlate UnitPlate { get; set; }
        [XmlElement(ElementName = "Unitld")]
        public Unitld Unitld { get; set; }
        [XmlElement(ElementName = "Latitude")]
        public Latitude Latitude { get; set; }
        [XmlElement(ElementName = "Longitude")]
        public Longitude Longitude { get; set; }
        [XmlElement(ElementName = "Altitude")]
        public Altitude Altitude { get; set; }
        [XmlElement(ElementName = "Odometer")]
        public Odometer Odometer { get; set; }
        [XmlElement(ElementName = "SpeedGps")]
        public SpeedGps SpeedGps { get; set; }
        [XmlElement(ElementName = "Course")]
        public Course Course { get; set; }
        [XmlElement(ElementName = "Oil")]
        public Oil Oil { get; set; }
        [XmlElement(ElementName = "Ignition")]
        public Ignition Ignition { get; set; }
        [XmlElement(ElementName = "DateGps")]
        public DateGps DateGps { get; set; }
        [XmlElement(ElementName = "idgps")]
        public Idgps Idgps { get; set; }
        [XmlElement(ElementName = "PanicButton")]
        public PanicButton PanicButton { get; set; }
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Type { get; set; }
    }

    [XmlRoot(ElementName = "return")]
    public class Return
    {
        [XmlElement(ElementName = "item")]
        public Item Item { get; set; }
        [XmlAttribute(AttributeName = "type", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "arrayType", Namespace = "http://schemas.xmlsoap.org/soap/encoding/")]
        public string ArrayType { get; set; }
    }

    [XmlRoot(ElementName = "UltimaPosicionResponse", Namespace = "query.php")]
    public class UltimaPosicionResponse
    {
        [XmlElement(ElementName = "return")]
        public Return Return { get; set; }
        [XmlAttribute(AttributeName = "ns1", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Ns1 { get; set; }
    }

    [XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Body
    {
        [XmlElement(ElementName = "UltimaPosicionResponse", Namespace = "query.php")]
        public UltimaPosicionResponse UltimaPosicionResponse { get; set; }
    }

    [XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public class Envelope
    {
        [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public Body Body { get; set; }
        [XmlAttribute(AttributeName = "encodingStyle", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
        public string EncodingStyle { get; set; }
        [XmlAttribute(AttributeName = "SOAP-ENV", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string SOAPENV { get; set; }
        [XmlAttribute(AttributeName = "xsd", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsd { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
        [XmlAttribute(AttributeName = "SOAP-ENC", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string SOAPENC { get; set; }
    }

}
