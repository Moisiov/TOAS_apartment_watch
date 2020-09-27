using System;
using System.Collections.Generic;
using System.Text;

namespace TOASApartmentWatch.Models.ApartmentData
{
    class MonthlyApartmentContainer
    {
        public string Title { get; set; }
        public List<ApartmentModel> Apartments { get; set; }
    }
}
