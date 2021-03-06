﻿using System;
using System.Collections.Generic;
using System.Text;

namespace TOAS_apartment_watch
{
    class ApartmentModel
    {
        public string Target { get; set; }
        public string ApartmentType { get; set; }
        public string Area { get; set; }
        public string Floor { get; set; }
        public string Rent { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
