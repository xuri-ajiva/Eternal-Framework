using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EternalFramework {
    public class Eternal {
        private Guid _Guid = Guid.NewGuid();
        public Eternal() {

        }
        public string GetID => _Guid.ToString();
    }
}
