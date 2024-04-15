﻿using BookingApp.ApplicationServices;
using BookingApp.Model;
using BookingApp.Repository;
using BookingApp.Resources;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BookingApp.View.GuideView.Pages
{
    /// <summary>
    /// Interaction logic for TourCreationPage.xaml
    /// </summary>
    public partial class TourCreationPage : Page, INotifyPropertyChanged
    {
        public User LoggedUser { get; set; }

       

        public Tour SelectedTour { get; set; }
        public Location SelectedLocation { get; set; }
        public String SelectedImageUrl { get; set; }
        public String SelectedCheckpoint { get; set; }

        public DateTime SelectedTourDate { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public static ObservableCollection<String> ImagesCollection { get; set; }
        public static ObservableCollection<String> CheckpointsCollection { get; set; }
        public static ObservableCollection<DateTime> TourDatesCollection { get; set; }
        
        public event EventHandler SomethingHappened;

        private string checkpoint;
        public string Checkpoint
        {
            get
            {
                return checkpoint;
            }
            set
            {
                if (value != checkpoint)
                {
                    checkpoint = value;
                    OnPropertyChanged("Checkpoint");
                }
            }
        }


        public TourCreationPage(User user)
        {
            InitializeComponent();
            DataContext = this;
            LoggedUser = user;
            datePicker.DisplayDateStart = DateTime.Now;
           

            SelectedLocation = new Location();
            SelectedTour = new Tour();
            SelectedTourDate = new DateTime();
            CheckpointsCollection = new ObservableCollection<string>();
            ImagesCollection = new ObservableCollection<string>();
            TourDatesCollection = new ObservableCollection<DateTime>();
        }

        protected virtual void OnSomethingHappened(EventArgs e)
        {
            SomethingHappened?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddCheckPointClick(object sender, EventArgs e)
        {

            CheckpointsCollection.Add(checkpoint);
            txtTourCheckpoints.Clear();
        }



        private void SelectImagesClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Images|*.jpg;*.jpeg;*.png;*.gif|All Files|*.*";
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string imagePath in openFileDialog.FileNames)
                {
                    int relativePathStartIndex = imagePath.IndexOf("\\Resources");
                    String relativePath = imagePath.Substring(relativePathStartIndex);
                    ImagesCollection.Add(relativePath);
                }
            }
        }

        private void SelectDatesClick(object sender, RoutedEventArgs e)
        {
            DateTime time;
            DateTime.TryParse(datePicker.SelectedDate.Value.Date.ToShortDateString() + " " + txtTourScheduleTime.Text, out time);
            TourDatesCollection.Add(time);
            datePicker.SelectedDate = null;
            txtTourScheduleTime.Clear();
        }

        private void RemoveDateClick(object sender, RoutedEventArgs e)
        {
            DateTime SelectedDate = SelectedTourDate;
            TourDatesCollection.Remove(SelectedDate);
        }

        bool IsDataValid()
        {
            return !string.IsNullOrEmpty(txtTourName.Text)
                && !string.IsNullOrEmpty(txtTourLanguage.Text)
                && !string.IsNullOrEmpty(txtTourCity.Text)
                && !string.IsNullOrEmpty(txtTourState.Text)
                && !string.IsNullOrEmpty(txtTourCapacity.Text)
                && !string.IsNullOrEmpty(txtTourDescription.Text)
                && !string.IsNullOrEmpty(txtTourDescription.Text)
                && CheckpointsCollection.Count >= 2
                && ImagesCollection.Count >= 1
                && TourDatesCollection.Count >= 1;


        }



        private void SaveTourClick(object sender, RoutedEventArgs e)
        {
            if (!IsDataValid())
            {
                return;
            }
           
            
            SelectedLocation = LocationService.GetInstance().Save(SelectedLocation);
            SelectedTour.LocationId = SelectedLocation.Id;
            SelectedTour.GuideId = LoggedUser.Id;
            SelectedTour = TourService.GetInstance().Save(SelectedTour);


            SaveImages(ImagesCollection.ToList());
            SaveTourDatesAndCheckpoints(TourDatesCollection.ToList(), CheckpointsCollection.ToList());
            OnSomethingHappened(EventArgs.Empty);

            MessageBox.Show("Tour Added");
        }

      

        private void SaveImages(List<String> images)
        {
            foreach (string relativePath in images)
            {
                ImageService.GetInstance().Save(new Model.Image(relativePath, SelectedTour.Id, Enums.ImageType.Tour));
            }
        }

        private void SaveTourDatesAndCheckpoints(List<DateTime> dates, List <String> checkpoints)
        {
            foreach (DateTime date in dates)
            {
               TourSchedule tourSchedule = TourScheduleService.GetInstance().Save(new TourSchedule(date, SelectedTour.Id, SelectedTour.Capacity,Enums.TourActivity.Ready));
                foreach(string checkpoint in checkpoints)
                {
                    CheckpointService.GetInstance().Save(new Checkpoint(checkpoint, SelectedTour.Id,false,tourSchedule.Id));
                }
            }
        }

        private void CheckpointRemoveClick(object sender, EventArgs e)
        {
            string checkpoint = SelectedCheckpoint;
            CheckpointsCollection.Remove(checkpoint);
        }

        private void ImageRemoveClick(object sender, EventArgs e)
        {
            string imageUrl = SelectedImageUrl;
            ImagesCollection.Remove(imageUrl);
        }

    }
}

