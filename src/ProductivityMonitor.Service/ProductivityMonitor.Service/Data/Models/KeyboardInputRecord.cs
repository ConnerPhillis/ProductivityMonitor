using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProductivityMonitor.Service.Data.Models
{
    public class KeyboardInputRecord : AbstractInputRecord
    {
        public string KeyPressed { get; set; }

        public ApplicationRecord ActiveApplication { get; set; }
        public int ActiveApplicationId { get; set; }
    }
}