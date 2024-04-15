﻿using BookingApp.Domain.RepositoryInterfaces;
using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.RepositoryInterfaces;
using BookingApp.Resources;
using BookingApp.Serializer;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static BookingApp.Resources.Enums;

namespace BookingApp.ApplicationServices
{
    public class AccommodationReservationService
    {
        public IAccommodationReservationRepository accommodationReservationRepository;
        private List<DateRanges> _availableDates;
        private List<DateRanges> _bookedDates;
        private DateOnly _firstDate;
        private DateOnly _lastDate;

        public AccommodationReservationService(IAccommodationReservationRepository _accommodationReservationRepository)
        {
            accommodationReservationRepository = _accommodationReservationRepository;
            _availableDates = new List<DateRanges>();
            _bookedDates = new List<DateRanges>();
            _firstDate = new DateOnly();
            _lastDate = new DateOnly();
        }


        public static AccommodationReservationService GetInstance()
        {
            return App.ServiceProvider.GetRequiredService<AccommodationReservationService>();
        }
        public List<AccommodationReservation> GetAll() { 
            return accommodationReservationRepository.GetAll();
        }
        public AccommodationReservation Save(AccommodationReservation reservation)
        {
            return accommodationReservationRepository.Save(reservation);
        }
        public void Delete(AccommodationReservation reservation)
        {
             accommodationReservationRepository.Delete(reservation);
        }
        public AccommodationReservation Update(AccommodationReservation reservation)
        {
            return accommodationReservationRepository.Update(reservation);
        }

        public int GetNumberOfReservation(int guestId)
        {
            int numberOfReservation = 0;
            foreach (AccommodationReservation reservation in GetAll())
                if (reservation.GuestId == guestId && reservation.Status == Enums.ReservationStatus.Active)
                    numberOfReservation++;
            return numberOfReservation;
        }
        public List<AccommodationReservation> GetByAccommodation(Accommodation accommodation)
        {
            List<AccommodationReservation> reservations = GetAll();
            return reservations.FindAll(c => c.AccommodationId == accommodation.Id);
        }
        public AccommodationReservation GetByReservationId(int id)
        {
            List<AccommodationReservation> reservations = GetAll();
            return reservations.Find(c => c.Id == id);
        }
        public List<AccommodationReservation> GetActiveReservationsByGuest(int guestId)
        {
            List<AccommodationReservation> guestReservations = new List<AccommodationReservation>();
           // List<AccommodationReservation> reservations = GetAll();
            foreach (AccommodationReservation reservation in GetAll())
            {
                if (reservation.GuestId == guestId && reservation.Status == ReservationStatus.Active)
                {
                    guestReservations.Add(reservation);
                }
            }
            return guestReservations;
        }
        public List<AccommodationReservation> GetAllReservationsByGuest(int guestId)
        {
            List<AccommodationReservation> guestReservations = new List<AccommodationReservation>();
            foreach (AccommodationReservation reservation in GetAll())
            {
                if (reservation.GuestId == guestId)
                    guestReservations.Add(reservation);
            }
            return guestReservations;
        }
    }
}
