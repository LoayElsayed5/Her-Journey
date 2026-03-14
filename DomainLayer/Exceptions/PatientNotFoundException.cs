using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Exceptions
{
    public sealed class PatientNotFoundException(int id) : NotFoundException($"Patient With Id: {id} Is Not Found")
    {
    }
}
