using UnityEngine;
using System.Collections.Generic;
using comrt.comnet;

// 士兵数据
public class SoldierInfo
{
    public int ConfigID;
    public SoldierConfig _cfg;

    public SoldierConfig Cfg
    {
        get {
            if (_cfg == null) {
                _cfg = SoldierConfigLoader.GetConfig(ConfigID);
            }

            return _cfg;
        }    
    }

    public void Deserialize(PSolider data)
    {
        
    }
}
