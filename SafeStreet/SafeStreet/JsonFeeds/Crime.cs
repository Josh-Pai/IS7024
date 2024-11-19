﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using CincinnatiCrime;
//
//    var crime = Crime.FromJson(jsonString);

namespace CincinnatiCrime
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Crime
    {
        [JsonProperty("instanceid")]
        public string Instanceid { get; set; }

        [JsonProperty("incident_no")]
        public string IncidentNo { get; set; }

        [JsonProperty("date_reported")]
        public DateTimeOffset DateReported { get; set; }

        [JsonProperty("date_from")]
        public DateTimeOffset DateFrom { get; set; }

        [JsonProperty("date_to")]
        public DateTimeOffset DateTo { get; set; }

        [JsonProperty("clsd")]
        public string Clsd { get; set; }

        [JsonProperty("ucr")]
        public string Ucr { get; set; }

        [JsonProperty("dst")]
        public string Dst { get; set; }

        [JsonProperty("beat")]
        public String Beat { get; set; }

        [JsonProperty("offense")]
        public string Offense { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("theft_code", NullValueHandling = NullValueHandling.Ignore)]
        public string TheftCode { get; set; }

        [JsonProperty("dayofweek")]
        public string Dayofweek { get; set; }

        [JsonProperty("rpt_area")]
        public String RptArea { get; set; }

        [JsonProperty("cpd_neighborhood")]
        public string CpdNeighborhood { get; set; }

        [JsonProperty("weapons")]
        public string Weapons { get; set; }

        [JsonProperty("date_of_clearance")]
        public DateTimeOffset DateOfClearance { get; set; }

        [JsonProperty("hour_from", NullValueHandling = NullValueHandling.Ignore)]
        public string HourFrom { get; set; }

        public int? ParsedHourFrom => int.TryParse(HourFrom, out int hour) ? hour : null;

        [JsonProperty("hour_to")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long HourTo { get; set; }

        [JsonProperty("address_x", NullValueHandling = NullValueHandling.Ignore)]
        public string AddressX { get; set; }

        [JsonProperty("longitude_x", NullValueHandling = NullValueHandling.Ignore)]
        public string LongitudeX { get; set; }

        [JsonProperty("latitude_x", NullValueHandling = NullValueHandling.Ignore)]
        public string LatitudeX { get; set; }

        [JsonProperty("victim_age")]
        public string VictimAge { get; set; }

        [JsonProperty("victim_gender", NullValueHandling = NullValueHandling.Ignore)]
        public string VictimGender { get; set; }

        [JsonProperty("suspect_age")]
        public string SuspectAge { get; set; }

        [JsonProperty("ucr_group")]
        public string UcrGroup { get; set; }

        [JsonProperty("zip")]
        public string Zip { get; set; }

        [JsonProperty("community_council_neighborhood")]
        public string CommunityCouncilNeighborhood { get; set; }

        [JsonProperty("sna_neighborhood")]
        public string SnaNeighborhood { get; set; }

        [JsonProperty("suspect_gender", NullValueHandling = NullValueHandling.Ignore)]
        public string SuspectGender { get; set; }
    }

    public partial class Crime
    {
        public static List<Crime> FromJson(string json) => JsonConvert.DeserializeObject<List<Crime>>(json, CincinnatiCrime.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this List<Crime> self) => JsonConvert.SerializeObject(self, CincinnatiCrime.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            // Handle null values directly
            if (reader.TokenType == JsonToken.Null)
            {
                return t == typeof(long?) ? (long?)null : 0; // Return null for nullable long, otherwise 0
            }

            // Handle integer tokens directly
            if (reader.TokenType == JsonToken.Integer)
            {
                return Convert.ToInt64(reader.Value); // Directly return the number as long
            }

            // Attempt to parse as a string if it’s a string token
            if (reader.TokenType == JsonToken.String)
            {
                var value = serializer.Deserialize<string>(reader);
                if (Int64.TryParse(value, out long l))
                {
                    return l;
                }
            }

            // Log unexpected token type or value for debugging
            throw new Exception($"Cannot unmarshal type long. Unexpected token: {reader.TokenType}, Value: {reader.Value}");
        }


        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }
}
