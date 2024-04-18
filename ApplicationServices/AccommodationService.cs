﻿using BookingApp.Domain.RepositoryInterfaces;
using BookingApp.DTOs;
using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.RepositoryInterfaces;
using BookingApp.Resources;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.ApplicationServices
{
    public class AccommodationService
    {
        public IAccommodationRepository AccommodationRepository;


        public LocationService locationService;


        public AccommodationService(IAccommodationRepository accommodationRepository) 
        {
            AccommodationRepository = accommodationRepository;
        }


        public static AccommodationService GetInstance()
        {
            return App.ServiceProvider.GetRequiredService<AccommodationService>();
        }

        public int GetTotalReservationCount(int id)
        {
            int total = 0;
            Accommodation temp = new Accommodation();
            foreach (AccommodationReservation reservation in AccommodationReservationService.GetInstance().GetAll())
            {
                temp = GetByReservationId(reservation.AccommodationId);
                if (temp.OwnerId == id && reservation.Status == Enums.ReservationStatus.Active)
                {
                    total++;
                }
            }

            return total;
        }

        public int GetReservationCountForAccommodation(int accId,int year)
        {
            int total = 0;
            Accommodation temp = new Accommodation();
            foreach (AccommodationReservation reservation in AccommodationReservationService.GetInstance().GetAll())
            {
                temp = GetByReservationId(reservation.AccommodationId);
                if (temp.Id == accId && reservation.Status == Enums.ReservationStatus.Active && reservation.CheckOutDate.Year == year)
                {
                    total++;
                }
            }

            return total;
        }

        public int GetChangesCountForAccommodation(int accId,int year)
        {
            int total = 0;
            Accommodation temp = new Accommodation();
            foreach (AccommodationReservation reservation in AccommodationReservationService.GetInstance().GetAll())
            {
                temp = GetByReservationId(reservation.AccommodationId);
                if (temp.Id == accId && reservation.Status == Enums.ReservationStatus.Changed && reservation.CheckOutDate.Year == year)
                {
                    total++;
                }
            }

            return total;
        }

        public int GetCancelationCountForAccommodation(int accId,int year)
        {
            int total = 0;
            Accommodation temp = new Accommodation();
            foreach (AccommodationReservation reservation in AccommodationReservationService.GetInstance().GetAll())
            {
                temp = GetByReservationId(reservation.AccommodationId);
                if (temp.Id == accId && reservation.Status == Enums.ReservationStatus.Canceled && reservation.CheckOutDate.Year == year)
                {
                    total++;
                }
            }

            return total;
        }

        public List<int> GatherAllReservationYears(List<ReservationOwnerDTO> reservations)
        {
            List<int> years = new List<int>();
            foreach(ReservationOwnerDTO reservation in reservations)
            {
                if(!years.Contains(reservation.CheckOut.Year))
                {
                    years.Add(reservation.CheckOut.Year);
                }
            }

            years.Sort((a, b) => b.CompareTo(a));
            return years;
        }

        public int GetTotalAccommodationCount(int id)
        {
            int total = 0;
            foreach (Accommodation accommodation in AccommodationRepository.GetAll())
            {
                if (accommodation.OwnerId == id)
                {
                    total++;
                }
            }
            return total;
        }

        public List<Accommodation> GetAll()
        {
            return AccommodationRepository.GetAll();
        }

        public Accommodation Save(Accommodation accommodation)
        {
            return AccommodationRepository.Save(accommodation);
        }

        public List<Accommodation> GetByOwnerId(int id)
        {
            return AccommodationRepository.GetByOwnerId(id);
        }

        public Accommodation GetByReservationId(int id)
        {
            return AccommodationRepository.GetByReservationId(id);
        }

        public bool CheckIfAlreadyBooked(ReservationChanges reservationChange, Accommodation accommodation)
        {
            foreach (AccommodationReservation reservation in AccommodationReservationService.GetInstance().GetAll())
            {
                if (reservation.AccommodationId == accommodation.Id && reservationChange.ReservationId != reservation.Id && DoesDateInterfere(reservation, reservationChange))
                {
                    return true;
                }
            }

            return false;
        }

        public bool DoesDateInterfere(AccommodationReservation oldR, ReservationChanges reservationChange)
        {
            if (reservationChange.NewCheckIn < oldR.CheckInDate && reservationChange.NewCheckOut < oldR.CheckInDate)
                return false;

            if (reservationChange.NewCheckIn > oldR.CheckOutDate && reservationChange.NewCheckOut > oldR.CheckOutDate)
                return false;

            return true;
        }

        public ObservableCollection<ReservationOwnerDTO> GetReservationsForAccommodation(AccommodationOwnerDTO SelectedAccommodation)
        {
            ObservableCollection<ReservationOwnerDTO> ReservationsForAccommodation = new ObservableCollection<ReservationOwnerDTO>();

            foreach (AccommodationReservation reservation in AccommodationReservationService.GetInstance().GetAll())
            {
                if (reservation.AccommodationId == SelectedAccommodation.Id && reservation.Status != Enums.ReservationStatus.Changed)
                {
                    Accommodation accommodation = AccommodationRepository.GetByReservationId(SelectedAccommodation.Id);
                    String userName = GuestService.GetInstance().GetFullname(reservation.GuestId);
                    String imagePath = ImageService.GetInstance().AddMainAccommodationImage(accommodation);
                    Location location = LocationService.GetInstance().GetByAccommodation(accommodation);

                    ReservationOwnerDTO newReservation = new ReservationOwnerDTO(userName, reservation, SelectedAccommodation.Name, location, imagePath);

                    ReservationsForAccommodation.Add(newReservation);
                }
            }

            return ReservationsForAccommodation;
        }

        public Enums.AccommodationType GetType(bool? aptChecked, bool? cottageChecked)
        {
            if (aptChecked == true)
            {
                return Enums.AccommodationType.Apartment;
            }
            else if (cottageChecked == true)
            {
                return Enums.AccommodationType.Cottage;
            }
            else
            {
                return Enums.AccommodationType.House;
            }
        }


    }
}
