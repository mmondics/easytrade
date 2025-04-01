using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using EasyTrade.BrokerService.Helpers;
using EasyTrade.BrokerService.Entities.Prices; // For Price class
using EasyTrade.BrokerService.Entities.Prices.ServiceConnector; // For IPriceServiceConnector interface

namespace EasyTrade.BrokerService.Entities.Prices
{
    public class Price
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int InstrumentId { get; set; }

        // Apply the custom DateTimeConverter to handle DateTime during JSON serialization
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime Timestamp { get; set; }

        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }

        public Price() { }

        public Price(
            int instrumentId,
            DateTime timestamp,
            decimal open,
            decimal high,
            decimal low,
            decimal close
        )
        {
            InstrumentId = instrumentId;
            Timestamp = timestamp;
            Open = open;
            High = high;
            Low = low;
            Close = close;
        }

        public PriceEntity ToEntity()
        {
            return new PriceEntity
            {
                InstrumentId = this.InstrumentId,
                Timestamp = this.Timestamp,
                Open = this.Open,
                High = this.High,
                Low = this.Low,
                Close = this.Close
            };
        }
    }
    
    // Example of a PriceEntity class, adjust as per your need
    public class PriceEntity
    {
        public int InstrumentId { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
    }
}
