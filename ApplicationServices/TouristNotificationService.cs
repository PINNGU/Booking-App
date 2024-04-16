﻿using BookingApp.Domain.Model;
using BookingApp.Domain.RepositoryInterfaces;
using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.RepositoryInterfaces;
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
                TouristNotification notification = new TouristNotification(reservation.TouristId, TourService.GetInstance().GetById(tourSchedule.TourId).Name);
                List<TourGuests> guests = TourGuestService.GetInstance().GetAllByReservationId(reservation.Id);

                foreach (var guest in guests)
                {
                    if (guests.Any())
                        notification.Message += guest.Name + " " + guest.Surname + " " + guest.CheckpointId + "; ";
                }

                if (guests.Find(g => g.UserTypeId != -1) != null)
                    _notificationRepository.Save(notification);
            }
        }
    }
}
