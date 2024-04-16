﻿using BookingApp.Model;
using BookingApp.Repository;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using BookingApp.Resources;
using BookingApp.View.GuestViews;
using BookingApp.ApplicationServices;

namespace BookingApp.View
{
    /// <summary>
    /// Interaction logic for SignInForm.xaml
    /// </summary>
    public partial class SignInForm : Window
    {

        private readonly UserRepository _repository;
        private readonly LocationRepository _locationRepository;
        private readonly ImageRepository _imageRepository;
        private readonly AccommodationReservationRepository _accommodationReservationRepository;
        private readonly TourScheduleRepository _tourScheduleRepository;
        private readonly TourReservationRepository _tourReservationRepository;
        private readonly UserRepository _userRepository;
        private readonly GuestRepository _guestRepository;
        private readonly OwnerRepository _ownerRepository;
        private readonly OwnerReviewRepository _ownerReviewRepository;
        private readonly TourScheduleService _scheduleService;
        private readonly TourReservationService _reservationService;
        private readonly LocationService _locationService;
        private readonly ImageService _imageService;
        //private readonly TourRepository _tourRepository;

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                if (value != _username)
                {
                    _username = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public SignInForm()
        {
            InitializeComponent();
            DataContext = this;
            _repository = new UserRepository();
            _locationRepository = new LocationRepository();
            _imageRepository = new ImageRepository();
            _accommodationReservationRepository = new AccommodationReservationRepository();
            _tourScheduleRepository = new TourScheduleRepository();
            _tourReservationRepository = new TourReservationRepository();
            _userRepository = new UserRepository();
            _ownerRepository = new OwnerRepository();
            _guestRepository = new GuestRepository();
            _ownerReviewRepository = new OwnerReviewRepository();
            _scheduleService = new TourScheduleService();
            _reservationService = new TourReservationService();
            _locationService = new LocationService();
            _imageService = new ImageService();
            //_tourRepository = new TourRepository();
        }

        private void SignIn(object sender, RoutedEventArgs e)
        {
            User user = _repository.GetByUsername(Username);
            if (user != null)
            {
                if (user.Password == txtPassword.Password)
                {
                    switch (user.Type)
                    {

                        case Enums.UserType.Owner:
                            Owner owner = _ownerRepository.GetAll().Find(o => o.Id == user.Id);
                            InitiateAccommodationView(owner);
                            break;
                        case Enums.UserType.Guest:
                            Guest guest = _guestRepository.GetAll().Find(o => o.UserId == user.Id);
                            InitiateGuestView(guest);
                            break;
                        case Enums.UserType.Tourist:
                            InitiateTouristView(user);
                            break;
                        case Enums.UserType.Guide:
                            InitiateGuideView(user);
                            break;
                    }
                    
                }
                else
                {
                    MessageBox.Show("Wrong password!");
                }
            }
            else
            {
                MessageBox.Show("Wrong username!");
            }
            
        }

        private void InitiateAccommodationView(Owner owner)
        {
            AccommodationViewMenu accommodationViewMenu = new AccommodationViewMenu(owner, _locationRepository, _accommodationReservationRepository,_guestRepository,_ownerReviewRepository);
            accommodationViewMenu.Show();
            Close();
        }

        private void InitiateGuestView(Guest guest)
        {
            if (guest != null)
            {
                GuestStartView guest1StartView = new GuestStartView(guest);
                guest1StartView.Show();
                Close();
            }
        }

        private void InitiateTouristView(User user) 
        {
            TouristViewMenu touristViewMenu = new TouristViewMenu(user, _locationService, _imageService, _scheduleService, _reservationService);
            touristViewMenu.Show();
            Close();
        }

        private void InitiateGuideView(User user) 
        {
            GuideViewMenu guideViewMenu = new GuideViewMenu(user, _locationRepository, _imageRepository);
            guideViewMenu.Show();
            Close();
        }
    }
}
