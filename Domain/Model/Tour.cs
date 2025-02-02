﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xaml.Schema;
using BookingApp.Repository;
using BookingApp.Resources;
using BookingApp.Serializer;
namespace BookingApp.Model
{
    public class Tour : ISerializable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int LocationId { get; set; }
        public string Description { get; set; }
        public int LanguageId { get; set; }
        public int Capacity {  get; set; }
        public List<Checkpoint> Checkpoints { get; set; }
        public double Duration { get; set; }
        public List<TourSchedule> TourSchedules { get; set; }
        public Enums.TourType Type { get; set; }
        public List<Image> Images { get; set; }
        public int GuideId { get; set; }
        public int RequestId { get; set; }

        public Tour()
        { 
            Images = new List<Image>(); 
            TourSchedules = new List<TourSchedule>();
            
        }

        public string[] ToCSV()
        {
            string[] csvValues = { Id.ToString(), Name,LocationId.ToString(), Description , LanguageId.ToString() , Capacity.ToString() , Duration.ToString(), GuideId.ToString(), Type.ToString(), RequestId.ToString()};
            return csvValues;
        }

        public void FromCSV(string[] values)
        {
            Id = Convert.ToInt32(values[0]);
            Name = values[1];
            LocationId = Convert.ToInt32(values[2]);
            Description =  values[3];
            LanguageId = Convert.ToInt32(values[4]);
            Capacity = Convert.ToInt32(values[5]);
            Duration = Convert.ToDouble(values[6]);
            GuideId = Convert.ToInt32(values[7]);
            Type = (Enums.TourType)Enum.Parse(typeof(Enums.TourType), values[8]);
            RequestId = Convert.ToInt32(values[9]);
        }

    }

}
