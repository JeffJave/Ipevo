using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APILibrary.Model.UPS.Response
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class ResponseStatus
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class Alert
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class Response
    {
        public ResponseStatus ResponseStatus { get; set; }
        public List<Alert> Alert { get; set; }
    }

    public class Type
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class UnitOfMeasurement
    {
        public string Code { get; set; }
    }

    public class Factor
    {
        public string Value { get; set; }
        public UnitOfMeasurement UnitOfMeasurement { get; set; }
    }

    public class Rate
    {
        public Type Type { get; set; }
        public Factor Factor { get; set; }
    }

    public class Weight
    {
        public string Value { get; set; }
        public UnitOfMeasurement UnitOfMeasurement { get; set; }
    }

    public class AdjustedWeight
    {
        public string Value { get; set; }
        public UnitOfMeasurement UnitOfMeasurement { get; set; }
    }

    public class Commodity
    {
        public string Description { get; set; }
        public Weight Weight { get; set; }
        public AdjustedWeight AdjustedWeight { get; set; }
    }

    public class TotalShipmentCharge
    {
        public string CurrencyCode { get; set; }
        public string MonetaryValue { get; set; }
    }

    public class BillableShipmentWeight
    {
        public string Value { get; set; }
        public UnitOfMeasurement UnitOfMeasurement { get; set; }
    }

    public class DimensionalWeight
    {
        public string Value { get; set; }
        public UnitOfMeasurement UnitOfMeasurement { get; set; }
    }

    public class Service
    {
        public string Code { get; set; }
    }

    public class AlternateRateType
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class AlternateRatesResponse
    {
        public AlternateRateType AlternateRateType { get; set; }
        public List<Rate> Rate { get; set; }
        public BillableShipmentWeight BillableShipmentWeight { get; set; }
    }

    public class RatingSchedule
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }

    public class TimeInTransit
    {
        public string DaysInTransit { get; set; }
    }

    public class FreightRateResponse
    {
        public Response Response { get; set; }
        public List<Rate> Rate { get; set; }
        public Commodity Commodity { get; set; }
        public TotalShipmentCharge TotalShipmentCharge { get; set; }
        public BillableShipmentWeight BillableShipmentWeight { get; set; }
        public DimensionalWeight DimensionalWeight { get; set; }
        public Service Service { get; set; }
        public AlternateRatesResponse AlternateRatesResponse { get; set; }
        public RatingSchedule RatingSchedule { get; set; }
        public TimeInTransit TimeInTransit { get; set; }
    }

    public class FreightRateResponseRoot
    {
        public FreightRateResponse FreightRateResponse { get; set; }
    }


}
