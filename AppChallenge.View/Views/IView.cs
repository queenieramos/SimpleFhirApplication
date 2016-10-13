using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppChallenge.UI
{
    public interface IView
    {
        void DisplayOperationCompletedNotification(Operation operation, string customMessage = "");
        void DisplayErrorNotification(Exception exception);
    }
}
