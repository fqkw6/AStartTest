﻿using UnityEngine;
using System.Collections;

namespace Lockstep
{
    public class HeightSet : Ability
    {
        [SerializeField]
        private int _mapIndex;

        public int MapIndex { get { return _mapIndex; } }

        [SerializeField]
        private long _bonusHeight;

        private long _offset;

        [Lockstep(true)]
        public long Offset
        {
            get { return _offset; }
            set
            {
                if (_offset != value)
                {
                    _offset = value;
                    ForceUpdate = true;
                }
            }
        }

        public bool ForceUpdate { get; set; }

        protected override void OnSimulate()
        {
            if (Agent.Body.PositionChanged || Agent.Body.PositionChangedBuffer || ForceUpdate)
            {
                long height = HeightmapHelper.Instance.GetHeight(MapIndex, Agent.Body.Position) + _bonusHeight + Offset;
                Agent.Body.HeightPos = height;
            }
        }
    }
}