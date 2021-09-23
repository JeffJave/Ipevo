using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Model.UPS.Request
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Address
    {
        public string AddressLine { get; set; }
        public string City { get; set; }
        public string StateProvinceCode { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public string ResidentialAddressIndicator { get; set; }
    }

    public class Phone
    {
        public string Number { get; set; }
        public string Extension { get; set; }
    }

    public class ShipFrom
    {
        public string Name { get; set; }
        public Address Address { get; set; }
        public string AttentionName { get; set; }
        public Phone Phone { get; set; }
        public string EMailAddress { get; set; }
    }

    public class ShipTo
    {
        public string Name { get; set; }
        public Address Address { get; set; }
        public string AttentionName { get; set; }
        public Phone Phone { get; set; }
    }

    public class Payer
    {
        public string Name { get; set; }
        public Address Address { get; set; }
        public string ShipperNumber { get; set; }
        public string AccountType { get; set; }
        public string AttentionName { get; set; }
        public Phone Phone { get; set; }
        public string EMailAddress { get; set; }
    }

    public class ShipmentBillingOption
    {
        public string Code { get; set; }
    }

    public class PaymentInformation
    {
        public Payer Payer { get; set; }
        public ShipmentBillingOption ShipmentBillingOption { get; set; }
    }

    public class Service
    {
        public string Code { get; set; }
    }

    public class UnitOfMeasurement
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class Weight
    {
        public UnitOfMeasurement UnitOfMeasurement { get; set; }
        public string Value { get; set; }
    }

    public class Dimensions
    {
        public UnitOfMeasurement UnitOfMeasurement { get; set; }
        public string Length { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
    }

    public class PackagingType
    {
        public string Code { get; set; }
    }

    public class Commodity
    {
        public string Description { get; set; }
        public Weight Weight { get; set; }
        public Dimensions Dimensions { get; set; }
        public string NumberOfPieces { get; set; }
        public PackagingType PackagingType { get; set; }
        public string FreightClass { get; set; }
    }

    public class AlternateRateOptions
    {
        public string Code { get; set; }
    }

    public class PickupRequest
    {
        public string PickupDate { get; set; }
    }

    public class GFPOptions
    {
        public string GPFAccesorialRateIndicator { get; set; }
    }

    public class FreightRateRequest
    {
        public ShipFrom ShipFrom { get; set; }
        public string ShipperNumber { get; set; }
        public ShipTo ShipTo { get; set; }
        public PaymentInformation PaymentInformation { get; set; }
        public Service Service { get; set; }
        public Commodity Commodity { get; set; }
        public string DensityEligibleIndicator { get; set; }
        public AlternateRateOptions AlternateRateOptions { get; set; }
        public PickupRequest PickupRequest { get; set; }
        public GFPOptions GFPOptions { get; set; }
        public string TimeInTransitIndicator { get; set; }
    }

    public class FreightRateRequestRoot
    {
        public FreightRateRequest FreightRateRequest { get;set;}
    }
}
