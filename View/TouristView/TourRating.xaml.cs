﻿using BookingApp.ApplicationServices;
using BookingApp.Domain.Model;
using BookingApp.DTOs;
using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.RepositoryInterfaces;
using BookingApp.Resources;
using BookingApp.ViewModels.TouristViewModel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BookingApp.View.TouristView
{
    /// <summary>
    /// Interaction logic for TourRating.xaml
    /// </summary>
    public partial class TourRating : Window
    {
        private readonly ImageRepository _imageRepository;
        //private readonly TourReviewService _reviewService = new TourReviewService();
        public TourScheduleDTO SelectedTour { get; set; }
        public User LoggedUser { get; set; }

        public TourReviewDTO TourReviewDTO { get; set; }
        public String SelectedImageUrl { get; set; }
        public static ObservableCollection<String> ImagesCollection { get; set; }
        public TourRatingViewModel RatingViewModel { get; set; }
        public TourRating(TourScheduleDTO selectedTour, ImageRepository imageRepository, User user)
        {
            InitializeComponent();
            RatingViewModel = new TourRatingViewModel(this, selectedTour, imageRepository,user);
            DataContext = RatingViewModel;
           
        }

    }
}
