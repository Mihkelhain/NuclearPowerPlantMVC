using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuclearPowerPlantMVC.Data;
using NuclearPowerPlantMVC.Models;

namespace NuclearPowerPlantMVC.Controllers
{
	[Authorize]
	public class PlantsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlantsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
			var nuclearPlants = await _context.NuclearPlants.ToListAsync();
			foreach (NuclearPlant nuclearPlant in nuclearPlants)
				nuclearPlant.Reactors = _context.Reactors.Where(x => x.NuclearPlant.Id == nuclearPlant.Id).ToList();
			return View(nuclearPlants);
		}

		public IActionResult AddPlant()
		{
			return View();
		}

		[HttpPost]
        public async Task<IActionResult> AddPlant(NuclearPlant model)
        {
			if (model?.Name == null || model?.Name.Length < 1 || model?.Name.Length > 20) return View(model);
            await _context.NuclearPlants.AddAsync(model);
			await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

		public async Task<IActionResult> AddReactor(int id)
		{
			var plant = await _context.NuclearPlants.FindAsync(id);
			if (plant == null) return NotFound();
			var reactor = new Reactor { NuclearPlant = plant };
			return View(reactor);
		}

		[HttpPost]
		public async Task<IActionResult> AddReactor(int id, Reactor reactor)
		{
			var plant = await _context.NuclearPlants.Where(x => x.Id == id).FirstOrDefaultAsync();
			if (plant == null) return NotFound();
			reactor.NuclearPlant = plant;
			if (reactor.EnergyProduction < 1) return View(reactor);
			plant?.Reactors?.Add(new Reactor { EnergyProduction = reactor.EnergyProduction });
			if (plant != null)
			{
				_context.Update(plant);
				_context.SaveChanges();
			}
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> DeletePlant(int id)
		{
			var plant = await _context.NuclearPlants.Where(x => x.Id == id).FirstOrDefaultAsync();
			if (plant == null) return NotFound();
			plant.Reactors = await _context.Reactors.Where(x => x.NuclearPlant.Id == plant.Id).ToListAsync();
			return View(plant);
		}

		[HttpPost]
		public async Task<IActionResult> DeletePlantConfirmed(int id)
		{
			var plant = await _context.NuclearPlants.Where(x => x.Id == id).FirstOrDefaultAsync();
			if (plant == null) return NotFound();
			_context.Remove(plant);
			var reactors = await _context.Reactors.Where(x => x.NuclearPlant.Id == plant.Id).ToListAsync();
			foreach (Reactor reactor in reactors)
				_context.Remove(reactor);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> DeleteReactor(int id)
		{
			var reactor = await _context.Reactors.Where(x => x.Id == id).FirstOrDefaultAsync();
			if (reactor == null) return NotFound();
			return View(reactor);
		}

		[HttpPost]
		public async Task<IActionResult> DeleteReactorConfirmed(int id)
		{
			var reactor = await _context.Reactors.FindAsync(id);
			if (reactor != null)
				_context.Remove(reactor);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> EditPlantName(int id)
		{
			var plant = await _context.NuclearPlants.Where(x => x.Id == id).FirstOrDefaultAsync();
			if (plant == null) return NotFound();
			return View(plant);
		}

		[HttpPost]
		public async Task<IActionResult> EditPlantName(int id, string name)
		{
			var plant = await _context.NuclearPlants.Where(x => x.Id == id).FirstOrDefaultAsync();
			if (plant == null) return NotFound();
			plant.Name = name;
			_context.Update(plant);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}

		public async Task<IActionResult> EditReactorProduction(int id)
		{
			var reactor = await _context.Reactors.Where(x => x.Id == id).FirstOrDefaultAsync();
			if (reactor == null) return NotFound();
			return View(reactor);
		}

		[HttpPost]
		public async Task<IActionResult> EditReactorProduction(int id, double energyProduction)
		{
			var reactor = await _context.Reactors.Where(x => x.Id == id).FirstOrDefaultAsync();
			if (reactor == null) return NotFound();
			if (energyProduction < 1) return View(reactor);
			reactor.EnergyProduction = energyProduction;
			_context.Update(reactor);
			await _context.SaveChangesAsync();
			return RedirectToAction("Index");
		}
	}
}
