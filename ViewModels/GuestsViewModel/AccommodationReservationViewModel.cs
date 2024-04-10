﻿using BookingApp.DTOs;
using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.Resources;
using BookingApp.View.GuestViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Windows;

namespace BookingApp.ViewModels.GuestsViewModel
{
    public class AccommodationReservationViewModel : INotifyPropertyChanged
    {
        public AccommodationOwnerDTO Accommodation { get; set; }
        public Guest Guest { get; set; }
        public NavigationService NavService { get; set; }
        public AccommodationReservationView ReservationView { get; set; }
        private ImageRepository _imageRepository;
        private AccommodationReservationRepository _accommodationReservationRepository;
        private AccommodationRepository _accommodationRepository;
        public ObservableCollection<DateRanges> AvailableDates { get; set; }
        private AccommodationReservationRepository _reservationRepository;
        public List<Model.Image> ListImages { get; set; }
        private int _accommodationId;
        public DateRanges SelectedDates { get; set; }
        public RelayCommand ReserveCommand { get; set; }
        public RelayCommand FindDateCommand { get; set; }
        public RelayCommand NextImageCommand { get; set; }
        public RelayCommand PreviousImageCommand { get; set; }
        public RelayCommand FirstDateCommand { get; set; }
        public AccommodationReservationViewModel(Guest guest, AccommodationOwnerDTO SelectedAccommodation, AccommodationReservationView reservationView, NavigationService navigation)
        {
            Guest = guest;
            Accommodation = SelectedAccommodation;
            ReservationView = reservationView;
            NavService = navigation;
            ImageRepository _imageRepository = new ImageRepository();

            _accommodationId = SelectedAccommodation.Id;
            AccommodationName = SelectedAccommodation.Name;
            State = SelectedAccommodation.State;
            City = SelectedAccommodation.City;
            Type = SelectedAccommodation.Type.ToString();
            CancelationDays = SelectedAccommodation.CancelationDays.ToString();
            MaxGuests = SelectedAccommodation.MaxGuests.ToString();
            MinDays = SelectedAccommodation.MinReservationDays.ToString();
            List<Model.Image> lista = new List<Model.Image>();
            foreach (Model.Image image in _imageRepository.GetByEntity(SelectedAccommodation.Id, Enums.ImageType.Accommodation))
            {
                lista.Add(image);

            }
            ListImages = lista;
            _accommodationReservationRepository = new AccommodationReservationRepository();
            _accommodationRepository = new AccommodationRepository();
            AvailableDates = new ObservableCollection<DateRanges>();

            NextImageCommand = new RelayCommand(Execute_NextImageCommand);
            PreviousImageCommand = new RelayCommand(Execute_PreviousImageCommand);
            FirstDateCommand = new RelayCommand(Execute_FirstDateCommand);
            ReserveCommand = new RelayCommand(Execute_ReserveCommand);
            FindDateCommand = new RelayCommand(Execute_FindDateCommand);

            AvaibleDatesVisibility = Visibility.Collapsed;


        }
        public void Execute_NextImageCommand(object obj)
        {
            if (CurrentImageIndex < ListImages.Count - 1)
                CurrentImageIndex++;
            else CurrentImageIndex = 0;
        }
        public void Execute_FindDateCommand(object obj)
        {
            DateTime? firstDatePicekr = ReservationView.FirstDatePicker.SelectedDate;
            DateTime? lastDatePicekr = ReservationView.LastDatePicker.SelectedDate;
            if (firstDatePicekr.HasValue && Convert.ToInt32(DaysNumber) > 0 && Convert.ToInt32(GuestNumber) > 0 && lastDatePicekr.HasValue)
            {
                DateOnly firstDate = DateOnly.FromDateTime((DateTime)firstDatePicekr);
                DateOnly lastDate = DateOnly.FromDateTime((DateTime)lastDatePicekr);
                if (lastDate < firstDate.AddDays(Convert.ToInt32(DaysNumber)))
                {
                    MessageBox.Show("The end date must be greater than the start date by a minimum number of days", "Date entry error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    List<DateRanges> availableDates = new List<DateRanges>();
                    availableDates = _accommodationReservationRepository.GetAvailableDates(firstDate, lastDate, Convert.ToInt32(DaysNumber), _accommodationId);
                    AvailableDates.Clear();
                    foreach (DateRanges dateRange in availableDates)
                    {
                        AvailableDates.Add(dateRange);
                    }
                    AvaibleDatesVisibility = Visibility.Visible;
                }
            }
            else
            {
                MessageBox.Show("The fields are not filled in correctly!", "WRONG FORMAT ", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        public void Execute_ReserveCommand(object obj)
        {
            if (SelectedDates != null)
            {
                AccommodationReservation accommodationReservation = new AccommodationReservation(Accommodation.Id, Guest.Id, SelectedDates.CheckIn, SelectedDates.CheckOut, Convert.ToInt32(GuestNumber), Enums.ReservationStatus.Active, _accommodationRepository.GetByReservationId(Accommodation.Id));
                _accommodationReservationRepository.Save(accommodationReservation);
                MessageBox.Show("Successful kao booking!", "WELL DONE", MessageBoxButton.OK);
                NavService.Navigate(new GuestMyReservationsView(Guest, NavService));
            }
            else MessageBox.Show("You must select date ranges!", "Select date", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        public void Execute_FirstDateCommand(object obj)
        {
            ReservationView.LastDatePicker.IsEnabled = true;
            DateTime? firstDatePicekr = ReservationView.FirstDatePicker.SelectedDate;
            if (firstDatePicekr.HasValue) ReservationView.LastDatePicker.DisplayDateStart = firstDatePicekr.Value.AddDays(Convert.ToInt32(MinDays));
        }

        public void Execute_PreviousImageCommand(object obj)
        {
            if (CurrentImageIndex > 0)
                CurrentImageIndex--;
            else CurrentImageIndex = ListImages.Count - 1;

        }
        private Visibility avaibleDatesVisibility;

        public Visibility AvaibleDatesVisibility
        {
            get { return avaibleDatesVisibility; }
            set
            {
                if (avaibleDatesVisibility != value)
                {
                    avaibleDatesVisibility = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _accommodationName;
        public string AccommodationName
        {
            get => _accommodationName;
            set
            {
                if (value != _accommodationName)
                {
                    _accommodationName = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _state;
        public string State
        {
            get => _state;
            set
            {
                if (value != _state)
                {
                    _state = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _city;
        public string City
        {
            get => _city;
            set
            {
                if (value != _city)
                {
                    _city = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _type;
        public string Type
        {
            get => _type;
            set
            {
                if (value != _type)
                {
                    _type = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _maxGuests;
        public string MaxGuests
        {
            get => _maxGuests;
            set
            {
                if (value != _maxGuests)
                {
                    _maxGuests = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _minDays;
        public string MinDays
        {
            get => _minDays;
            set
            {
                if (value != _minDays)
                {
                    _minDays = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _cancelationDays;
        public string CancelationDays
        {
            get => _cancelationDays;
            set
            {
                if (value != _cancelationDays)
                {
                    _cancelationDays = value;
                    OnPropertyChanged();
                }
            }
        }

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

        private string _firstDate;
        public string FirstDate
        {
            get => _firstDate;
            set
            {
                if (value != _firstDate)
                {
                    _firstDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _lastDate;
        public string LastDate
        {
            get => _lastDate;
            set
            {
                if (value != _lastDate)
                {
                    _lastDate = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _guestNumber;
        public string GuestNumber
        {
            get => _guestNumber;
            set
            {
                if (value != _guestNumber)
                {
                    _guestNumber = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _daysNumber;
        public string DaysNumber
        {
            get => _daysNumber;
            set
            {
                if (value != _daysNumber)
                {
                    _daysNumber = value;
                    OnPropertyChanged();
                }
            }
        }
        private string _imageAccommodation;
        public string ImageAccommodation
        {
            get => _imageAccommodation;
            set
            {
                if (value != _imageAccommodation)
                {
                    _imageAccommodation = value;
                    OnPropertyChanged();
                }
            }
        }
        private int _currentImageIndex = 0;
        public int CurrentImageIndex
        {
            get => _currentImageIndex;
            set
            {
                _currentImageIndex = value;
                OnPropertyChanged(nameof(CurrentImage));
            }
        }
        public Model.Image CurrentImage => ListImages[CurrentImageIndex];


        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
