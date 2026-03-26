using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CampusTouch.Application.Common.Exceptions
{
    public  class BuisnessRuleException:Exception
    {
        public BuisnessRuleException(string message):base(message)
        {
            
        }
    }
}
