using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomRadioButtonsGroup
{
    public class MainPageViewModel
    {

        public List<DevType> DevTypes { get; set; }
        public List<string> Genres { get; set; }

        public MainPageViewModel()
        {
            DevTypes = new List<DevType>
            {
                new DevType{Id=1, Title="Web developer"},
                new DevType{Id=2, Title="UI developer"},
                new DevType{Id=3, Title="Mobile developer"},
                new DevType{Id=4, Title="Desktop developer"}
            };

            Genres = new List<string> { "Female", "Male", "Prefer not to say" };
        }
    }
}
