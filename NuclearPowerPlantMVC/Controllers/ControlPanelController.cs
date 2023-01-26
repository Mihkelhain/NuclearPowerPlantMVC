using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuclearPowerPlantMVC.Data;
using NuclearPowerPlantMVC.Models;

namespace NuclearPowerPlantMVC.Controllers
{
    [Authorize]
    public class ControlPanelController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ControlPanelController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var nuclearPlants = await _context.NuclearPlants.ToListAsync();
			return View(nuclearPlants);
        }

        public async Task<IActionResult> Control(int id)
        {
			var nuclearPlant = _context.NuclearPlants.Where(x => x.Id == id).FirstOrDefault();
            if (nuclearPlant == null) return NotFound();
			nuclearPlant.Reactors = await _context.Reactors.Where(x => x.NuclearPlant.Id == nuclearPlant.Id).ToListAsync();
			return View(nuclearPlant);
		}

        public async Task<IActionResult> ChangeDemand(int id)
        {
            var plant = await _context.NuclearPlants.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (plant == null) return NotFound();
            return View(plant);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeDemand(int id, double energyDemand)
        {
            var plant = await _context.NuclearPlants.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (plant == null) return NotFound();
            plant.Reactors = await _context.Reactors.Where(x => x.NuclearPlant.Id == plant.Id).ToListAsync();
            plant.EnergyDemand = energyDemand;
            _context.Update(plant);

            double totalProduction = plant.Reactors.Where(x => x.IsOn).Sum(x => x.EnergyProduction);
            if (energyDemand > totalProduction)
            {
                foreach (var item in plant.Reactors)
                {
                    item.IsOn = false;
                    _context.Update(item);
                }
                plant.Status = "Overheated";
            }
            else
            {
                plant.Status = plant.Reactors.Count > 0 ? plant.Reactors.Find(x => x.IsOn) != null ? "Normal" : "Off" : "Off";
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Control", new { id });
        }

        public async Task<IActionResult> SwitchReactors(int id)
        {
            var plant = await _context.NuclearPlants.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (plant == null) return NotFound();
            plant.Reactors = await _context.Reactors.Where(x => x.NuclearPlant.Id == plant.Id).ToListAsync();

            double totalProduction = plant.Reactors.Sum(x => x.EnergyProduction);

            if (totalProduction >= plant.EnergyDemand)
            {
                bool setOn = true;
                if ((plant.Reactors[0].IsOn == false && plant.Reactors.Find(x => x.IsOn == true) != null) || (plant.Reactors[0].IsOn == true && plant.Reactors.Find(x => x.IsOn == false) != null) || plant.Reactors.Find(x => x.IsOn == false) == null)
                    setOn = false;
                foreach (var reactor in plant.Reactors)
                {
                    reactor.IsOn = setOn;
                    if (reactor.IsOn)
                        plant.LastTurnedOn = DateTime.Now;
                    _ = (reactor.IsOn) ? plant.Status = "Normal" : plant.Status = "Off";
                }
            }
            else
            {
                plant.Status = "Overheated";
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Control", new { id });
        }

        public async Task<IActionResult> SwitchReactor(int id, int plantid)
        {
            var reactor = await _context.Reactors.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (reactor == null) return NotFound();
            reactor.IsOn ^= true;
            await _context.SaveChangesAsync();
            return RedirectToAction("Control", new { id = plantid });
        }
    }
}
