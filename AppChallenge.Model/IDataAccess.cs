using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Collections;

namespace AppChallenge.Model
{
    public interface IDataAccess
    {
        void Add();
        void Delete(Guid id);
        void Update();
        ICollection GetList(string filter="");
        ICollection SearchExternalDatabase(string nameToSearch);
        void ImportData(ICollection collection, out int patientsAdded, out int duplicatePatients);

    }
}
