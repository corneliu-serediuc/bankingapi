using System;
using System.Text.Json.Serialization;

namespace BankingApi.Entities;

public class Entity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [JsonIgnore]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [JsonIgnore]
    public DateTime? UpdatedAt { get; set; } = null;
}
