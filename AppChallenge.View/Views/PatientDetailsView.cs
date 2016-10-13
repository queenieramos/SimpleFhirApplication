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
using System.Collections;

namespace AppChallenge.UI
{
    public partial class PatientDetailsView : Form, IPatientDetailsView
    {
        private Patient _patient;
        private IPatientDetailsViewPresenter _presenter;
        private const string BLANK_VALUE = "0";
        private const string CUSTOM_DATE_TIME_FORMAT = "dd/MM/yyyy";

        public PatientDetailsView()
        {
            InitializeComponent();
            InitializeComboboxBindings();
        }

        public Patient Patient
        {
            get
            {
                return _patient;
            }
            set
            {
                _patient = value;
                BindProperties();
            }
        }

        private void BindProperties()
        {
            if (_patient.IsActive == true)
            {
                radioBtnYesActive.Checked = true;
            }
            else
            {
                radioBtnNoActive.Checked = true;
            }

            if (_patient.IsAnimal == true)
            {
                radioBtnYesAnimal.Checked = true;
            }
            else
            {
                radioBtnNoAnimal.Checked = true;
            }

            txtFamilyName.Text = _patient.FamilyName;
            txtGivenName.Text = _patient.GivenName;
            if (_patient.Birthdate.HasValue)
            {
                dateTimePickerBirthdate.Value = _patient.Birthdate.Value;
            }

            if (_patient.PatientGender != null)
            {
                cboGender.SelectedValue = _patient.PatientGender;
            }

            if (_patient.PatientMaritalStatus != null)
            {
                cboMaritalStatus.SelectedValue = _patient.PatientMaritalStatus;
            }
            
            if (_patient.IsDeceased == true)
            {
                radioBtnYesDeceased.Checked = true;
            }
            else
            {
                radioBtnNoDeceased.Checked = true;
            }
            txtHomePhone.Text = _patient.HomePhone;
            txtMobilePhone.Text = _patient.MobilePhone;
            txtEmail.Text = _patient.Email;
            txtStreetAddress.Text = _patient.StreetAddress;
            txtCity.Text = _patient.City;
            txtState.Text = _patient.State;

            if (_patient.Country != null)
            {
                cboCountry.SelectedText = _patient.Country;
            }

            txtPostalCode.Text = _patient.PostalCode;

            txtAnimalBreed.Text = _patient.AnimalBreed;
            txtAnimalSpecies.Text = _patient.AnimalSpecies;

            if (_patient.AnimalGenderStatus != null)
            {
                cboGenderStatus.SelectedValue = _patient.AnimalGenderStatus;
            }

            if (_patient.PatientIdentifier.IdentifierUseValue != null)
            {
                cboUse.SelectedValue = _patient.PatientIdentifier.IdentifierUseValue;
            }
            
            if (_patient.PatientIdentifier.IdentifierTypeCodeValue != null)
            {
                cboTypeCode.SelectedValue = _patient.PatientIdentifier.IdentifierTypeCodeValue;

                // This will be null if the imported patient identifier type code does not exists in the app.config.
                if (cboTypeCode.SelectedValue == null)
                {
                    // Set to default.
                    cboTypeCode.SelectedIndex = 0;
                }
            }

            txtTypeText.Text = _patient.PatientIdentifier.IdentifierTypeTextValue;
            txtSystemValue.Text = _patient.PatientIdentifier.IdentifierSystemValue;
            txtIdentifierValue.Text = _patient.PatientIdentifier.IdentifierValue;
            txtAssigner.Text = _patient.PatientIdentifier.AssignerDisplayValue;

            txtContactFamilyName.Text = _patient.PatientContact.FamilyName;
            txtContactGivenName.Text = _patient.PatientContact.GivenName;
            txtContactHomePhone.Text = _patient.PatientContact.HomePhone;
            txtContactMobilePhone.Text = _patient.PatientContact.MobilePhone;

            if (_patient.PatientContact.Relationship != null)
            {
                cboRelationship.SelectedValue = _patient.PatientContact.Relationship;
            }
        }

        public void InitializeComboboxBindings()
        {
            SetComboboxBinding(cboCountry, ListHelper.ListCountries);
            SetComboboxBinding(cboGender, ListHelper.DictionaryGender);
            SetComboboxBinding(cboGenderStatus, ListHelper.DictionaryGenderStatus);
            SetComboboxBinding(cboMaritalStatus, ListHelper.DictionaryMaritalStatus);
            SetComboboxBinding(cboRelationship, ListHelper.DictionaryContactRelationship);
            SetComboboxBinding(cboTypeCode, ListHelper.DictionaryIdentifierTypeCode);
            SetComboboxBinding(cboUse, ListHelper.DictionaryIdentifierUse);
        }

        private void SetComboboxBinding(ComboBox cboBox, ICollection collection)
        {
            if (collection.GetType() == typeof(List<string>))
            {
                cboBox.DataSource = collection;
            }
            else
            {
                cboBox.DataSource = new BindingSource(collection, null);
                cboBox.DisplayMember = "Value";
                cboBox.ValueMember = "Key";
            }
        }

        public void ShowDialogForm()
        {
            ShowDialog();
        }

        public void CloseDialogForm()
        {
            Close();
        }

