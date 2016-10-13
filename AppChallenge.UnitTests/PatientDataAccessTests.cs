using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data.Entity;
using AppChallenge.Model;
using System.Linq;
using System.Collections.Generic;

namespace AppChallenge.UnitTests
{
    [TestClass]
    public class PatientDataAccessTests
    {
        private Mock<DbSet<Patient>> _mockSet;
        private Mock<FhirDBContext> _mockContext;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockSet = new Mock<DbSet<Patient>>();

            _mockContext = new Mock<FhirDBContext>();
            _mockContext.Setup(m => m.Patients).Returns(_mockSet.Object);
        }

        [TestMethod]
        public void Add_PatientViaContext_ShouldBeSaved()
        {
            var patient = new Patient() { Id=Guid.NewGuid(), FamilyName= "Test" };

            var dataAccess = new PatientDataAccess(patient, _mockContext.Object);
            dataAccess.Add();

            _mockSet.Verify(m => m.Add(It.IsAny<Patient>()), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void Delete_Patient_ChangesSaved()
        {
            var idToDelete = Guid.NewGuid();

            var data = new List<Patient>
            {
                new Patient { Id = idToDelete, IsActive = true }
            }.AsQueryable();

            _mockSet.As<IQueryable<Patient>>().Setup(m => m.Provider).Returns(data.Provider);
            _mockSet.As<IQueryable<Patient>>().Setup(m => m.Expression).Returns(data.Expression);
            _mockSet.As<IQueryable<Patient>>().Setup(m => m.ElementType).Returns(data.ElementType);
            _mockSet.As<IQueryable<Patient>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var dataAccess = new PatientDataAccess(_mockContext.Object);
            dataAccess.Delete(idToDelete);

            _mockSet.Verify(m => m.Add(It.Is<Patient>(x=>x.Id == idToDelete)), Times.Once());
            _mockContext.Verify(m => m.SaveChanges(), Times.Once());
        }

        [TestMethod]
        public void GetList_DisplayListOfPatients_ShouldNotReturnInactivePatients()
        {
            var patients = new List<Patient>
            {
                new Patient { Id = Guid.NewGuid(), IsActive = true },
                new Patient { Id = Guid.NewGuid(), IsActive = true },
                new Patient { Id = Guid.NewGuid(), IsActive = true },
                new Patient { Id = Guid.NewGuid(), IsActive = false }
            }.AsQueryable();

            _mockSet.As<IQueryable<Patient>>().Setup(m => m.Provider).Returns(patients.Provider);
            _mockSet.As<IQueryable<Patient>>().Setup(m => m.Expression).Returns(patients.Expression);
            _mockSet.As<IQueryable<Patient>>().Setup(m => m.ElementType).Returns(patients.ElementType);
            _mockSet.As<IQueryable<Patient>>().Setup(m => m.GetEnumerator()).Returns(patients.GetEnumerator());

            var dataAccess = new PatientDataAccess(_mockContext.Object);
            var patientList = dataAccess.GetList();

            Assert.AreEqual(3, patientList.Count);
        }

        [TestMethod]
        public void GetList_SearchNonExistingPatient_ShouldReturnEmpty()
        {
            var patients = new List<Patient>
            {
                new Patient { Id = Guid.NewGuid(), FamilyName="Doe", 
                                GivenName="John", IsActive = true, PatientGender = "male",
                                Birthdate = DateTime.Now, HomePhone = "", MobilePhone = "", Email = ""},
                new Patient { Id = Guid.NewGuid(), FamilyName="Doe", 
                                GivenName="Jane", IsActive = true, PatientGender = "female",
                                Birthdate = DateTime.Now, HomePhone = "", MobilePhone = "", Email = ""},
                new Patient { Id = Guid.NewGuid(), FamilyName="Doe", 
                                GivenName="Joe", IsActive = true, PatientGender = "male",
                                Birthdate = DateTime.Now, HomePhone = "", MobilePhone = "", Email = ""}
            }.AsQueryable();

            _mockSet.As<IQueryable<Patient>>().Setup(m => m.Provider).Returns(patients.Provider);
            _mockSet.As<IQueryable<Patient>>().Setup(m => m.Expression).Returns(patients.Expression);
            _mockSet.As<IQueryable<Patient>>().Setup(m => m.ElementType).Returns(patients.ElementType);
            _mockSet.As<IQueryable<Patient>>().Setup(m => m.GetEnumerator()).Returns(patients.GetEnumerator());

            var dataAccess = new PatientDataAccess(_mockContext.Object);

            var patientList = dataAccess.GetList("Adam");

            Assert.AreEqual(0, patientList.Count);
        }

        [TestMethod]
        public void GetList_SearchForExistingPatient_ShouldReturnMatchedPatients()
        {
            var patients = new List<Patient>
            {
                new Patient { Id = Guid.NewGuid(), FamilyName="Doe", 
                                GivenName="John", IsActive = true, PatientGender = "male",
                                Birthdate = DateTime.Now, HomePhone = "", MobilePhone = "", Email = ""},
                new Patient { Id = Guid.NewGuid(), FamilyName="Doe", 
                                GivenName="Jane", IsActive = true, PatientGender = "female",
                                Birthdate = DateTime.Now, HomePhone = "", MobilePhone = "", Email = ""}
            }.AsQueryable();

            _mockSet.As<IQueryable<Patient>>().Setup(m => m.Provider).Returns(patients.Provider);
            _mockSet.As<IQueryable<Patient>>().Setup(m => m.Expression).Returns(patients.Expression);
            _mockSet.As<IQueryable<Patient>>().Setup(m => m.ElementType).Returns(patients.ElementType);
            _mockSet.As<IQueryable<Patient>>().Setup(m => m.GetEnumerator()).Returns(patients.GetEnumerator());

            var dataAccess = new PatientDataAccess(_mockContext.Object);

            var patientList = dataAccess.GetList("Jane");

            Assert.AreEqual(1, patientList.Count);
        }
        
    }
}
