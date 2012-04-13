using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BikeInCity.Model
{
    public class InformationTip
    {
        public virtual int Id { get; set; }
        public virtual String Title { get; set; }
        public virtual String Description { get; set; }
        public virtual String Author { get; set; }
        public virtual String ImageUrl { get; set; }
        public virtual City City { get; set; }
        public virtual double Lat { get; set; }
        public virtual double Lng { get; set; }
    }
}
