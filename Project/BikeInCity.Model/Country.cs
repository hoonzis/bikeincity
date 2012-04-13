using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BikeInCity.Model
{
    public class Country
    {
        public virtual int Id { get; set; }
        public virtual String Name { get; set; }
        public virtual IList<City> Cities { get; set; }
    }
}
