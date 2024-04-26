﻿using BookingApp.ApplicationServices;
using BookingApp.DTOs;
using BookingApp.Model;
using BookingApp.ViewModels.OwnerViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace BookingApp.View.OwnerViews
{
    /// <summary>
    /// Interaction logic for ScheduleRenovation.xaml
    /// </summary>
    public partial class ScheduleRenovation : Window
    {
        ScheduleRenovationVM ViewModel { get; set; }

        public ScheduleRenovation(AccommodationOwnerDTO Accommodation,List<ReservationOwnerDTO> Reservations)
        {
            ViewModel = new ScheduleRenovationVM(Accommodation, Reservations);
            DataContext = ViewModel;
            InitializeComponent();
        }

        private void Find_Click(object sender, RoutedEventArgs e)
        {
            HelpBox.Text = ViewModel.FindDates(StartDatePicker.SelectedDate,EndDatePicker.SelectedDate,DurationPicker.Text);

        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            int duration;
            if(ViewModel.isPeriodFound && StartDatePicker.SelectedDate != null && EndDatePicker.SelectedDate != null && int.TryParse(DurationPicker.Text, out duration))
            {
                if(MessageBox.Show("Confirm renovation?", "Schedule", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {

                    ViewModel.ConfirmSchedule(TitleText.Text);
                    Close();
                }
            }
            else
            {
                HelpBox.Text = "Please find a period first using dates and a duration.";
            }
        }
    }
}
