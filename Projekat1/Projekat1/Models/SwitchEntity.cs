using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Projekat1.Models
{
    public class SwitchEntity :PowerEntity
    {
        public SwitchEntity()
        {
            Color = Brushes.Green;
        }

        private string status;

        public string Status
        {
            get
            {
                return status;
            }

            set
            {
                status = value;
            }
        }
    }
}
