using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    class EnemyQuark : Enemy
    {
        public override Dictionary<string, object> Save()
        {
            return base.Save();
        }

        public override void Load(Dictionary<string, object> data)
        {
            base.Load(data);
        }
    }
}
