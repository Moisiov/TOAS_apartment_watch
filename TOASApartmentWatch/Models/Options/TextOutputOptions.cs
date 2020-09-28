using System;
using System.Collections.Generic;
using System.Text;

namespace TOASApartmentWatch.Models.Options
{
    public class TextOutputOptions
    {
        public ConsoleColor TextColor { get; set; }
        public ConsoleColor TextHighlightColor { get; set; }
        public int NewApartmentHighLightMinutes { get; set; }
        public bool ClearConsoleAutomatically { get; set; }
    }
}
