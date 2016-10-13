using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AppChallenge.Model;
using System.Reactive.Linq;
using System.Diagnostics;
using System.Threading;

namespace AppChallenge.UI
{
    public partial class PatientMainView : Form, IPatientMainView
    {
        private List<Patient> _patientList;
        private List<Patient> _patientImportList;
        private IPatientMainViewPresenter _presenter;
        private Guid _selectedPatientId;
        private string _searchString;
        private DatabaseSearchOption _selectedDatabase;
        private string _totalRecords;
        private string _currentView;

        public PatientMainView()
        {
            InitializeComponent();

            BindDatabaseOptions();
            BindEvents();
        }

        private void BindEvents()
        {
            // Trigger search patient every after 3 seconds from text change.
            OnSearchTextChanged.Sample(TimeSpan.FromSeconds(3))
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(x => SearchPatient());
        }

        private void SearchPatient()
        {
            SearchString = txtSearch.Text;
            SelectedDatabase = (DatabaseSearchOption)Enum.Parse(typeof(DatabaseSearchOption), cboBoxSearchOption.SelectedValue.ToString());
            
            _presenter.SearchPatient();
        }

        /// <summary>
        /// Enable/Disable buttons depending on the selected database.
        /// </summary>
        public void EnableDisableButtons()
        {
            switch (SelectedDatabase)
            {
                case DatabaseSearchOption.External:
                    btnAdd.Enabled = false;
                    btnDelete.Enabled = false;
                    btnEdit.Enabled = false;
                    btnImport.Enabled = true;
                    break;
                case DatabaseSearchOption.Local:
                    btnAdd.Enabled = true;
                    btnDelete.Enabled = true;
                    btnEdit.Enabled = true;
                    btnImport.Enabled = false;
                    break;
            }
        }

        private void BindDatabaseOptions()
        {
            cboBoxSearchOption.DataSource = Enum.GetValues(typeof(DatabaseSearchOption));
        }

        public List<Patient> PatientList
        {
            get
            {
                return _patientList;
            }
            set
            {
                _patientList = value;

                PopulateGridView();
            }
        }

        private void PopulateGridView()
        {
            dgPatients.Rows.Clear();
            dgPatients.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            switch (_selectedDatabase)
            {
                case DatabaseSearchOption.Local:
                     dgPatients.MultiSelect = false;
                    break;

                case DatabaseSearchOption.External:
                    // Enable multiselect so you can import mulitple records.
                    dgPatients.MultiSelect = true;
                    break;
            }

            foreach (Patient p in _patientList)
            {
                dgPatients.Rows.Add(p.Id, p.FamilyName, p.GivenName, p.PatientGender, p.Birthdate, p.HomePhone, p.MobilePhone, p.Email);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _presenter.DisplayPatientDetailsForm(PatientDetailsDisplayMode.AddMode);
        }

        public void ShowForm()
        {
            Application.Run(this);
        }


        public void AttachPresenter(PatientMainViewPresenter presenter)
        {
            _presenter = presenter;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgPatients.SelectedRows.Count > 0)
            {
                SelectedPatientId = (Guid)dgPatients.SelectedRows[0].Cells[0].Value;
                _presenter.DisplayPatientDetailsForm(PatientDetailsDisplayMode.EditMode);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgPatients.SelectedRows.Count > 0)
            {
                SelectedPatientId = (Guid)dgPatients.SelectedRows[0].Cells[0].Value;
                _presenter.DeletePatient();
            }
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            PatientImportList = new List<Patient>();

            for (var i = 0; i < dgPatients.SelectedRows.Count; i++)
            {
                var selectedPatientId = Guid.Parse(dgPatients.SelectedRows[i].Cells[0].Value.ToString());
                Patient selectedPatient = PatientList.First(x => x.Id == selectedPatientId);

                PatientImportList.Add(selectedPatient);
            }

            _presenter.ImportPatientData();

            // Clear selection then select first row as default selected row.
            dgPatients.ClearSelection();
            dgPatients.Rows[0].Selected = true;
        }

        public Guid SelectedPatientId
        {
            get
            {
                return _selectedPatientId;
            }
            set
            {
                _selectedPatientId = value;
            }
        }

        public string SearchString
        {
            get
            {
                SearchString = txtSearch.Text;
                return _searchString;
            }
            set
            {
                _searchString = value;
            }
        }

        public DatabaseSearchOption SelectedDatabase
        {
            get
            {
                return _selectedDatabase;
            }
            set
            {
                _selectedDatabase = value;
            }
        }

        public string TotalRecords
        {
            get
            {
                return _totalRecords;
            }
            set
            {
                _totalRecords = value;
                lblNumberOfRecords.Text = _totalRecords;
            }
        }

        public string CurrentView
        {
            get
            {
                return _currentView;
            }
            set
            {
                _currentView = value;
                lblViewingDescription.Text = _currentView;
            }
        }


        public List<Patient> PatientImportList
        {
            get
            {
                return _patientImportList;
            }
            set
            {
                _patientImportList = value;
            }
        }


        public void DisplayOperationCompletedNotification(Operation operation, string customMessage = "")
        {
            switch (operation)
            {
                case Operation.DeletePatient:
                    MessageBox.Show("Patient deleted successfully.", "AppChallenge - Delete Patient", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case Operation.ImportPatient:
                    MessageBox.Show(customMessage, "AppChallenge - Import Patient", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        public void DisplayErrorNotification(Exception exception)
        {
            MessageBox.Show(String.Format("Error Encountered: {0}", exception.Message), "AppChallenge - Error Encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public IObservable<EventArgs> OnSearchTextChanged
        {
            get
            {
                return Observable.FromEventPattern<EventHandler, EventArgs>(
                    h => this.txtSearch.TextChanged += h,
                    h => this.txtSearch.TextChanged -= h).Select(x => x.EventArgs);
            }
        }
        
    }
}
