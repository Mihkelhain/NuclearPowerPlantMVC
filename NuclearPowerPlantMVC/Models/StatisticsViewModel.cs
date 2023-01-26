namespace NuclearPowerPlantMVC.Models
{
    public class PlantStatusModel
    {
        public string Name { get; set; }
        public string Status { get; set; }
    }

    public class StatisticsViewModel
    {
        public double CurrentWaterConsumption { get; set; }
        public TimeSpan TotalUptimeSum { get; set; }
        public List<PlantStatusModel> Workorder { get; set; } = new();
    }
}
