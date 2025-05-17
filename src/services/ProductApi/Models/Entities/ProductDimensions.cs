using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ProductApi.Models.Entities;

public class ProductDimensions
{
    [BsonElement("length")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal? Length { get; set; }

    [BsonElement("width")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal? Width { get; set; }

    [BsonElement("height")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal? Height { get; set; }
}