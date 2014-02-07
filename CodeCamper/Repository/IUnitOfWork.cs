using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess;

namespace Repository
{
    public interface IUnitOfWork : IDisposable
    {
        int Save();
        IContext Context { get; }
    }
}
