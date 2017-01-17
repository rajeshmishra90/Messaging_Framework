using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessagingFramework.Logging.DTOS
{
    [Serializable]
    public class Contact
    {
        public string FirstName;
        public string LastName;
        public int Id;
        public Guid Guid;
    }
}
