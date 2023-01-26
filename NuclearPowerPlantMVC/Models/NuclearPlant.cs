using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NuclearPowerPlantMVC.Models
{
    public class NuclearPlant
    {
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        public string? Name { get; set; }

        [DisplayName("Energy Demand (MWh/day)")]
		[Range(1, Double.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
		public double EnergyDemand { get; set; } = 0;

        public string Status { get; set; } = "Off";

        public DateTime LastTurnedOn { get; set; } = DateTime.Now;

        public List<Reactor>? Reactors { get; set; } = new();

    }
}
