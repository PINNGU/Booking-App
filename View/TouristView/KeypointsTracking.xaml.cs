﻿using BookingApp.ApplicationServices;
using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.ViewModels.TouristViewModel;
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
    /// Interaction logic for KeypointsTracking.xaml
    /// </summary>
    public partial class KeypointsTracking : Window
    {
        public KeypointsTrackingViewModel KeypointsViewModel { get; set; }
        public KeypointsTracking(int tourScheduleId)
        {
            InitializeComponent();
            KeypointsViewModel = new KeypointsTrackingViewModel(this, tourScheduleId);
            DataContext = KeypointsViewModel;
        }
    }
}