        public void AttachPresenter(PatientDetailsViewPresenter patientDetailsViewPresenter)
        {
            _presenter = patientDetailsViewPresenter;
        }

        public void DisplayOperationCompletedNotification(Operation operation, string customMessage = "")
        {
            switch (operation)
            {
                case Operation.AddPatient:
                    MessageBox.Show("Patient added successfully!", "AppChallenge - Add Patient", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
                case Operation.EditPatient:
                    MessageBox.Show("Changes saved.", "AppChallenge - Edit Patient", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    break;
            }
        }

        public void DisplayErrorNotification(Exception exception)
        {
            MessageBox.Show(String.Format("Error Encountered: {0}", exception.Message), "AppChallenge - Error Encountered", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public void ToggleAnimalAttributes()
        {
            if (radioBtnNoAnimal.Checked)
            {
                txtAnimalSpecies.Text = String.Empty;
                txtAnimalSpecies.Enabled = false;

                txtAnimalBreed.Text = String.Empty;
                txtAnimalBreed.Enabled = false;

                cboGenderStatus.SelectedIndex = 0;
                cboGenderStatus.Enabled = false;
            }
            else
            {
                txtAnimalSpecies.Enabled = true;

                txtAnimalBreed.Enabled = true;

                cboGenderStatus.Enabled = true;
            }
        }

        public void SetDateTimeFormatString()
        {
            dateTimePickerBirthdate.CustomFormat = CUSTOM_DATE_TIME_FORMAT;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (radioBtnYesActive.Checked == true)
            {
                _patient.IsActive = true;
            }
            else
            {
                _patient.IsActive = false;
            }

            if (radioBtnYesAnimal.Checked == true)
            {
                _patient.IsAnimal = true;
            }
            else
            {
                _patient.IsAnimal = false;
            }

            _patient.FamilyName = txtFamilyName.Text;
            _patient.GivenName = txtGivenName.Text;

            if (dateTimePickerBirthdate.Text != " ")
            {
                _patient.Birthdate = dateTimePickerBirthdate.Value;
            }

            if (cboGender.SelectedValue.ToString() != BLANK_VALUE)
            {
                _patient.PatientGender = cboGender.SelectedValue.ToString();
            }

            if (cboMaritalStatus.SelectedValue.ToString() != BLANK_VALUE)
            {
                _patient.PatientMaritalStatus = cboMaritalStatus.SelectedValue.ToString();
            }

            if (radioBtnYesDeceased.Checked == true)
            {
                _patient.IsDeceased = true;
            }
            else
            {
                _patient.IsDeceased = false;
            }
            _patient.HomePhone = txtHomePhone.Text;
            _patient.MobilePhone = txtMobilePhone.Text;
            _patient.Email = txtEmail.Text;
            _patient.StreetAddress = txtStreetAddress.Text;
            _patient.City = txtCity.Text;
            _patient.State = txtState.Text;

            if (cboCountry.SelectedValue.ToString() != String.Empty)
            {
                _patient.Country = cboCountry.SelectedValue.ToString();
            }

            _patient.PostalCode = txtPostalCode.Text;

            if (_patient.IsAnimal)
            {
                _patient.AnimalBreed = txtAnimalBreed.Text;
                _patient.AnimalSpecies = txtAnimalSpecies.Text;

                if (cboGenderStatus.SelectedValue.ToString() != BLANK_VALUE)
                {
                    _patient.AnimalGenderStatus = cboGenderStatus.SelectedValue.ToString();
                }
            }

            if (cboUse.SelectedValue.ToString() != BLANK_VALUE)
            {
                _patient.PatientIdentifier.IdentifierUseValue = cboUse.SelectedValue.ToString();
            }

            if (cboTypeCode.SelectedValue.ToString() != BLANK_VALUE)
            {
                _patient.PatientIdentifier.IdentifierTypeCodeValue = cboTypeCode.SelectedValue.ToString();
            }

            _patient.PatientIdentifier.IdentifierTypeTextValue = txtTypeText.Text;
            _patient.PatientIdentifier.IdentifierSystemValue = txtSystemValue.Text;
            _patient.PatientIdentifier.IdentifierValue = txtIdentifierValue.Text;
            _patient.PatientIdentifier.AssignerDisplayValue = txtAssigner.Text;

            _patient.PatientContact.FamilyName = txtContactFamilyName.Text;
            _patient.PatientContact.GivenName = txtContactGivenName.Text;
            _patient.PatientContact.HomePhone = txtContactHomePhone.Text;
            _patient.PatientContact.MobilePhone = txtContactMobilePhone.Text;

            if (cboRelationship.SelectedValue.ToString() != BLANK_VALUE)
            {
                _patient.PatientContact.Relationship = cboRelationship.SelectedValue.ToString();
            }
            _presenter.SaveChanges();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _presenter.CloseForm();
        }

        private void dateTimePickerBirthdate_ValueChanged(object sender, EventArgs e)
        {
            // Set date format when a value is selected from the calendar.
            _presenter.SetDateTimeFormat();
        }

        private void radioBtnAnimal_CheckedChanged(object sender, EventArgs e)
        {
            // Enable or Disable Animal-related fields.
            _presenter.ToggleAnimalFieldAttributes();
        }
    }
}
