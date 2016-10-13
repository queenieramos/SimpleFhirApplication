using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppChallenge.UI
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize view.
            var view = new PatientMainView();

            // Initialize presenter.
            var presenter = new PatientMainViewPresenter(view);
            presenter.Start();

            Application.Run(view);
        }
    }
}
