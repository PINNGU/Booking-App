﻿using BookingApp.Model;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BookingApp.Observer;

namespace BookingApp.Repository
{
    public class TourGuestRepository
    {
        private const string FilePath = "../../../Resources/Data/guests.csv";

        private readonly Serializer<TourGuests> _serializer;
        private readonly List<IObserver> _observers;

        private readonly TourReservationRepository _tourReservationRepository;

        private List<TourGuests> _guests;
        public Subject subject;


        public TourGuestRepository()
        {
            _observers = new List<IObserver>();
            _serializer = new Serializer<TourGuests>();
            _guests = _serializer.FromCSV(FilePath);
            subject = new Subject();
            _tourReservationRepository = new TourReservationRepository();

        }

        public List<TourGuests> GetAll()
        {
            return _serializer.FromCSV(FilePath);
        }

        public TourGuests Save(TourGuests guest)
        {
            guest.Id = NextId();
            _guests = _serializer.FromCSV(FilePath);
            _guests.Add(guest);
            _serializer.ToCSV(FilePath, _guests);
            subject.NotifyObservers();

            return guest;
        }

        public int NextId()
        {
            _guests = _serializer.FromCSV(FilePath);
            if (_guests.Count < 1)
            {
                return 1;
            }
            return _guests.Max(x => x.Id) + 1;
        }

        public void Delete(TourGuests guest)
        {
            _guests = _serializer.FromCSV(FilePath);
            TourGuests found = _guests.Find(x => x.Id == guest.Id);
            _guests.Remove(found);
            _serializer.ToCSV(FilePath, _guests);
            subject.NotifyObservers();
        }

        public TourGuests Update(TourGuests guest)
        {
            _guests = _serializer.FromCSV(FilePath);
            TourGuests current = _guests.Find(x => x.Id == guest.Id);
            int index = _guests.IndexOf(current);
            _guests.Remove(current);
            _guests.Insert(index, guest);
            _serializer.ToCSV(FilePath, _guests);
            subject.NotifyObservers();
            return guest;
        }

        public List <TourGuests> GetAllByTourId(int tourScheduleId)
        {
            _guests = _serializer.FromCSV(FilePath);
            List<TourGuests> guests = new List <TourGuests>();
            List <TourReservation> tourReservations = new List <TourReservation>();
            tourReservations = _tourReservationRepository.GetAllByTourScheduleId(tourScheduleId);
            foreach(TourReservation reservation in tourReservations)
            {
                foreach(TourGuests tourGuest in _guests)
                {
                    if(reservation.Id == tourGuest.ReservationId && tourGuest.IsSelected==false)
                    {
                        guests.Add(tourGuest);
                    }
                }
            }
            return guests;
        }



        public void Subscribe(IObserver observer)
        {
            _observers.Add(observer);
        }

        public void Unsubscribe(IObserver observer)
        {
            _observers.Remove(observer);
        }
        public void NotifyObservers()
        {
            foreach (var observer in _observers)
            {
                observer.Update();
            }
        }

    }
}

