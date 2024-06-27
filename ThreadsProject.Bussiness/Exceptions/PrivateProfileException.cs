using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadsProject.Bussiness.Exceptions
{
    public class PrivateProfileException:Exception
    {
        public PrivateProfileException(string message) : base(message) { }
    }
}
