using PrintManager.Sql.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrintManager.Sql.IBLL
{
    public interface IProductOrderBLL: PrintManager.Sql.IBLL.IBLL<ProductOrder>
    {
        ProductOrder GetItemForDesc(string desc);
        ProductOrder GetItemForMO(string desc);
    }
}
