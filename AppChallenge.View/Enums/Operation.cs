using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppChallenge.UI
{
    /// <summary>
    /// Enum that determines the user activity. This is used for displaying the appropriate MessageBox to show to the user.
    /// </summary>
    public enum Operation
    {
        AddPatient,
        EditPatient,
        DeletePatient,
        ImportPatient
    }
}
