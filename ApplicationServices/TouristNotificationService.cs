﻿using BookingApp.Domain.Model;
using BookingApp.Domain.RepositoryInterfaces;
using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.RepositoryInterfaces;
using BookingApp.Resources;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.ApplicationServices
{
    class TouristNotificationService
    {
        private ITourisNotificationRepository _notificationRepository;

        public TouristNotificationService(ITourisNotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public TouristNotificationService()
        {
            _notificationRepository = new TourisNotificationRepository();

        }

        public static TouristNotificationService GetInstance()
        {
            return App.ServiceProvider.GetRequiredService<TouristNotificationService>();
        }

        public List<TouristNotification> GetAll()
        {
            return _notificationRepository.GetAll();
        }
        public void SendNotification(TourSchedule tourSchedule)
        {
            foreach(var reservation in TourReservationService.GetInstance().GetAllByRealisationId(tourSchedule.Id))
            {
                string message = "Following guests have shown up on these checkpoints: ";
                TouristNotification notification = new TouristNotification(message,reservation.TouristId, TourService.GetInstance().GetById(tourSchedule.TourId).Name, Resources.Enums.NotificationType.Attendance);
                List<TourGuests> guests = TourGuestService.GetInstance().GetAllByReservationId(reservation.Id);

                foreach (var guest in guests)
                {
                    if (guests.Any())
                        notification.Message += guest.Name + " " + guest.Surname + " " + CheckpointService.GetInstance().GetById(guest.CheckpointId).Name + "; ";
                }

                if (guests.Find(g => g.UserTypeId != -1) != null)
                    _notificationRepository.Save(notification);
            }
        }

        public void SendRequestNotification(TourSchedule tourSchedule,TourRequest request,int userId)
        {
            Guide guide = GuideService.GetInstance().GetByUserId(userId);
            Tour tour = TourService.GetInstance().GetById(tourSchedule.TourId);
            string message = "Guide " + guide.Name + " " + guide.Surname + "accepted your request and set the date for: " + tourSchedule.Start;
            TouristNotification notification = new TouristNotification(message, request.TouristId, tour.Name, Enums.NotificationType.AcceptedRequest);
            notification.Recieved = DateTime.Now;
            _notificationRepository.Save(notification);
        }

       public void SendStatisticTourNotification(int tourId)
        {
            Tour tour = TourService.GetInstance().GetById(tourId);
            foreach (TourRequest request in SimpleRequestService.GetInstance().GetNotAccepted())
            {
                if (request.LanguageId == tour.LanguageId || request.LocationId == tour.LocationId)
                {
                    Location location = LocationService.GetInstance().GetById(tour.LocationId);
                    string message = "Based on your requests, you may be interested into tour details, location: " + location.City + ", " + location.State + "; " + "language: " + LanguageService.GetInstance().GetById(tour.LanguageId).Name;
                    TouristNotification notification = new TouristNotification(message, request.TouristId, tour.Name, Enums.NotificationType.NewTour);
                    notification.Recieved = DateTime.Now;
                    _notificationRepository.Save(notification);
                }
            }         
        }

        /*public List<Tourist> GetTouristForNotification(Tour tour)
        {
            List<Tourist> tourists = new List<Tourist>();
            foreach (Tourist tourist in TouristService.GetInstance().GetAll())
            {
                foreach (TourRequest request in SimpleRequestService.GetInstance().GetNotAccepted(tourist.Id))
                {
                    if (request.LocationId == tour.LocationId || request.LanguageId == tour.LanguageId)
                    {
                        tourists.Add(tourist);
                        break;
                    }
                }
            }
            return tourists;
        }*/
    }
}
