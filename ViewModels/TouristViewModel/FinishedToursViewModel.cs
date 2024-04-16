﻿using BookingApp.ApplicationServices;
using BookingApp.DTOs;
using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.View.GuideView.Pages;
using BookingApp.View.TouristView;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BookingApp.ViewModels.TouristViewModel
{
    public class FinishedToursViewModel
    {
        public static ObservableCollection<TourScheduleDTO> Tours { get; set; }
        public User LoggedUser { get; set; }

        public RelayCommand RateTourCommand { get; set; }

        private ImageRepository _imageRepository;
        public TourScheduleDTO SelectedTourSchedule { get; set; }
        public FinishedTours Window { get; set; }
        public FinishedToursViewModel(FinishedTours window, User user)
        {
            Window = window;
            LoggedUser = user;

            _imageRepository = new ImageRepository();
            Tours = new ObservableCollection<TourScheduleDTO>();
            RateTourCommand = new RelayCommand(Execute_RateTourCommand);

            Update();
        }

        public void Update()
        {
            Tours.Clear();
            foreach (TourSchedule tour in TourScheduleService.GetInstance().GetAllFinishedTours(LoggedUser))
            {
                Tours.Add(new TourScheduleDTO(tour));
            }

        }

        public void Execute_RateTourCommand(object obj)
        {
            if (SelectedTourSchedule != null)
            {
                TourRating rating = new TourRating(SelectedTourSchedule, _imageRepository, LoggedUser);
                rating.ShowDialog();//U CODE BEHIND
            }

        }
    }
}
