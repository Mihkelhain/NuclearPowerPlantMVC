using Microsoft.Build.Framework;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NuclearPowerPlantMVC.Models
{
    public class Reactor
    {
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required]
        [DisplayName("Energy Production (MWh/day)")]
		[Range(1, Double.MaxValue, ErrorMessage = "The field {0} must be greater than {1}.")]
		public double EnergyProduction { get; set; }

        [DisplayName("On/Off")]
        public bool IsOn { get; set; }

        public NuclearPlant? NuclearPlant { get; set; }
    }
}
