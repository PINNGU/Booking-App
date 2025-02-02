﻿using BookingApp.DTOs;
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

namespace BookingApp.View.OwnerViews
{
  

    public partial class DetailedStatisticsView : Window
    {
        public ObservableCollection<MonthlyStatisticDTO> Statistics { get; set; }
        public MonthlyStatisticDTO SelectedMonth {  get; set; }

        public DetailedStatisticsView(List<MonthlyStatisticDTO> statistics,int year)
        {
            
            this.Statistics = new ObservableCollection<MonthlyStatisticDTO>(statistics);
            Title = "Statistics for " + year;
            SelectedMonth = statistics.First();
           

            DataContext = this;
            
            InitializeComponent();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape) 
            {
                Close();
            }
            else if(e.Key == Key.Enter)
            {
                StatisticsList.SelectedIndex = 0;
                StatisticsList.UpdateLayout();
                StatisticsList.Focus();
            }
        }
    }
}
