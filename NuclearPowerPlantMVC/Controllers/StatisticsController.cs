using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuclearPowerPlantMVC.Data;
using NuclearPowerPlantMVC.Models;
using System.Numerics;

namespace NuclearPowerPlantMVC.Controllers
{
	public class StatisticsController : Controller
	{
		private readonly ApplicationDbContext _context;
		private static readonly double waterConsumptionPerKWH = 0.220;

		public StatisticsController(ApplicationDbContext context)
		{
			_context = context;
		}

		public async Task<IActionResult> Index()
		{
            var nuclearPlants = await _context.NuclearPlants.ToListAsync();
            foreach (NuclearPlant nuclearPlant in nuclearPlants)
                nuclearPlant.Reactors = _context.Reactors.Where(x => x.NuclearPlant.Id == nuclearPlant.Id).ToList();

            var model = new StatisticsViewModel
			{
				CurrentWaterConsumption = await GetWaterConsumption(),
				TotalUptimeSum = TimeSpan.FromSeconds(nuclearPlants.Sum(x => (x.Reactors.Find(x => x.IsOn) != null) ? DateTime.Now.Subtract(x.LastTurnedOn).TotalSeconds : 0))
			};
			foreach (var plant in nuclearPlants)
			{
				model.Workorder.Add(new PlantStatusModel
				{
					Name = plant.Name,
					Status = plant.Status
				});
			}
			return View(model);
		}

		private async Task<double> GetWaterConsumption()
		{
			double waterConsuption = 0;
			var nuclearPlants = await _context.NuclearPlants.ToListAsync();
			foreach (NuclearPlant nuclearPlant in nuclearPlants)
				nuclearPlant.Reactors = _context.Reactors.Where(x => x.NuclearPlant.Id == nuclearPlant.Id).ToList();
			foreach (var plant in nuclearPlants)
			{
				if (plant.Reactors.Find(x => x.IsOn) != null)
				{
					double timeDays = DateTime.Now.Subtract(plant.LastTurnedOn).TotalSeconds / 86400;
					foreach (var reactor in plant.Reactors.Where(x => x.IsOn))
					{
						// convert MWh/day to KWh by multiplying by 1000
						waterConsuption += waterConsumptionPerKWH * reactor.EnergyProduction * 1000 * timeDays;
					}
				}
			}
			return Math.Round(waterConsuption, 3);
		}
	}
}
