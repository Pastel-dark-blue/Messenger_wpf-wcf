using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WcfService.ServerClasses
{
    internal class ServerUser
    {
        public long Id { get; set; }

        public OperationContext OperationContext { get; set; }
    }
}
