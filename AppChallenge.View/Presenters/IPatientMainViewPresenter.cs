using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppChallenge.UI
{
    public interface IPatientMainViewPresenter
    {
        void Start();
        void RefreshGrid();
        void DisplayPatientDetailsForm(PatientDetailsDisplayMode patientDetailsDisplayMode);
        void DeletePatient();
        void SearchPatient();
        void ImportPatientData();
    }
}
