using AppChallenge.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppChallenge.Model;
using System.Collections;
using System.Reactive.Linq;
using System.Diagnostics;
using System.Threading;
using AppChallenge.Logging;

namespace AppChallenge.UI
{
    public class PatientMainViewPresenter : IPatientMainViewPresenter
    {
        private readonly IPatientMainView _view;
        private IPatientDetailsViewPresenter _patientdetailsViewPresenter;

        private const string SEARCH_RESULTS_FROM_EXTERNAL_DATABASE = "Search Results from external database";
        private const string SEARCH_RESULTS_FROM_LOCAL_DATABASE = "Search Results from local database";
        private const string RECORDS_FROM_LOCAL_DATABASE = "Records from local database";

        public PatientMainViewPresenter(IPatientMainView view)
        {
            _view = view;
            SetPatientList();
        }

        public void Start()
        {
            _view.AttachPresenter(this);
        }

        private void SetPatientList()
        {
            PatientDataAccess patientDataAccess = new PatientDataAccess(new FhirDBContext());
            _view.PatientList = (List<Patient>)patientDataAccess.GetList();
            _view.CurrentView = RECORDS_FROM_LOCAL_DATABASE;
            _view.TotalRecords = _view.PatientList.Count().ToString();
            _view.EnableDisableButtons();
        }

        public void DisplayPatientDetailsForm(PatientDetailsDisplayMode patientDetailsDisplayMode)
        {
            _patientdetailsViewPresenter = new PatientDetailsViewPresenter(new PatientDetailsView());
            _patientdetailsViewPresenter.UpdatedPatient += RefreshGrid;

            _patientdetailsViewPresenter.ShowForm(patientDetailsDisplayMode, _view.SelectedPatientId);
        }

        public void RefreshGrid()
        {
            SetPatientList();
        }

        public void DeletePatient()
        {
            try
            {
                PatientDataAccess patientDataAccess = new PatientDataAccess(new FhirDBContext());
                patientDataAccess.Delete(_view.SelectedPatientId);
                _view.DisplayOperationCompletedNotification(Operation.DeletePatient);
                RefreshGrid();

                Logger.WriteLogToFile("User deleted a patient.");
            }
            catch (Exception exc)
            {
                Logger.WriteExceptionLogToFile("Delete Patient", exc);
                _view.DisplayErrorNotification(exc);
            }

        }

        public void SearchPatient()
        {
            try
            {
                PatientDataAccess patientDataAccess = new PatientDataAccess(new FhirDBContext());

                switch (_view.SelectedDatabase)
                {
                    case DatabaseSearchOption.Local:

                        Logger.WriteLogToFile(String.Format("User searches for \"{0}\" in local database.", _view.SearchString));

                        _view.PatientList = (List<Patient>)patientDataAccess.GetList(_view.SearchString);

                        if (_view.SearchString == String.Empty)
                        {
                            //resets the list
                            _view.CurrentView = RECORDS_FROM_LOCAL_DATABASE;
                        }
                        else
                        {
                            _view.CurrentView = SEARCH_RESULTS_FROM_LOCAL_DATABASE;
                        }
                        break;

                    case DatabaseSearchOption.External:

                        Logger.WriteLogToFile(String.Format("User searches for \"{0}\" in external database.", _view.SearchString));

                        // Need to check since an exception will be thrown if an empty string is passed.
                        if (_view.SearchString != String.Empty)
                        {
                            _view.PatientList = (List<Patient>)patientDataAccess.SearchExternalDatabase(_view.SearchString);
                            _view.CurrentView = SEARCH_RESULTS_FROM_EXTERNAL_DATABASE;
                        }
                        break;
                }

                // Set the label for displaying the no. of records.
                _view.TotalRecords = _view.PatientList.Count().ToString();
                _view.EnableDisableButtons();
                
            }
            catch (Exception exc)
            {
                Logger.WriteExceptionLogToFile("Search Patient", exc);
                _view.DisplayErrorNotification(exc);
            }
        }

        public void ImportPatientData()
        {
            try
            {
                Logger.WriteLogToFile(String.Format("User tries to import {0} records from external database to local database.", _view.PatientImportList.Count));

                var patientDataAccess = new PatientDataAccess(new FhirDBContext());
                var patientsAdded = 0;
                var duplicatePatients = 0;
                patientDataAccess.ImportData(_view.PatientImportList, out patientsAdded, out duplicatePatients);

                string customMessage = String.Empty;
                if (duplicatePatients > 0 && patientsAdded > 0)
                {
                    customMessage = String.Format("Import Successful!\n{0} patient(s) added.\n{1} patient(s) already exist(s) in the database.",
                                                    patientsAdded, duplicatePatients);
                }
                else if (duplicatePatients > 0 && patientsAdded == 0)
                {
                    customMessage = String.Format("{0} patient(s) added.\n{1} patient(s) already exist(s) in the database.",
                                                    patientsAdded, duplicatePatients);
                }
                else
                {
                    customMessage = String.Format("Import Successful!\n{0} patient(s) added.", patientsAdded);
                }

                Logger.WriteLogToFile(String.Format("User has successfully imported {0} records from external database to local database. {1} duplicate patient(s) found.", 
                                                        patientsAdded, duplicatePatients));
                
                _view.DisplayOperationCompletedNotification(Operation.ImportPatient, customMessage);
            }
            catch (Exception exc)
            {
                Logger.WriteExceptionLogToFile("Import data", exc);
                _view.DisplayErrorNotification(exc);
            }
            
        }
    }
}
