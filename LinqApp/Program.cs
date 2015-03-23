// From office
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqApp
{
    public class Practice
    {
        public string CodePractice { get; set; }
        public string CCGCode { get; set; }
    }

    public class CCGRate
    {
        public string OrgCode { get; set; }
        public string Type { get; set; }
        public float Rate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            List<Practice> Practices = new List<Practice>() {
            new Practice {CodePractice="1010", CCGCode="08T"}, 
            new Practice {CodePractice="1011", CCGCode="08T"},
            new Practice {CodePractice="1012", CCGCode="06J"},
            new Practice {CodePractice="1013", CCGCode="06J"},
            new Practice {CodePractice="1014", CCGCode="5QG"}
            };

            List<CCGRate> CCGRates = new List<CCGRate>() {
            new CCGRate {OrgCode="08T", Type="Visit", Rate=45, FromDate=new DateTime(2012,04,01), ToDate=new DateTime(2012,06,30)},
            new CCGRate {OrgCode="08T", Type="Admin", Rate=5, FromDate=new DateTime(2012,04,01), ToDate=new DateTime(2012,06,30)},
            new CCGRate {OrgCode="08T", Type="Visit", Rate=46, FromDate=new DateTime(2012,07,01), ToDate=new DateTime(2012,09,30)},
            new CCGRate {OrgCode="08T", Type="Admin", Rate=6, FromDate=new DateTime(2012,07,01), ToDate=new DateTime(2012,09,30)},
            new CCGRate {OrgCode="06J", Type="Visit", Rate=42, FromDate=new DateTime(2012,04,01), ToDate=new DateTime(2012,06,30)},
            new CCGRate {OrgCode="06J", Type="Admin", Rate=2, FromDate=new DateTime(2012,04,01), ToDate=new DateTime(2012,06,30)},
            new CCGRate {OrgCode="06J", Type="Visit", Rate=43, FromDate=new DateTime(2012,07,01), ToDate=new DateTime(2012,10,30)},
            new CCGRate {OrgCode="06J", Type="Admin", Rate=3, FromDate=new DateTime(2012,07,01), ToDate=new DateTime(2012,10,30)}
            };

            /* 
             * query syntax
             * change the initial group by to a distinct to match the method syntax
             * it may not be possible to implement a pre-distinct before group by with query syntax
             */
            //var query = from ccg in Practices
            //            group ccg by ccg.CCGCode into grpccg
            //            join rate in CCGRates on grpccg.First().CCGCode equals rate.OrgCode into rates
            //            select new
            //            {
            //                CCGCode = grpccg.First().CCGCode,
            //                CCGGPVisitRate = (rates.Count() == 0 ? 0.00 : rates.Where(type => type.Type == "Visit").First().Rate),
            //                CCGGPAdminRate = (rates.Count() == 0 ? 0.00 : rates.Where(type => type.Type == "Admin").First().Rate)
            //            };

            /*
             * method syntax
             */
            //var ccgrates = Practices
            //    .Select(ccg => new { ccg.CCGCode }).Distinct()
            //    .GroupJoin(CCGRates, ccg => ccg.CCGCode, rate => rate.OrgCode, (c, r) => new { ccg = c, rates = r })
            //    .Select(pv => new
            //    {
            //        CCGCode = pv.ccg.CCGCode,
            //        //CCGGPVisitRate = (pv.rates.Count() == 0 ? 0.00 : pv.rates.Where(rt => rt.Type == "Visit").First().Rate),
            //        //CCGGPAdminRate = (pv.rates.Count() == 0 ? 0.00 : pv.rates.Where(rt => rt.Type == "Admin").First().Rate)
            //        CCGGPVisitRate = pv.rates.Where(rt => rt.Type == "Visit").Count() > 0 ? pv.rates.Where(rt => rt.Type == "Visit").First().Rate : 0.00,
            //        CCGGPAdminRate = pv.rates.Where(rt => rt.Type == "Admin").Count() > 0 ? pv.rates.Where(rt => rt.Type == "Admin").First().Rate : 0.00
            //    });

            var ccgrates1 = Practices
                .Select(ccg => new { ccg.CCGCode }).Distinct()
                .GroupJoin(CCGRates, ccg => ccg.CCGCode, rate => rate.OrgCode, (c, r) => new
                    {
                        ccg = c,
                        rates = r.GroupBy(grp => new {grp.OrgCode, grp.FromDate })
                            .Select(pv => new
                            {
                                FromDate = pv.Where(rt => rt.Type == "Visit").First().FromDate,
                                ToDate = pv.Where(rt => rt.Type == "Visit").First().ToDate,
                                VisitRate = pv.Where(rt => rt.Type == "Visit").First().Rate,
                                AdminRate = pv.Where(rt => rt.Type == "Admin").First().Rate
                            })
                    });

            var ccgrates2 = CCGRates
                .Where(ccgr => ccgr.OrgCode == "08T")
                .GroupBy(grp => grp.FromDate)
                .Select(pv => new
                {
                    OrgCode = pv.Where(rt => rt.Type == "Admin").First().OrgCode,
                    FromDate = pv.Where(rt => rt.Type == "Visit").First().FromDate,
                    ToDate = pv.Where(rt => rt.Type == "Visit").First().ToDate,
                    VisitRate = pv.Where(rt => rt.Type == "Visit").First().Rate,
                    AdminRate = pv.Where(rt => rt.Type == "Admin").First().Rate
                });


                //.Select(pv => new
                //{
                //    CCGCode = pv.ccg.CCGCode,
                //    CCGGPVisitRate = (pv.rates.Count() == 0 ? 0.00 : pv.rates.Where(rt => rt.Type == "Visit").First().Rate),
                //    CCGGPAdminRate = (pv.rates.Count() == 0 ? 0.00 : pv.rates.Where(rt => rt.Type == "Admin").First().Rate)
                //});

            foreach (var item in ccgrates1)
            {
                Console.WriteLine(item.ccg.CCGCode);
                foreach (var itemr in item.rates)
                {
                    Console.WriteLine(itemr.FromDate.ToString() + "," + itemr.ToDate.ToString() + "," + itemr.AdminRate.ToString() + "," + itemr.VisitRate.ToString());
                }
                
                //Console.WriteLine("test me");
            }
            Console.ReadKey();

            foreach (var item in ccgrates2)
            {
                Console.WriteLine(item.OrgCode.ToString() + "," + item.FromDate.ToString() + "," + item.ToDate.ToString() + "," + item.AdminRate.ToString() + "," + item.VisitRate.ToString());
                //Console.WriteLine("test me");
            }
            Console.ReadKey();

        }
    }
}
