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
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }

        // GET: Vehicle/Index
        // Hem Admin hem User rolü erişebilir
        public async Task<IActionResult> Index()
        {
            // Tüm araçları getir
            var vehicles = await _context.Vehicles.ToListAsync();

            // Her araç için boşta bekleme süresini hesapla
            foreach (var vehicle in vehicles)
            {
                vehicle.IdleHours = (7 * 24) - (vehicle.ActiveWorkingHours + vehicle.MaintenanceHours);
            }

            return View(vehicles); // View'e gönder
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

                    // Sadece Aktif Çalışma ve Bakım Süresi güncelleniyor
                    existingVehicle.ActiveWorkingHours = vehicle.ActiveWorkingHours;
                    existingVehicle.MaintenanceHours = vehicle.MaintenanceHours;

                    // Değişiklikleri kaydet
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
                return RedirectToAction(nameof(Index)); // Listeye yönlendirme
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
                IdleHours = (7 * 24) - (v.ActiveWorkingHours + v.MaintenanceHours)
            }).ToList();

            return View(data); // Veriyi View'e gönder
        }

    }
}
