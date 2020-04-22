using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ProductivityMonitor.Service.Data.Models
{
    public class ApplicationRecord : AbstractInputRecord
    {
        public int Pid { get; set; }

        public string ApplicationName { get; set; }

        public string Title { get; set; }

        public List<KeyboardInputRecord> KeyboardInputs { get; set; }

        public List<MouseInputRecord> MouseInputs { get; set; }

    }
}