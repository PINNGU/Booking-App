﻿using BookingApp.Domain.Model;
using BookingApp.Domain.RepositoryInterfaces;
using BookingApp.DTOs;
using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.View.TouristView;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static BookingApp.Resources.Enums;

namespace BookingApp.ApplicationServices
{
    public class SimpleRequestService
    {
        private ISimpleRequestRepository _simpleRequestRepository;

        public SimpleRequestService(ISimpleRequestRepository simpleRequestRepository)
        {
            _simpleRequestRepository = simpleRequestRepository;
        }

        public SimpleRequestService()
        {
            _simpleRequestRepository = new SimpleRequestRepository();

        }

        public static SimpleRequestService GetInstance()
        {
            return App.ServiceProvider.GetRequiredService<SimpleRequestService>();
        }

        public List<TourRequest> GetAll()
        {
            return _simpleRequestRepository.GetAll();
        }

        public List<TourRequest> GetByTouristId(int touristId)
        {
            return _simpleRequestRepository.GetAll().Where(r => r.TouristId == touristId).ToList();
        }
        public List<TourRequest> GetByTouristIdByYear(int touristId, int year)
        {
            return _simpleRequestRepository.GetAll().Where(r => r.TouristId == touristId && r.StartDate.Year == year).ToList();
        }
        public TourRequest Save(TourRequest request)
        {
            return _simpleRequestRepository.Save(request);
        }
        private void SaveRequestGuests(int requestId, List<TourGuestDTO> guests, User loggedUser)
        {
            foreach (TourGuestDTO guest in guests)
            {
                TourGuests newGuest = new TourGuests(guest,-1, requestId);
                TourGuestService.GetInstance().Save(newGuest);
            }
        }

        public void MakeTourRequest(SimpleRequestDTO request, List<TourGuestDTO> guests, User user)
        {
            TourRequest simpleRequest = new TourRequest(request.locationId, request.languageId, request.Description, request.Start, request.End, request.TouristId, request.Status);
            Save(simpleRequest);
            SaveRequestGuests(simpleRequest.Id, guests, user);
        }

        public void CheckRequestStatus()
        {
            DateOnly currentDate = DateOnly.FromDateTime(DateTime.Now);
            DateOnly tresholdDate = currentDate.AddDays(2);

            foreach (TourRequest request in GetAll())
            {
                if (request.StartDate < tresholdDate)
                {
                    request.Status = Resources.Enums.RequestStatus.Invalid;
                    _simpleRequestRepository.Update(request);
                }
            }
            
        }
        public TourRequest GetById(int id)
        {
            return _simpleRequestRepository.GetById(id);
        }

        public List<TourRequest> GetAccepted(int userId)
        {
            return GetByTouristId(userId).Where(r => r.Status == RequestStatus.Accepted).ToList();
        }

        public List<TourRequest> GetAcceptedByYear(int userId, int year)
        {
            return GetAccepted(userId).Where(r => r.StartDate.Year == year).ToList();
        }
        public double GetAcceptedPercentageByYear(int userId, int year)
        {
            return (double)GetAcceptedByYear(userId, year).Count() / GetByTouristIdByYear(userId, year).Count() * 100.0; 
        }

        public double GetAllTimeAcceptedPercentage(int userId)
        {
            return (double)GetAccepted(userId).Count() / GetByTouristId(userId).Count() * 100.0;
        }

        public List<int> GetRequestYears(int userId)
        {
            return GetByTouristId(userId).Select(r => r.StartDate.Year).Distinct().ToList();
        }
        public List<string> GetRequestLangugaes(int userId)
        {
            List<TourRequest> userRequests = GetByTouristId(userId);
            List<int> languageIds = userRequests.Select(r => r.LanguageId).Distinct().ToList();
            List<string> distinctLanguages = new List<string>();

            foreach (int languageId in languageIds)
            {
                string languageName = LanguageService.GetInstance().GetNameById(languageId);
                distinctLanguages.Add(languageName);
            }

            return distinctLanguages;
        }
    

        public int GetAcceptedPeopleNumber(int userId)
        {
            int count = 0;
            foreach(TourRequest request in GetAccepted(userId))
            {
                count += TourGuestService.GetInstance().GetGuestsCountByRequest(request.Id);
            }
            return count;
        }
        public int GetAcceptedPeopleNumberByYear(int userId, int year)
        {
            int count = 0;
            foreach (TourRequest request in GetAcceptedByYear(userId, year))
            {
                count += TourGuestService.GetInstance().GetGuestsCountByRequest(request.Id);
            }
            return count;
        }

        public double GetAverageAccepetedPeople(int userId)
        {
            return (double)GetAcceptedPeopleNumber(userId) / GetAccepted(userId).Count(); 
        }
        public double GetAverageAccepetedPeopleByYear(int userId, int year)
        {
            return (double)GetAcceptedPeopleNumberByYear(userId, year) / GetAcceptedByYear(userId, year).Count();
        }
        public Dictionary<Location, int> GetRequestsByLocations(int userId)
        {
            Dictionary<Location, int> pairs = new Dictionary<Location, int>();

            foreach (Location location in LocationService.GetInstance().GetAllByUserRequest(userId).Distinct())
                pairs.Add(location, GetRequestNumberByLocation(location, userId));

            return pairs;
        }
        public Dictionary<string, int> GetRequestsByLanguages(int userId)
        {
            Dictionary<string, int> pairs = new Dictionary<string, int>();

            foreach (Language language in LanguageService.GetInstance().GetAllByUserRequest(userId).Distinct())
                pairs.Add(language.Name, GetRequestNumberByLanguage(language, userId));

            return pairs;
        }
        public int GetRequestNumberByLanguage(Language language, int userId)
        {
            return GetByTouristId(userId).Where(r => r.LanguageId == language.Id).Count();
        }

        public int GetRequestNumberByLocation(Location location, int userId)
        {
            return GetByTouristId(userId).Where(r => r.LocationId == location.Id).Count();
        }
    }
}
