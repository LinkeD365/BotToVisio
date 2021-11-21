using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkeD365.BotToVisio
{
    class Topic
    {
        [Browsable(false)]
        public string Id { get; set; }
        public string Name { get; set;  }   
        public string Description { get; set; }
        [Browsable(false)]
        public string Canvas { get; set; }
    }
}
