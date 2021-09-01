using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wizard.Models
{
    public interface IPoco<T> where T : class
    {
        T ToPoco();
    }
}
