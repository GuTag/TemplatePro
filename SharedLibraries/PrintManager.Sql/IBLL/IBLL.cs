using PrintManager.Sql.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Sql.IBLL
{
    public interface IBLL<T>
    {
        bool Add(string item);
        bool AddList(string items);
        bool Delete(string item);
        bool update(string item);

        List<T> GetAll();
        T GetItemForID(int id);

    }
}
