﻿using BookingApp.DTOs;
using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.Resources;
using BookingApp.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BookingApp.ViewModels
{
    public class AccommodationMenuVM
    {
        public  ObservableCollection<AccommodationOwnerDTO> Accommodations { get; set; }
        public  ObservableCollection<GuestReviewDTO> GuestReviews { get; set; }
        public  ObservableCollection<ReservationOwnerDTO> Reservations { get; set; }
        public ObservableCollection<ReservationChangeDTO> ReservationChanges { get; set; }



        public AccommodationOwnerDTO SelectedAccommodation { get; set; }
        public GuestReviewDTO SelectedGuestReview { get; set; }
        public ReservationOwnerDTO SelectedReservation { get; set; }
        public ReservationChangeDTO SelectedChange { get; set; }

        public OwnerInfoDTO OwnerInfo { get; set; }


        public Owner Owner { get; set; }
        private AccommodationRepository _repository;
        private LocationRepository _locationRepository;
        private ImageRepository _imageRepository;
        private AccommodationReservationRepository _reservationRepository;
        private UserRepository _userRepository;
        private GuestReviewRepository _guestReviewRepository;
        private ReservationChangeRepository _reservationChangeRepository;
        private OwnerRepository _ownerRepository;
        private GuestRepository _guestRepository;
        private OwnerReviewRepository _ownerReviewRepository;


        bool existsNotReviewed = false;
        bool existsCanceled = false;

        public AccommodationMenuVM(Owner owner, LocationRepository _locationRepository, ImageRepository _imageRepository, AccommodationReservationRepository _reservationRepository,
            UserRepository _userRepository, ReservationChangeRepository _reservationChangeRepository, OwnerRepository _ownerRepository,GuestRepository _guestRepository, OwnerReviewRepository _ownerReviewRepository)
        {
            InitiliazeRepositories(_locationRepository, _imageRepository, _reservationRepository, _userRepository, _reservationChangeRepository, _ownerRepository, _guestRepository,_ownerReviewRepository);

            Owner = owner;


            Accommodations = new ObservableCollection<AccommodationOwnerDTO>();
            GuestReviews = new ObservableCollection<GuestReviewDTO>();
            Reservations = new ObservableCollection<ReservationOwnerDTO>();
            ReservationChanges = new ObservableCollection<ReservationChangeDTO>();

            
            Update();
            EntryNotification();
 
        }

        public void EntryNotification() 
        {
            if(existsCanceled && existsNotReviewed)
            {
                MessageBox.Show("You have cancelled reservations and unreviewed guests!", "Notice!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (existsNotReviewed)
            {
                MessageBox.Show("You have unreviewed guests!", "Notice!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (existsCanceled)
            {
                MessageBox.Show("You have cancelled reservations!", "Notice!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public void InitiliazeRepositories(LocationRepository _locationRepository, ImageRepository _imageRepository, AccommodationReservationRepository _reservationRepository, 
            UserRepository _userRepository, ReservationChangeRepository _reservationChangeRepository, OwnerRepository _ownerRepository, GuestRepository _guestRepository, OwnerReviewRepository _ownerReviewRepository)
        {
            this._locationRepository = _locationRepository;
            this._imageRepository = _imageRepository;
            this._reservationRepository = _reservationRepository;
            this._userRepository = _userRepository;
            this._reservationChangeRepository = _reservationChangeRepository;
            this._ownerRepository = _ownerRepository;
            this._guestRepository = _guestRepository;
            this._ownerReviewRepository = _ownerReviewRepository;
            _repository = new AccommodationRepository();
            _guestReviewRepository = new GuestReviewRepository();
        }

        public void Register()
        {
            RegisterAccommodationMenu registerAccommodationMenu = new RegisterAccommodationMenu(_repository, _locationRepository, _imageRepository, Owner.Id);
            registerAccommodationMenu.ShowDialog();
            Update();
        }

        public void Update()
        {
            Accommodations.Clear(); //we must clear so it doesnt duplicate
            GuestReviews.Clear();
            Reservations.Clear();
            ReservationChanges.Clear();

            foreach (Accommodation a in _repository.GetByOwnerId(Owner.Id))
            {
                foreach (AccommodationReservation r in _reservationRepository.GetByAccommodation(a))
                {
                    if (r.Status == Enums.ReservationStatus.Canceled)
                    {
                        existsCanceled = true;
 
                    }
                    CheckReservationStatus(r, a);
                    CheckGuestReview(a, r);
                    UpdateOwner(r);
                }

                string imagePath = AddMainAccommodationImage(a);

                AddChangedReservations(a);

                Accommodations.Add(new AccommodationOwnerDTO(a, _locationRepository.GetByAccommodation(a), imagePath));
            }

            UpdateOwnerStatus();

        }

        public void CheckReservationStatus(AccommodationReservation reservation, Accommodation accommodation)
        {

            AddReservations(reservation, accommodation);

        }

        public bool CheckIfAlreadyBooked(ReservationChanges reservationChange, Accommodation accommodation)
        {
            foreach (AccommodationReservation reservation in _reservationRepository.GetAll())
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

        public void AddChangedReservations(Accommodation accommodation)
        {
            foreach (ReservationChanges reservationChange in _reservationChangeRepository.GetAll())
            {
                if (reservationChange.AccommodationId == accommodation.Id && reservationChange.Status == Enums.ReservationChangeStatus.Pending)
                {

                    String userName = _guestRepository.GetFullname(_reservationRepository.GetAll().Find(r => r.Id == reservationChange.ReservationId).GuestId);
                    String oldDate = reservationChange.OldCheckIn.ToString("dd MMMM yyyy") + "   ->   " + reservationChange.OldCheckOut.ToString("dd MMMM yyyy");
                    String newDate = reservationChange.NewCheckIn.ToString("dd MMMM yyyy") + "   ->   " + reservationChange.NewCheckOut.ToString("dd MMMM yyyy");
                    String bookedStatus = "No";
                    if (CheckIfAlreadyBooked(reservationChange, accommodation))
                        bookedStatus = "Yes";


                    ReservationChangeDTO newChange = new ReservationChangeDTO(reservationChange.ReservationId, userName, accommodation.Name, oldDate, newDate, bookedStatus);

                    ReservationChanges.Add(newChange);
                }

            }


        }


        public void AddReservations(AccommodationReservation reservation, Accommodation accommodation)
        {

            String userName = _guestRepository.GetFullname(reservation.GuestId);
            String imagePath = AddMainAccommodationImage(accommodation);

            ReservationOwnerDTO newReservation = new ReservationOwnerDTO(userName, reservation, accommodation.Name, _locationRepository.GetByAccommodation(accommodation), imagePath);

            Reservations.Add(newReservation);
        }

        public void CheckGuestReview(Accommodation accommodation, AccommodationReservation reservation)
        {
            DateTime GuestCheckout = reservation.CheckOutDate.ToDateTime(TimeOnly.MinValue);
            double DaysPassedForReview = (DateTime.Today - GuestCheckout).TotalDays;

            if (DaysPassedForReview >= 0)
            {
                GuestReview review = _guestReviewRepository.Get(reservation.Id);

                if (review == null && DaysPassedForReview < 5)
                {
                    AddGuestReview(accommodation, reservation);
                    existsNotReviewed = true;
                }
                else if (review != null)
                {
                    AddGuestReview(accommodation, reservation, review.CleanlinessGrade, review.RespectGrade, review.Comment);
                }

            }
        }

        public void AddGuestReview(Accommodation accommodation, AccommodationReservation reservation, int cleanlinessGrade = 0, int respectGrade = 0, string comment = "")
        {
            string GuestOccupationPeriod = reservation.CheckInDate.ToString("dd.MM.yyyy") + " - " + reservation.CheckOutDate.ToString("dd.MM.yyyy");
            GuestReviews.Add(new GuestReviewDTO(accommodation.Name, _guestRepository.GetFullname(reservation.GuestId), cleanlinessGrade, respectGrade, comment, GuestOccupationPeriod, reservation.Id));
        }




        public string AddMainAccommodationImage(Accommodation a)
        {
            Model.Image image = new Model.Image();
            foreach (Model.Image i in _imageRepository.GetByEntity(a.Id, Enums.ImageType.Accommodation))
            {
                image = i;
                return image.Path;
            }
            return null;
        }



        public void DetailedAccommodationView()
        {

            AccommodationDetailedMenu AccommodationDetailedMenu = new AccommodationDetailedMenu(GetImagesForAccommodaton(), GetReservationsForAccommodation(), SelectedAccommodation,_ownerReviewRepository);
            AccommodationDetailedMenu.ShowDialog();

        }

        private ObservableCollection<ReservationOwnerDTO> GetReservationsForAccommodation()
        {
            ObservableCollection<ReservationOwnerDTO> ReservationsForAccommodation = new ObservableCollection<ReservationOwnerDTO>();

            foreach (AccommodationReservation reservation in _reservationRepository.GetAll())
            {
                if (reservation.AccommodationId == SelectedAccommodation.Id && reservation.Status != Enums.ReservationStatus.Changed)
                {
                    Accommodation accommodation = _repository.GetByReservationId(SelectedAccommodation.Id);
                    String userName = _guestRepository.GetFullname(reservation.GuestId);
                    String imagePath = AddMainAccommodationImage(accommodation);
                    Location location = _locationRepository.GetByAccommodation(accommodation);

                    ReservationOwnerDTO newReservation = new ReservationOwnerDTO(userName, reservation, SelectedAccommodation.Name, location, imagePath);

                    ReservationsForAccommodation.Add(newReservation);
                }
            }

            return ReservationsForAccommodation;
        }

        private List<Model.Image> GetImagesForAccommodaton()
        {
            List<Model.Image> images = new List<Model.Image>();

            foreach (Model.Image i in _imageRepository.GetByEntity(SelectedAccommodation.Id, Enums.ImageType.Accommodation))
            {
                images.Add(i);
            }

            return images;
        }

        public void GradeEmptyReview()
        {

            if (SelectedGuestReview.respectGrade == 0)
            {
                ReviewGuest reviewGuest = new ReviewGuest(_guestReviewRepository, SelectedGuestReview.ReservationId);
                reviewGuest.ShowDialog();
            }
        }

        public OwnerReview FindLinkedReview(GuestReviewDTO review)
        {
            return _ownerReviewRepository.Get(review.reservationId);
        }

        public void ShowGuestsReview()
        {
            OwnerReview ownerReview = FindLinkedReview(SelectedGuestReview);
            if (ownerReview != null)
            {
                GuestsReviewOfAccommodation guestsReview = new GuestsReviewOfAccommodation(SelectedGuestReview, FindLinkedReview(SelectedGuestReview));
                guestsReview.ShowDialog();
            }
            else
                MessageBox.Show("This guest hasn't reviewed your accommodation yet.", "Notice", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        public void AllowReservationChange()
        {
            AllowReservationChange allowReservationChange = new AllowReservationChange(SelectedChange, _reservationRepository, _reservationChangeRepository);
            allowReservationChange.ShowDialog();



        }

        public void EnterOwnerInfo()
        {
            Math.Round(Owner.AverageGrade, 3);   
            OwnerInfo = new OwnerInfoDTO(Owner, GetTotalReservationCount(), GetTotalAccommodationCount());
            OwnerInfo ownerInfo = new OwnerInfo(OwnerInfo);
            ownerInfo.ShowDialog();

        }

        private void UpdateOwner(AccommodationReservation reservation)
        {
            foreach(OwnerReview review in _ownerReviewRepository.GetAll()) 
            {
                if(reservation.Id == review.ReservationId)
                {
                    Owner.GradeCount++;
                    Owner.AverageGrade = Owner.AverageGrade + ((review.Correctness + review.Cleanliness) / 2.0);
                }
            }
        }

        private void UpdateOwnerStatus()
        {
            Owner.AverageGrade = Owner.AverageGrade / Owner.GradeCount;

            Owner.AverageGrade = Math.Round(Owner.AverageGrade, 3);
            
            if (!Owner.IsSuper && Owner.GradeCount >= 50 &&  Owner.AverageGrade > 4.5)
            {
                Owner.IsSuper = true;
                _ownerRepository.Update(Owner);
            }
            else if (Owner.IsSuper && Owner.AverageGrade < 4.5)
            {
                Owner.IsSuper = false;
                _ownerRepository.Update(Owner);
            }
        }

        public int GetTotalReservationCount()
        {
            int total = 0;
            foreach (AccommodationReservation reservation in _reservationRepository.GetAll())
            {
                Accommodation temp = _repository.GetByReservationId(reservation.AccommodationId);
                if (temp.OwnerId == Owner.Id)
                {
                    total++;
                }
            }

            return total;
        }

        public int GetTotalAccommodationCount()
        {
            int total = 0;
            foreach (Accommodation accommodation in _repository.GetAll())
            {
                if (accommodation.OwnerId == Owner.Id)
                {
                    total++;
                }
            }
            return total;
        }


    }
}
