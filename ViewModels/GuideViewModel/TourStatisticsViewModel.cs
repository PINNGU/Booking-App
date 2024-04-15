﻿using BookingApp.ApplicationServices;
using BookingApp.DTOs;
using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.Resources;
using BookingApp.View.GuideView.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingApp.ViewModels.GuideViewModel
{
    public class TourStatisticsViewModel
    {
        public User LoggedUser { get; set; }
       


        public static ObservableCollection<TourStatisticsDTO> FinishedTours { get; set; }
        public static ObservableCollection<TourStatisticsDTO> MostVisitedTour { get; set; }

        private string _selectedYear;
        public string SelectedYear
        {
            get { return _selectedYear; }
            set
            {
                if (_selectedYear != value)
                {
                    _selectedYear = value;
                    OnPropertyChanged(nameof(SelectedYear));
                    if (_selectedYear.Equals("Default"))
                        FindMostVisitedTour(0);
                    else
                    {
                        FindMostVisitedTour(int.Parse(_selectedYear));
                    }
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }


        public TourStatisticsPage Window { get; set; }

        public TourStatisticsViewModel(TourStatisticsPage window,User user)
        {
           
            LoggedUser = user;

            Window = window;
           
            FinishedTours = new ObservableCollection<TourStatisticsDTO>();
            MostVisitedTour = new ObservableCollection<TourStatisticsDTO>();
            Update();
            FindMostVisitedTour(0);
        }


        public void FindMostVisitedTour(int selectedYear)
        {
            if (FinishedTours.Count == 0) return;

            TourStatisticsDTO mostVisitedTour = GetMostVisitedTour(selectedYear);

            MostVisitedTour.Clear();
            MostVisitedTour.Add(mostVisitedTour);
        }

        public TourStatisticsDTO GetMostVisitedTour(int selectedYear)
        {
            TourStatisticsDTO mostVisitedTour = FinishedTours.First();

            if (Window.datesBox.SelectedIndex != 0 && Window.datesBox.SelectedIndex != -1)
            {
                mostVisitedTour = FindMostVisitedTourByYear(selectedYear);
            }
            else
            {
                mostVisitedTour = FindMostVisitedTourOverall();
            }

            return mostVisitedTour;
        }

        public TourStatisticsDTO FindMostVisitedTourByYear(int selectedYear)
        {
            TourStatisticsDTO? mostVisitedTour = FinishedTours
                .Where(tour => tour.Year == selectedYear)
                .OrderByDescending(tour => tour.TouristNumber)
                .FirstOrDefault();

            return mostVisitedTour ?? FinishedTours.First();
        }
        public TourStatisticsDTO FindMostVisitedTourOverall()
        {
            TourStatisticsDTO? mostVisitedTour = FinishedTours
                .OrderByDescending(tour => tour.TouristNumber)
                .FirstOrDefault();

            return mostVisitedTour ?? FinishedTours.First();
        }




        //It is assumed that the statistics are done for the sum of all TourSchedules, if so, we go through the list of Tour
        //and take the tour schedule, for each tour we count the number of children/adults/elderly, we also need to make a function for dates Combobox because of distinct value
        public void Update()
        {
            Window.datesBox.Items.Clear();
            FinishedTours.Clear();
            List<int> dates = new List<int>();

            foreach (Tour tour in TourService.GetInstance().GetAllByUser(LoggedUser))
            {
                Location location = LocationService.GetInstance().GetById(tour.LocationId);
                Model.Image image = GetFirstTourImage(tour.Id);
                int touristCount = 0;
                int childrenCount = 0;
                int adultCount = 0;
                int elderlyCount = 0;
                foreach (TourSchedule schedule in TourScheduleService.GetInstance().GetAllByTourId(tour.Id))
                {
                    if (schedule.TourActivity != Enums.TourActivity.Finished) continue;

                    CountGuests(schedule, ref touristCount, ref childrenCount, ref adultCount, ref elderlyCount);

                    FinishedTours.Add(new TourStatisticsDTO(tour.Name, schedule.Start, tour.Language, image.Path, location, touristCount, childrenCount, adultCount, elderlyCount));
                    dates.Add(schedule.Start.Year);
                }

            }

            AddDatesToComboBox(dates);
        }


        public void CountGuests(TourSchedule schedule, ref int touristCount, ref int childrenCount, ref int adultCount, ref int elderlyCount)
        {
            foreach (TourGuests guest in TourGuestService.GetInstance().GetAllByTourId(schedule.Id))
            {
                touristCount++;

                if (guest.Age < 18)
                {
                    childrenCount++;
                }
                else if (guest.Age >= 18 && guest.Age < 50)
                {
                    adultCount++;
                }
                else
                {
                    elderlyCount++;
                }
            }
        }

        public void AddDatesToComboBox(List<int> dates)
        {
            var distinctDates = dates.Distinct().ToList();
            foreach (var date in distinctDates)
            {
                Window.datesBox.Items.Add(date);
            }

            Window.datesBox.Items.Insert(0, "Default");

        }

        public Model.Image GetFirstTourImage(int tourId)
        {
            return ImageService.GetInstance().GetByEntity(tourId, Enums.ImageType.Tour).First();
        }

    }
}
