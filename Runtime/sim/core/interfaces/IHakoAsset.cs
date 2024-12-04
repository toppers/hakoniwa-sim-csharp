using System;
using System.Collections.Generic;

namespace hakoniwa.sim.core
{

    public interface IHakoAsset
    {
        bool Initialize(List<IHakoObject> hakoObectList);
        bool RegisterOnHakoniwa();
        bool UnRegisterOnHakoniwa();
        bool Execute();
    }
}
