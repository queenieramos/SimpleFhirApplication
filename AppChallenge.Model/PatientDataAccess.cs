using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hl7.Fhir;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;

namespace AppChallenge.Model
{
    /// <summary>
    /// Patient-related operations.
    /// </summary>
    public class PatientDataAccess : IDataAccess
    {
        private Patient _patient = null;
        private const string PUBLIC_FHIR_TEST_SERVICE_ENDPOINT = "http://fhir2.healthintersections.com.au/open";
        private const int BATCH_SIZE = 50;
        private FhirDBContext _fhirDbContext;

        public PatientDataAccess(FhirDBContext fhirDbContext)
        {
            _fhirDbContext = fhirDbContext;
        }

        public PatientDataAccess(Patient patient, FhirDBContext fhirDbContext)
        {
            _fhirDbContext = fhirDbContext;
            _patient = patient;
        }

        /// <summary>
        /// Add the patient to the database.
        /// </summary>
        public void Add()
        {
            try
            {
                if (_patient != null)
                {
                    using (_fhirDbContext)
                    {
                        _fhirDbContext.Patients.Add(_patient);
                        _fhirDbContext.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Marks the patient as inactive. Does not actually delete from the server.
        /// </summary>
        /// <param name="id">Guid id of the Patient</param>
        public void Delete(Guid id)
        {
            try
            {
                using (_fhirDbContext)
                {
                    var p = (from patient in _fhirDbContext.Patients
                             where patient.Id == id
                             select patient).FirstOrDefault();
                    if (p != null)
                    {
                        p.IsActive = false;
                        _fhirDbContext.Patients.Add(p);
                        var entry = _fhirDbContext.Entry(p);
                        entry.State = System.Data.Entity.EntityState.Modified;
                        _fhirDbContext.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Updates the patient.
        /// </summary>
        public void Update()
        {
            if (_patient != null)
            {
                try
                {
                    using (_fhirDbContext)
                    {
                        _fhirDbContext.Patients.Add(_patient);

                        var entry = _fhirDbContext.Entry(_patient);
                        entry.State = System.Data.Entity.EntityState.Modified;

                        _fhirDbContext.PatientContacts.Add(_patient.PatientContact);
                        var patientContactEntry = _fhirDbContext.Entry(_patient.PatientContact);
                        patientContactEntry.State = System.Data.Entity.EntityState.Modified;

                        _fhirDbContext.PatientIdentifiers.Add(_patient.PatientIdentifier);
                        var patientIdentifierEntry = _fhirDbContext.Entry(_patient.PatientIdentifier);
                        patientIdentifierEntry.State = System.Data.Entity.EntityState.Modified;

                        _fhirDbContext.SaveChanges();
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                
            }
        }

        /// <summary>
        /// Gets the full patient details.
        /// </summary>
        /// <param name="id">Guid Id of the patient.</param>
        public Patient Get(Guid id)
        {
            try
            {
                using (_fhirDbContext)
                {
                    var selectedPatient = (from patient in _fhirDbContext.Patients
                                           where patient.Id == id
                                           select patient).FirstOrDefault();

                    var patientIdentifier = (from identifier in _fhirDbContext.PatientIdentifiers
                                             where identifier.PatientId == id
                                             select identifier).FirstOrDefault();

                    var patientContact = (from contact in _fhirDbContext.PatientContacts
                                          where contact.PatientId == id
                                          select contact).FirstOrDefault();

                    selectedPatient.PatientIdentifier = patientIdentifier;
                    selectedPatient.PatientContact = patientContact;

                    return selectedPatient;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets the list of active patients.
        /// </summary>
        /// <param name="filter">String to search. Will return the whole list if blank.</param>
        public ICollection GetList(string filter = "")
        {
            try
            {
                using (_fhirDbContext)
                {
                    List<Patient> patients = null;

                    if (filter == String.Empty)
                    {
                        patients = (from patient in _fhirDbContext.Patients
                                    where patient.IsActive == true
                                    select patient).ToList();
                    }
                    else
                    {
                        patients = (from patient in _fhirDbContext.Patients
                                    let searchable = patient.FamilyName + "|" + patient.GivenName + "|" +
                                                     patient.PatientGender + "|" + patient.Birthdate.Value + "|" +
                                                     patient.HomePhone + "|" + patient.MobilePhone + "|" +
                                                     patient.Email
                                    where searchable.Contains(filter)
                                    where patient.IsActive == true
                                    select patient).ToList();
                    }

                    return patients;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Searches for the patient's name in the given public FHIR test server.
        /// </summary>
        /// <param name="searchString">String to search</param>
        public ICollection SearchExternalDatabase(string nameToSearch)
        {
            try
            {
                List<Patient> listPatientsSearchResult = new List<Patient>();
                var client = new FhirClient(PUBLIC_FHIR_TEST_SERVICE_ENDPOINT);

                SearchParams p = new SearchParams();

                var query = new string[] { "name=" + nameToSearch };
                var bundle = client.Search("Patient", query);

                foreach (var entry in bundle.Entry)
                {
                    var patient = PatientResourceToPatientMapper.MapToPatient(entry.Resource as Hl7.Fhir.Model.Patient);
                    listPatientsSearchResult.Add(patient);
                }

                return listPatientsSearchResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Checks database for existing patient.
        /// </summary>
        /// <param name="patient"> Patient to be added. </param>
        /// <returns> True if patient already exists, otherwise false.</returns>
        private bool PatientExists(Patient patient)
        {
            using (var context = new FhirDBContext())
            {
                var patientInDatabase = (from p in context.Patients
                                        where p.FamilyName == patient.FamilyName
                                        where p.GivenName == patient.GivenName
                                        where p.Birthdate == patient.Birthdate
                                        where p.Email == patient.Email
                                        where p.HomePhone == patient.HomePhone
                                        where p.MobilePhone == patient.MobilePhone
                                        where p.IsActive == patient.IsActive
                                        select p).FirstOrDefault();

                if (patientInDatabase != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Imports the selected patients from external database to local database.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="patientsAdded"></param>
        /// <param name="duplicatePatients"></param>
        public void ImportData(ICollection collection, out int patientsAdded, out int duplicatePatients)
        {
            patientsAdded = 0;

            try
            {
                using (_fhirDbContext)
                {
                    List<Patient> patients = (List<Patient>)collection;

                    for (var i = 0; i < patients.Count; i++)
                    {
                        if (!PatientExists(patients[i]))
                        {
                            _fhirDbContext.Patients.Add(patients[i]);
                            patientsAdded++;

                            if (patientsAdded % BATCH_SIZE == 0)
                            {
                                // Save in batches of 50.
                                _fhirDbContext.SaveChanges();
                            }
                        }
                    }
                    _fhirDbContext.SaveChanges();
                }

                duplicatePatients = collection.Count - patientsAdded;
            }
            catch (Exception)
            {
                // Let presenter handle errors.
                throw;
            }
        }
    }
}
