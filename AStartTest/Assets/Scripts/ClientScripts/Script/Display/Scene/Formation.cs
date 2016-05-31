using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 阵型类别
public enum FormationType
{
    MATRIX,         // 正常方阵 排满一行，继续拍后面的行
    SQUARE,         // 方形 优先满足正方形的条件
    CIRCLE,         // 圆弧 弓箭手包围射击阵型
    ARROW,          // 三角箭矢 骑兵冲锋
}

/*
// 阵型 一个Troop会持有一个阵型
public class Formation
{
    private int _formationId;               // 方阵的配置id
    private int _unitCount;
    private FormationConfig _config;

    private List<Vector3> _formationSlots = new List<Vector3>();    // 阵型中每个位置的坐标(偏移量)
    private List<GameObject> _goSlots = new List<GameObject>();

    private Transform _transform;

    public Formation(Transform troop, int configId, int count)
    {
        _transform = troop;
        ChangeFormation(configId, count);
    }

    // 阵型类别
    public FormationType Type
    {
        get { return (FormationType)_config.type; }
    }

    // 士兵的行间距
    public float RowOffset
    {
        get { return _config.row_offset; }
    }

    // 士兵的列间距
    public float ColOffset
    {
        get { return _config.col_offset; }
    }

    // 每行的最大数目  0为不限制
    public int MaxCountPerRow
    {
        get { return _config.max_count_per_row; }
    }

    // 是否允许自转向（如弓箭手转换方阵方向时只需要把每个角色朝向修改，而不需要修改方阵坐标）
    public int AllowSelfRotation
    {
        get { return _config.self_rotation; }
    }

    // 队伍位置的随机偏移
    public float RandomRowOffset
    {
        get { return _config.random_row; }
    }

    // 队伍位置的随机偏移
    public float RandomColOffset
    {
        get { return _config.random_col; }
    }

    public Transform GetSlot(int index)
    {
        if (index >= _goSlots.Count) {
            return null;
        }

        return _goSlots[index].transform;
    }

    public Vector3 GetSlotPosition(int index)
    {
        if (index >= _goSlots.Count) {
            return Vector3.zero;
        }

        return _goSlots[index].transform.position;
    }

    public void ChangeFormation(int configId, int count)
    {
        _formationId = configId;
        _config = FormationConfigLoader.GetConfig(configId);

        _unitCount = count;

        for (int i = 0; i < _unitCount; ++i) {
            if (i >= _goSlots.Count) {
                GameObject go = new GameObject();
                go.transform.parent = _transform;
                go.transform.position = Vector3.zero;
                _goSlots.Add(go);
            }
        }

        // 更新阵型坐标
        UpdateFormation();
    }

    // 修正阵型位置
    public void UpdateFormation()
    {
        // 计算当前军队数目中每个点的位置
        switch (Type) {
            case FormationType.MATRIX:
                CalcFormationPositionMatrix();
                break;
            case FormationType.SQUARE:
                break;
            case FormationType.CIRCLE:
                break;
            case FormationType.ARROW:
                break;
        }
    }

    // 计算当前阵型中每个阵型位置相对于中心的偏移
    private void CalcFormationPositionMatrix()
    {
        // 最大列数
        int rowCount = Mathf.CeilToInt(1.0f * _unitCount / MaxCountPerRow);
        float startRowOffset = -1.0f * rowCount / 2 * RowOffset;
        float startColOffset = -1.0f * Mathf.Min(MaxCountPerRow, _unitCount) / 2 * ColOffset;
        if (MaxCountPerRow % 2 == 0) {
            startColOffset += ColOffset / 2;
        }

        float currentX = startColOffset;
        float currentY = startRowOffset;
        for (int i = 0; i < rowCount; ++i) {
            for (int j = 0; j < MaxCountPerRow; ++j) {
                int index = i * MaxCountPerRow + j;
                if (index >= _unitCount) {
                    break;
                }

                float xoffset = Random.Range(-RandomColOffset, RandomColOffset);
                float zoffset = Random.Range(-RandomRowOffset, RandomRowOffset);
                Vector3 offset = new Vector3(currentX + xoffset, 0, currentY + zoffset);
                _goSlots[index].transform.localPosition = offset;
                currentX += ColOffset;
            }

            currentX = startColOffset;
            currentY += RowOffset;
        }
    }
}
*/
