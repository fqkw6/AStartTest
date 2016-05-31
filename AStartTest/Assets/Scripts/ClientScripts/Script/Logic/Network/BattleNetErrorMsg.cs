using UnityEngine;
using System.Collections;
using ProtoBuf;
using message;

public partial class Net
{
    public static bool CheckErrorCode(ErrorCode err, MSG msg)
    {
        // 调试信息
        if (err == ErrorCode.OK) {
            if (msg != MSG.SYN_TIMESTAMP) {
                Log.Info("----{0} {1}", msg.ToString(), "成功");
            }
            return true;
        }

        Log.Error("----{0} {1}  ({2})", msg.ToString(), "失败", err);

        // 特殊的错误进行特殊的提示
        switch (err) {
                
        }
        return false;
    }
}
