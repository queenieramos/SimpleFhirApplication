using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppChallenge.Model;

namespace AppChallenge.UI
{
    public interface IPatientDetailsView : IView
    {
        Patient Patient { get; set; }

        void ShowDialogForm();
        void CloseDialogForm();
        void InitializeComboboxBindings();
        void ToggleAnimalAttributes();
        void SetDateTimeFormatString();

        void AttachPresenter(PatientDetailsViewPresenter patientDetailsViewPresenter);
    }
}
