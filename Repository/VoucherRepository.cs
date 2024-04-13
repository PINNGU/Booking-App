﻿using BookingApp.Model;
using BookingApp.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookingApp.Serializer;
using BookingApp.Observer;


namespace BookingApp.Repository
{
    public class VoucherRepository
    {
        private const string FilePath = "../../../Resources/Data/vouchers.csv";
        private readonly Serializer<Voucher> _serializer;
        private List<Voucher> _vouchers;
        public Subject subject;
        
        public TourReservationRepository _tourReservationRepository;
        
        public VoucherRepository()
        {
            _serializer = new Serializer<Voucher>();
            _vouchers = _serializer.FromCSV(FilePath);
            subject = new Subject();
            _tourReservationRepository = new TourReservationRepository();
        }

        public List <Voucher> GetAll()
        {
            return _serializer.FromCSV(FilePath);
        }

        public Voucher Save(Voucher voucher)
        {
            voucher.Id = NextId();
            _vouchers = _serializer.FromCSV(FilePath);
            _vouchers.Add(voucher);
            _serializer.ToCSV(FilePath, _vouchers);
            
            subject.NotifyObservers();
            return voucher;
        }
        
        public void Delete(Voucher voucher)
        {
            _vouchers = _serializer.FromCSV(FilePath);
            Voucher found = _vouchers.Find(x => x.Id == voucher.Id);
            _vouchers.Remove(found);
            _serializer.ToCSV(FilePath, _vouchers);
            subject.NotifyObservers();
        }

        public void SaveAllGuests(List<TourGuests> guests)
        {

            foreach(TourGuests tourGuest in guests)
            {
                TourReservation tourReservation = _tourReservationRepository.GetById(tourGuest.ReservationId);
                Voucher voucher = new Voucher();
                voucher.TouristName = tourGuest.Name;
                voucher.TouristSurname = tourGuest.Surname;
                voucher.TouristBirth = tourGuest.Age;
                voucher.IssuingDate = DateTime.Now.AddYears(1);
                voucher.UserId = tourGuest.UserTypeId;
                Save(voucher);
            }
        }


        public int NextId()
        {
            _vouchers = _serializer.FromCSV(FilePath);
            if (_vouchers.Count < 1)
            {
                return 1;
            }
            return _vouchers.Max(x => x.Id) + 1;
        }

        public List<Voucher> GetAllByUser(User user)
        {
            List<Voucher> vouchers = new List<Voucher>();

            foreach(Voucher voucher in GetAll())
            {
                if(voucher.UserId == user.Id && voucher.IssuingDate > DateTime.Now)
                {
                    vouchers.Add(voucher);
                }
            }
            return vouchers;
        }

    }
}
