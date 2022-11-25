using System;
using System.Collections.Generic;

namespace Cloud5mins.AzShortener
{
    public class ClickStatsEntityList
    {
        public List<ClickStatsEntity> ClickStatsList { get; set; }

        public ClickStatsEntityList(){}
        public ClickStatsEntityList (List<ClickStatsEntity> list)
        {
            ClickStatsList = list;
        }
    }
}