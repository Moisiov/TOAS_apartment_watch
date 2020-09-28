using System;
using System.Collections.Generic;
using System.Text;
using TOASApartmentWatch.Models.ApartmentData;

namespace TOASApartmentWatch.TelegramAPI
{
    public interface ITelegramAPI
    {
        /// <summary>
        /// Update Telegram-bots list of apartments based on fetched apartments from TOAS 
        /// </summary>
        /// <param name="data">list of all fetched apartments</param>
        public void UpdateApartmentData(List<MonthlyApartmentContainer> data);
    }
}
