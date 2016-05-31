using UnityEngine;
using System.Collections.Generic;

public class TestFormation : MonoBehaviour {
    public int Count = 15;
    public List<GameObject> _prefabList;
    public int MaxCountPerRow = 5;
    public float RowOffset = 1;
    public float ColOffset = 1;

    private List<GameObject> _goSlots = new List<GameObject>();

	void Start () {
	    foreach (var item in _prefabList) {
            List<GameObject> soldierList = new List<GameObject>();

            for (int i = 0; i < Count; ++i) {
                GameObject go = Instantiate(item);
                go.transform.parent = item.transform.parent;
                soldierList.Add(go);
            }

            UpdateFormation(soldierList, item.transform.position);
	    }
	}
    
    private void UpdateFormation(List<GameObject> soldierList, Vector3 basePos)
    {
        // 最大列数
        int rowCount = Mathf.CeilToInt(1.0f * Count / MaxCountPerRow);
        float startRowOffset = -1.0f * rowCount / 2 * RowOffset;
        float startColOffset = -1.0f * Mathf.Min(MaxCountPerRow, Count) / 2 * ColOffset;
        if (MaxCountPerRow % 2 == 0) {
            startColOffset += ColOffset / 2;
        }

        float currentX = startColOffset;
        float currentY = startRowOffset;
        for (int i = 0; i < rowCount; ++i) {
            for (int j = 0; j < MaxCountPerRow; ++j) {
                int index = i * MaxCountPerRow + j;
                if (index >= Count) {
                    break;
                }

                Vector3 offset = new Vector3(currentX, 0, currentY);
                soldierList[index].transform.localPosition = offset + basePos;
                currentX += ColOffset;
            }

            currentX = startColOffset;
            currentY += RowOffset;
        }
    }
}
