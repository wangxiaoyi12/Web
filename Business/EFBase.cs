using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class EFBase<T> : RepositoryBase<T, DataBase.DbMallEntities> where T : class
    {
    }
}
