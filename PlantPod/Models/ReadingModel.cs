using System;

namespace PlantPod.Models
{
    public class ReadingModel
    {
        public int Id { get; set; }
        public int Arduino_id { get; set; }
        public decimal ReadingValue { get; set; }
        public DateTime Post_date { get; set; }
        public string Sensor_type { get; set; }
    }
}