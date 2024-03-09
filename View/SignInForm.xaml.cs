﻿using BookingApp.Model;
using BookingApp.Repository;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using BookingApp.Resources;

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
                            AccommodationViewMenu accommodationViewMenu = new AccommodationViewMenu(user,_locationRepository,_imageRepository,_accommodationReservationRepository,_repository);
                            accommodationViewMenu.Show();
                            Close();
                            break;
                        case Enums.UserType.Guest:
                            AccommodationReservationViewMenu accommodationReservationViewMenu = new AccommodationReservationViewMenu(_locationRepository);
                            accommodationReservationViewMenu.Show();
                            Close(); 
                            break;
                        case Enums.UserType.Tourist:
                            TouristViewMenu touristViewMenu = new TouristViewMenu( _locationRepository, _imageRepository);
                            touristViewMenu.Show();
                            Close();
                            break;
                        case Enums.UserType.Guide:
                            //replace with guide 
                            GuideViewMenu guideViewMenu = new GuideViewMenu(user,_locationRepository);
                            guideViewMenu.Show();
                            Close();
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
    }
}
