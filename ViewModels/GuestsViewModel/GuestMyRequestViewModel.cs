﻿using BookingApp.DTOs;
using BookingApp.View.GuestViews;
using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Windows;


namespace BookingApp.ViewModels.GuestsViewModel
{
    public class GuestMyRequestViewModel : INotifyPropertyChanged
    {
        private AccommodationRepository _accommodationRepository;
        private ImageRepository _imageRepository;
        private AccommodationReservationRepository accommodationReservationRepository;
        private ReservationChangeRepository _reservationChangeRepository;
        public static ObservableCollection<ImageDTO> Images { get; set; }
        public ObservableCollection<GuestRequstDTO> ChangeReservations { get; set; }
        public Guest Guest { get; set; }
        public NavigationService NavService { get; set; }
        public RelayCommand SeeMoreCommand { get; set; }
        public GuestMyRequestViewModel(Guest guest, NavigationService navigation)
        {
            Guest = guest;
            accommodationReservationRepository = new AccommodationReservationRepository();
            _imageRepository = new ImageRepository();
            _accommodationRepository = new AccommodationRepository();
            _reservationChangeRepository= new ReservationChangeRepository();
            ChangeReservations = new ObservableCollection<GuestRequstDTO>();
            //SeeMoreCommand = new RelayCommand(Execute_SeeMoreCommand);
            NavService = navigation;
            Update();
        }
        
        public void Update()
        {
            ChangeReservations.Clear();
            foreach (ReservationChanges reservationChange in _reservationChangeRepository.GetAll())
            {
                Model.Image image = new Model.Image();
                foreach (Model.Image i in _imageRepository.GetByEntity(reservationChange.AccommodationId, Enums.ImageType.Accommodation))
                {
                    image = i;
                    break;
                }
                Accommodation accommodation = _accommodationRepository.GetByReservationId(reservationChange.AccommodationId);
                ChangeReservations.Add(new GuestRequstDTO(Guest, reservationChange, accommodation, image.Path));
            }
        }
       
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
