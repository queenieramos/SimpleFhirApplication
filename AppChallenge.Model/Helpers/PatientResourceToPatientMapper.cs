using System;
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
    /// Converts FHIR Patient Resource to Patient Model.
    /// </summary>
    public class PatientResourceToPatientMapper
    {
        public static AppChallenge.Model.Patient MapToPatient(Hl7.Fhir.Model.Patient patientResource)
        {
            AppChallenge.Model.Patient patientModel = new AppChallenge.Model.Patient();

            patientModel.Id = Guid.NewGuid();
            patientModel.FamilyName = patientResource.Name[0].Family.FirstOrDefault();
            patientModel.GivenName = patientResource.Name[0].Given.FirstOrDefault();

            // Default to true if value is null.
            patientModel.IsActive = patientResource.Active.HasValue ? patientResource.Active.Value : true;

            if (patientResource.Telecom.Count != 0)
            {
                var telecom = patientResource.Telecom;

                patientModel.HomePhone = telecom.FirstOrDefault(x => x.Use == ContactPoint.ContactPointUse.Home) != null ? 
                                            telecom.First(x => x.Use == ContactPoint.ContactPointUse.Home).Value : null;

                patientModel.MobilePhone = telecom.FirstOrDefault(x => x.Use == ContactPoint.ContactPointUse.Mobile) != null ?
                                           telecom.First(x => x.Use == ContactPoint.ContactPointUse.Mobile).Value : null;

                patientModel.Email = telecom.FirstOrDefault(x => x.System == ContactPoint.ContactPointSystem.Email) != null ?
                                        telecom.First(x => x.System == ContactPoint.ContactPointSystem.Email).Value : null;
            }

            patientModel.PatientGender = patientResource.Gender.HasValue ? patientResource.Gender.Value.ToString() : null;

            if (patientResource.BirthDate != null)
            {
                patientModel.Birthdate = DateTime.Parse(patientResource.BirthDate);
            }

            if (patientResource.Deceased != null)
            {
                var patientResourceDeceasedBoolean = patientResource.Deceased as FhirBoolean;

                if (patientResourceDeceasedBoolean != null)
                {
                    patientModel.IsDeceased = patientResourceDeceasedBoolean.Value != null ? 
                                                patientResourceDeceasedBoolean.Value.Value : false;
                }
                else
                {
                    var patientResourceDeceasedDateTime = patientResource.Deceased as FhirDateTime;
                    if (!String.IsNullOrEmpty(patientResourceDeceasedDateTime.Value))
                    {
                        // Set to true if Deceased datetime has a value.
                        patientModel.IsDeceased = true;
                    }
                }
            }
            else
            {
                // Default to false if Deceased property is null.
                patientModel.IsDeceased = false;
            }

            if (patientResource.Address.Count != 0)
            {
                var address = patientResource.Address.First();

                patientModel.StreetAddress = address.Line != null ? address.Line.FirstOrDefault() : null;
                patientModel.City = address.City != null ? address.City : null;
                patientModel.State = address.State != null ? address.State : null;
                patientModel.PostalCode = address.PostalCode != null ? address.PostalCode : null;
                patientModel.Country = address.Country != null ? address.Country : null;
            }

            patientModel.PatientMaritalStatus = patientResource.MaritalStatus != null ? 
                                                    patientResource.MaritalStatus.Text : null;

            if (patientResource.Animal != null)
            {
                patientModel.IsAnimal = true;

                if (patientResource.Animal.Breed != null)
                {
                    var breedCoding = patientResource.Animal.Breed.Coding.FirstOrDefault();
                                        patientModel.AnimalBreed = breedCoding != null ? breedCoding.Display : null;
                }

                if (patientResource.Animal.Species != null)
                {
                    var speciesCoding = patientResource.Animal.Species.Coding.FirstOrDefault();
                    patientModel.AnimalSpecies = speciesCoding != null ? speciesCoding.Display : null;
                }

                if (patientResource.Animal.GenderStatus != null)
                {
                    var genderStatusCoding = patientResource.Animal.GenderStatus.Coding.FirstOrDefault();
                    patientModel.AnimalGenderStatus = genderStatusCoding != null ? genderStatusCoding.Code : null;
                }
            }
            else
            {
                patientModel.IsAnimal = false;
            }

            patientModel.PatientContact = new PatientContact(patientModel.Id);

            if (patientResource.Contact.Count != 0)
            {
                var contact = patientResource.Contact.First();

                patientModel.PatientContact.FamilyName = contact.Name.Family.FirstOrDefault();
                patientModel.PatientContact.GivenName = contact.Name.Given.FirstOrDefault();

                if (contact.Relationship.Count != 0)
                {
                    var relationship = contact.Relationship.FirstOrDefault();
                    patientModel.PatientContact.Relationship = relationship.Coding.Count != 0 ? 
                                                                relationship.Coding.FirstOrDefault().Code : null;
                }

                if (contact.Telecom.Count != 0)
                {
                    var telecom = contact.Telecom;

                    patientModel.PatientContact.HomePhone = telecom.FirstOrDefault(x => x.Use == ContactPoint.ContactPointUse.Home) != null ?
                                                                telecom.FirstOrDefault(x => x.Use == ContactPoint.ContactPointUse.Home).Value : null;

                    patientModel.PatientContact.MobilePhone = telecom.FirstOrDefault(x => x.Use == ContactPoint.ContactPointUse.Mobile) != null ? 
                                                                telecom.FirstOrDefault(x => x.Use == ContactPoint.ContactPointUse.Mobile).Value : null;
                }
            }

            patientModel.PatientIdentifier = new PatientIdentifier(patientModel.Id);

            if (patientResource.Identifier.Count != 0)
            {
                var identifier = patientResource.Identifier.FirstOrDefault();

                patientModel.PatientIdentifier.IdentifierUseValue = identifier.Use.HasValue ? identifier.Use.Value.ToString() : null;

                if (identifier.Type != null)
                {
                    patientModel.PatientIdentifier.IdentifierTypeCodeValue = identifier.Type.Coding.Count != 0 ? 
                                                                                identifier.Type.Coding.FirstOrDefault().Code 
                                                                                : null;
                    patientModel.PatientIdentifier.IdentifierTypeTextValue = identifier.Type.Text;
                }
                
                patientModel.PatientIdentifier.IdentifierSystemValue = identifier.System;
                patientModel.PatientIdentifier.IdentifierValue = identifier.Value;
                patientModel.PatientIdentifier.AssignerDisplayValue = identifier.Assigner != null ? 
                                                                        identifier.Assigner.Display : null;
            }

            return patientModel;
        }
    }
}
