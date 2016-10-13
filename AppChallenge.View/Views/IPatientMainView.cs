using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppChallenge.Model;

namespace AppChallenge.UI
{
    public interface IPatientMainView : IView
    {
        List<Patient> PatientList { get;  set; }
        List<Patient> PatientImportList { get; set; }
        DatabaseSearchOption SelectedDatabase { get; set; }
        Guid SelectedPatientId { get; set; }
        string SearchString { get; set; }
        string TotalRecords { get; set; }
        string CurrentView { get; set; }

        void AttachPresenter(PatientMainViewPresenter patientMainViewPresenter);
        void EnableDisableButtons();

        IObservable<EventArgs> OnSearchTextChanged { get; }
    }
}
