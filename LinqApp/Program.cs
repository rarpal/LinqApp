using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            new Practice {CodePractice="1014", CCGCode="5QG"}};

            List<CCGRate> CCGRates = new List<CCGRate>() {
            new CCGRate {OrgCode="08T", Type="Visit", Rate=5},
            new CCGRate {OrgCode="08T", Type="Admin", Rate=6},
            new CCGRate {OrgCode="06J", Type="Visit", Rate=8},
            new CCGRate {OrgCode="06J", Type="Admin", Rate=3}};

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
            var ccgrates = Practices
                .Select(ccg => new { ccg.CCGCode }).Distinct()
                .GroupJoin(CCGRates, ccg => ccg.CCGCode, rate => rate.OrgCode, (c, r) => new { ccg = c, rates = r })
                .Select(pv => new
                    {
                        CCGCode = pv.ccg.CCGCode,
                        CCGGPVisitRate = (pv.rates.Count() == 0 ? 0.00 : pv.rates.Where(rt => rt.Type == "Visit").First().Rate),
                        CCGGPAdminRate = (pv.rates.Count() == 0 ? 0.00 : pv.rates.Where(rt => rt.Type == "Admin").First().Rate)
                    });

            foreach (var item in ccgrates)
            {
                Console.WriteLine(item.CCGCode + "," + item.CCGGPVisitRate.ToString() + "," + item.CCGGPAdminRate.ToString());
            }
            Console.ReadKey();
        }
    }
}
