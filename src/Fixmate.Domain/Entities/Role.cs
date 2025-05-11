using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FixMate.Domain.Enums
{
    /// <summary>
    ///  in fact there's no need for this, but it is a good practice to have a role enum for future enhancements 
    /// </summary>

    public enum Role
    {
        Admin, 
        ServiceProvider,
        Customer

    }
}
