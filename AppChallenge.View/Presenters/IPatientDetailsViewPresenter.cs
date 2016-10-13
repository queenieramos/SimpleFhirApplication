using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppChallenge.UI
{
    public interface IPatientDetailsViewPresenter
    {
        void InitializePresenterAndPatient(Guid patientId);
        void ShowForm(PatientDetailsDisplayMode patientDetailsDisplayMode, Guid patientId);
        void CloseForm();
        void SaveChanges();
        void ToggleAnimalFieldAttributes();
        void SetDateTimeFormat();

        event Action UpdatedPatient;
    }
}
