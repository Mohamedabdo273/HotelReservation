﻿using Infrastructures.Repository.IRepository;
using Infrastructures.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Utilities.Utility;

namespace HotelReservation.Areas.Company.Controllers
{
    [Area("Company")]
    [Authorize(Roles = SD.CompanyRole)]
    public class HotelAmenityController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public HotelAmenityController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: AmenityController
        public ActionResult Index(int id)
        {
            try
            {
                Hotel amenities;
                if (id != 0)
                {
                    Response.Cookies.Append("HotelIdCookie", id.ToString());
                    
                }
                else
                {
                    id = int.Parse(Request.Cookies["HotelIdCookie"]);
                    
                }
                amenities = unitOfWork.HotelRepository.HotelsWithAmenities(id);
                return View(amenities);
            }
            catch
            {
                return RedirectToAction("NotFound", "Home", new { area = "Customer" });
            }
        }

        // GET: AmenityController/Create
        public ActionResult Create(int hotelId)
        {
            try
            {
                var amenity = unitOfWork.AmenityRepository.Get();
                var hotelAmenities = unitOfWork.HotelAmenitiesRepository.Get(where: h => h.HotelId == hotelId).Select(a => a.AmenityId).ToList();
                ViewBag.Amenity = amenity;
                ViewBag.HotelAmenities = hotelAmenities;
                ViewBag.HotelId = hotelId;
                return View();
            }
            catch
            {
                return RedirectToAction("NotFound", "Home", new { area = "Customer" });
            }
        }

        // POST: AmenityController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(int HotelId, List<int> amenitiesId)
        {
            try
            {
                if (HotelId != 0 && amenitiesId.Count != 0)
                {
                    var toDelete = unitOfWork.HotelAmenitiesRepository.Get(where: h => h.HotelId == HotelId);
                    unitOfWork.HotelAmenitiesRepository.DeleteRange(toDelete);
                    foreach (var item in amenitiesId)
                    {
                        var amenity = new HotelAmenities()
                        {
                            HotelId = HotelId,
                            AmenityId = item,
                        };
                        unitOfWork.HotelAmenitiesRepository.Create(amenity);
                    }
                    unitOfWork.Complete();
                    return RedirectToAction(nameof(Index));
                }
                if (amenitiesId.Count == 0)
                {
                    var toDelete = unitOfWork.HotelAmenitiesRepository.Get(where: h => h.HotelId == HotelId);
                    if (toDelete.Any())
                    {
                        unitOfWork.HotelAmenitiesRepository.DeleteRange(toDelete);
                        unitOfWork.Complete();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction("NotFound", "Home", new { area = "Customer" });
            }
        }

        // POST: AmenityController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int amenityId, int hotelId)
        {
            try
            {
                var hotelAmenities = new HotelAmenities { AmenityId = amenityId, HotelId = hotelId };
                unitOfWork.HotelAmenitiesRepository.Delete(hotelAmenities);
                unitOfWork.Complete();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction("NotFound", "Home", new { area = "Customer" });
            }
        }
    }
}