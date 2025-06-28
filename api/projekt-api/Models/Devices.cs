namespace projekt_api.Models
{
public class Devices
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Owner { get; set; }
    public bool Status { get; set; }
    public bool IsCar { get; set; }
}
}

// AZURE CONTAINTER INSTANCES

// "PartitionKey": parking_spot,
//         "RowKey": str(datetime.utcnow().timestamp()).replace('.', ''),  # unikalny klucz
//         "TablicaRejestracyjna": license_plate,
//         "Zajete": status,
//         "CzasPostoju": duration_string,
//         "TimestampISO": timestamp_iso