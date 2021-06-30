using System;
using System.Collections.Generic;
using TestTasks.InternationalTradeTask.Models;

namespace TestTasks.InternationalTradeTask
{
    public class CommodityRepository
    {
        public double GetImportTariff(string commodityName)
        {
            double result = 0;

            string commodityCode = GetCodeByName(commodityName);

            var commodityGroupQueue = new Queue<ICommodityGroup>(_allCommodityGroups);
            while(commodityGroupQueue.Count > 0)
            {
                ICommodityGroup group = commodityGroupQueue.Dequeue();

                if(commodityCode.Contains(group.SITCCode))
                {
                    if (group.ImportTarif != null) result = (double)group.ImportTarif;

                    if (commodityCode == group.SITCCode) break;

                    foreach (ICommodityGroup gr in group.SubGroups) commodityGroupQueue.Enqueue(gr);
                }
            }

            return result;
        }

        public double GetExportTariff(string commodityName)
        {
            double result = 0;

            string commodityCode = GetCodeByName(commodityName);

            var commodityGroupQueue = new Queue<ICommodityGroup>(_allCommodityGroups);
            while (commodityGroupQueue.Count > 0)
            {
                ICommodityGroup group = commodityGroupQueue.Dequeue();

                if (commodityCode.Contains(group.SITCCode))
                {
                    if (group.ExportTarif != null) result = (double)group.ExportTarif;

                    if (commodityCode == group.SITCCode) break;

                    foreach (ICommodityGroup gr in group.SubGroups) commodityGroupQueue.Enqueue(gr);
                }
            }

            return result;
        }

        private string GetCodeByName(string commodityName)
        {
            var commodityGroupQueue = new Queue<ICommodityGroup>(_allCommodityGroups);

            while(commodityGroupQueue.Count > 0)
            {
                ICommodityGroup gr = commodityGroupQueue.Dequeue();

                if (gr.Name == commodityName)
                {
                    return gr.SITCCode;
                }
                else
                {
                    if(gr.SubGroups != null)
                    {
                        foreach (ICommodityGroup group in gr.SubGroups)
                        {
                            commodityGroupQueue.Enqueue(group);
                        }
                    }                    
                }
            }

            throw new ArgumentException();
        }

        private FullySpecifiedCommodityGroup[] _allCommodityGroups = new FullySpecifiedCommodityGroup[]
        {
            new FullySpecifiedCommodityGroup("06", "Sugar, sugar preparations and honey", 0.05, 0)
            {
                SubGroups = new CommodityGroup[]
                {
                    new CommodityGroup("061", "Sugar and honey")
                    {
                        SubGroups = new CommodityGroup[]
                        {
                            new CommodityGroup("0611", "Raw sugar,beet & cane"),
                            new CommodityGroup("0612", "Refined sugar & other prod.of refining,no syrup"),
                            new CommodityGroup("0615", "Molasses", 0, 0),
                            new CommodityGroup("0616", "Natural honey", 0, 0),
                            new CommodityGroup("0619", "Sugars & syrups nes incl.art.honey & caramel"),
                        }
                    },
                    new CommodityGroup("062", "Sugar confy, sugar preps. Ex chocolate confy", 0, 0)
                }
            },
            new FullySpecifiedCommodityGroup("282", "Iron and steel scrap", 0, 0.1)
            {
                SubGroups = new CommodityGroup[]
                {
                    new CommodityGroup("28201", "Iron/steel scrap not sorted or graded"),
                    new CommodityGroup("28202", "Iron/steel scrap sorted or graded/cast iron"),
                    new CommodityGroup("28203", "Iron/steel scrap sort.or graded/tinned iron"),
                    new CommodityGroup("28204", "Rest of 282.0")
                }
            }
        };
    }
}
