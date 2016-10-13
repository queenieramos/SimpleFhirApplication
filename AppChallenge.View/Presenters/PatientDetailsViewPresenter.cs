using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppChallenge.Model;
using AppChallenge.Logging;

namespace AppChallenge.UI
{
    public class PatientDetailsViewPresenter : IPatientDetailsViewPresenter
    {
        private readonly IPatientDetailsView _view;
        private PatientDetailsDisplayMode _patientDetailsDisplayMode;
        public event Action UpdatedPatient;

        public PatientDetailsViewPresenter(IPatientDetailsView view)
        {
            _view = view;
        }

        public void ShowForm(PatientDetailsDisplayMode patientDetailsDisplayMode, Guid patientId)
        {
            _patientDetailsDisplayMode = patientDetailsDisplayMode;
            InitializePresenterAndPatient(patientId);
            _view.ShowDialogForm();
        }

        public void CloseForm()
        {
            _view.CloseDialogForm();
        }

        public void ToggleAnimalFieldAttributes()
        {
            _view.ToggleAnimalAttributes();
        }

        public void SetDateTimeFormat()
        {
            _view.SetDateTimeFormatString();
        }

        public void InitializePresenterAndPatient(Guid patientId)
        {
            _view.AttachPresenter(this);

            if (_patientDetailsDisplayMode == PatientDetailsDisplayMode.AddMode)
            {
                _view.Patient = new Patient(Guid.NewGuid());
            }
            else
            {
                PatientDataAccess patientDataAccess = new PatientDataAccess(new FhirDBContext());
                var patient = patientDataAccess.Get(patientId);
                _view.Patient = patient;
            }
        }

        public void SaveChanges()
        {
            try
            {
                IDataAccess patientDataAccess = new PatientDataAccess(_view.Patient, new FhirDBContext());
                if (_patientDetailsDisplayMode == PatientDetailsDisplayMode.AddMode)
                {
                    patientDataAccess.Add();
                    _view.DisplayOperationCompletedNotification(Operation.AddPatient);

                    Logger.WriteLogToFile(String.Format("User added {0} {1}", _view.Patient.GivenName, _view.Patient.FamilyName));
                }
                else
                {
                    patientDataAccess.Update();
                    _view.DisplayOperationCompletedNotification(Operation.EditPatient);

                    Logger.WriteLogToFile(String.Format("User updated {0} {1}", _view.Patient.GivenName, _view.Patient.FamilyName));
                }

                CloseForm();

                if (UpdatedPatient != null)
                {
                    UpdatedPatient();
                }
            }
            catch (Exception exc)
            {
                Logger.WriteExceptionLogToFile("Save Patient Details", exc);
            }
        }
    }
}
