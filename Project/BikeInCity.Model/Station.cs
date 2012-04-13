using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace BikeInCity.Model
{
    public class Station
    {
        public virtual int Id { get; set; }
        public virtual String Address { get; set; }
        public virtual double Lat { get; set; }
        public virtual double Lng { get; set; }
        public virtual int Total { get; set; }
        public virtual int Free { get; set; }
        public virtual int Ticket { get; set; }
        public virtual bool IsUpdate { get; set; }
        public virtual City City { get; set; }
    }
}
