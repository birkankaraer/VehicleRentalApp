using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleRentalApp.Data;
using VehicleRentalApp.Models;

namespace VehicleRentalApp.Controllers
{
    public class VehicleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VehicleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Vehicle/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vehicle/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VehicleName,PlateNumber")] Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                var existingvehicle = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.VehicleName == vehicle.VehicleName);

                var existingplatenumber = await _context.Vehicles
                    .FirstOrDefaultAsync(pn => pn.PlateNumber == vehicle.PlateNumber);

                if (existingvehicle != null)
                {
                    ModelState.AddModelError("VehicleName", "Bu araç zaten mevcut");
                    return View(vehicle);
                }

                if (existingplatenumber != null)
                {
                    ModelState.AddModelError("VehicleName", "Bu plaka zaten mevcut");
                    return View(vehicle);
                }


                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }

        // GET: Vehicle/Index
        public async Task<IActionResult> Index()
        {
            var vehicles = await _context.Vehicles.ToListAsync();
            foreach (var vehicle in vehicles)
            {
                vehicle.IdleHours = (7 * 24) - (vehicle.ActiveWorkingHours + vehicle.MaintenanceHours);
            }
            return View(vehicles);
        }

        [HttpGet]
        public async Task<IActionResult> GetVehicles()
        {
            var vehicles = await _context.Vehicles.ToListAsync();
            return Json(vehicles);
        }


        // GET: Vehicle/Edit/5
        [Authorize(Roles = "User")] // Sadece "User" rolündeki kullanıcılar erişebilir
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            return View(vehicle);
        }

        // POST: Vehicle/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ActiveWorkingHours,MaintenanceHours")] Vehicle vehicle)
        {
            if (id != vehicle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingVehicle = await _context.Vehicles.FindAsync(id);

                    if (existingVehicle == null)
                    {
                        return NotFound();
                    }

                    existingVehicle.ActiveWorkingHours = vehicle.ActiveWorkingHours;
                    existingVehicle.MaintenanceHours = vehicle.MaintenanceHours;

                    _context.Update(existingVehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }

        private bool VehicleExists(int id)
        {
            return _context.Vehicles.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Graph()
        {
            var vehicles = await _context.Vehicles.ToListAsync();

            var data = vehicles.Select(v => new
            {
                Name = v.VehicleName,
                ActiveWorkingHours = v.ActiveWorkingHours,
                MaintenanceHours = v.MaintenanceHours
            }).ToList();

            return View(data);
        }
    }
}