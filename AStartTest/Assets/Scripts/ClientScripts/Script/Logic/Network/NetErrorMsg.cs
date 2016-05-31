using UnityEngine;
using System.Collections;
using comrt.comnet;

public partial class Net
{
    public static bool CheckErrorCode(int err, int cmd)
    {
        return CheckErrorCode(err, (eCommand)cmd);
    }

    public static bool CheckErrorCode(int err, eCommand eCMD)
    {
        eErrorCode eErr = (eErrorCode)err;

        // 调试信息
        if (err == 0) {
            Log.Info("{0} {1}", eCMD.ToString(), "成功");
            return true;
        }

        Log.Info("{0} {1}  ({2}){3}", eCMD.ToString(), "失败", err, eErr.ToString());
        ProcessLoginError(eErr);
        ProcessCityBuildingError(eErr);
        ProcessCommonError(eErr);
        
        return false;
    }

    private static void ProcessLoginError(eErrorCode eErr)
    {
        switch (eErr) {
            // 账号登录
            case eErrorCode.USERNAME_REPEAT:
                // 账号名重复
                UIUtil.ShowErrMsgFormat("MSG_LOGIN_ACCOUNT_REPEAT");
                break;
            case eErrorCode.USER_NAME_NO_EXIST:
                UIUtil.ShowErrMsgFormat("MSG_LOGIN_USER_NAME_NOT_EXIST");
                break;
        }
    }

    private static void ProcessCityBuildingError(eErrorCode eErr)
    {
        // 错误信息
        switch (eErr) {
            case eErrorCode.SOME_SOLIDER_IS_UPGRADING:
                // 升级士兵队列 只能唯一
                UIUtil.ShowErrMsgFormat("MSG_CITY_TRAIN_SOLDIER_BUSY");
                break;
            case eErrorCode.SOLIDER_LEVEL_REACH_MAX:
                // 士兵等级以达到最大值
                UIUtil.ShowErrMsgFormat("UI_CITY_BUILDING_TRAIN_MAX");
                break;
            case eErrorCode.NO_ENOUGH_WOOD:
                // 没有足够的木材  
                UIUtil.ShowErrMsgFormat("MSG_CITY_BUILDING_WOOD_LIMIT");
                break;
            case eErrorCode.NO_ENOUGH_STONE:
                // 石材不够
                UIUtil.ShowErrMsgFormat("MSG_CITY_BUILDING_STONE_LIMIT");
                break;
            case eErrorCode.RESOURCE_BYOND_MAX_CAPACITY:
                // 资源已满
                UIUtil.ShowMsgFormat("MSG_CITY_BUILDING_FULL");
                break;
        }
    }

    private static void ProcessCommonError(eErrorCode eErr)
    {
        // 错误信息
        switch (eErr) {
            case eErrorCode.NO_ENOUGH_GOLD:
                // 没有足够的黄金
                UIUtil.ShowErrMsgFormat("MSG_CITY_BUILDING_MONEY_LIMIT");
                break;
            case eErrorCode.NO_ENOUGH_DIAMOND:
                // 没有足够的钻石
                UIUtil.ShowErrMsgFormat("MSG_CITY_BUILDING_GOLD_LIMIT");
                break;
        }
    }
}
