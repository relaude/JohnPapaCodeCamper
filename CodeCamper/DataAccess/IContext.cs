using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Breeze.WebApi;
using Newtonsoft.Json.Linq;

namespace DataAccess
{
    public interface IContext : IDisposable
    {
        //default
        int SaveModifications();
        void SetModified(object entity);
        void SetAdd(object entity);

        //Breeeze Web Api
        string Metadata();
        SaveResult SaveChanges(JObject saveBundle);
    }
}
