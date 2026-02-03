using Core.Organizations.Domain;
using Core.Reports.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Reports.Impl
{


    public static class MLFSReportGrouping
    {

        public static string[] GetGroup()
        {
            return FillGroupData();
        }

        public static List<FranchiseeGroupModel> GetGroupedFranchisee(List<Franchisee> franchiseeList)
        {
            return FillGroupedFranchiseeData(franchiseeList);
        }

        public static List<FranchiseeGroupModel> FillGroupedFranchiseeData(List<Franchisee> franchiseeDomainList)
        {
            var startDateTime = new DateTime(DateTime.Now.Year - 1, 01, 01);
            var endDateTime = new DateTime(DateTime.Now.Year, 12, 31);
            var franchiseeModel = new FranchiseeModel();
            var franchiseeModelList = new List<FranchiseeModel>();
            var franchiseeGroupModel = new FranchiseeGroupModel();
            franchiseeGroupModel.FranchiseeModel = new List<FranchiseeModel>();
            var franchiseeGroupModelList = new List<FranchiseeGroupModel>();



            franchiseeGroupModel = new FranchiseeGroupModel();
            franchiseeGroupModel.FranchiseeModel = new List<FranchiseeModel>();
            franchiseeModelList = new List<FranchiseeModel>();
            franchiseeModel = new FranchiseeModel();
            var franchiseeListForCinn = new List<string>() { "KY-Louisville", "OH-Cincinnati" };
            var cinnGroupList = franchiseeDomainList.Where(x => franchiseeListForCinn.Contains(x.Organization.Name) && x.Organization.IsActive).ToList();
            franchiseeGroupModel.GroupName = "CINN GROUP";
            foreach (var cinnGroup in cinnGroupList)
            {
                franchiseeModel = new FranchiseeModel();
                franchiseeModel.FranchiseeName = cinnGroup.Organization.Name;
                franchiseeModelList.Add(franchiseeModel);
            }

            franchiseeGroupModel.FranchiseeModel.AddRange(franchiseeModelList);
            franchiseeGroupModelList.Add(franchiseeGroupModel);


            franchiseeGroupModel = new FranchiseeGroupModel();
            franchiseeGroupModel.FranchiseeModel = new List<FranchiseeModel>();
            franchiseeModelList = new List<FranchiseeModel>();
            franchiseeModel = new FranchiseeModel();
            franchiseeGroupModel.GroupName = "MI GROUP";
            var franchiseeListForMI = new List<string>() { "MI-Detroit", "MI-Grand Rapids", "OH-Cleveland" };
            var miGroupList = franchiseeDomainList.Where(x => franchiseeListForMI.Contains(x.Organization.Name) && x.Organization.IsActive).ToList();
            foreach (var miGroup in miGroupList)
            {
                franchiseeModel = new FranchiseeModel();
                franchiseeModel.FranchiseeName = miGroup.Organization.Name;
                franchiseeModelList.Add(franchiseeModel);
            }
            franchiseeGroupModel.FranchiseeModel.AddRange(franchiseeModelList);
            franchiseeGroupModelList.Add(franchiseeGroupModel);


            franchiseeGroupModel = new FranchiseeGroupModel();
            franchiseeGroupModel.FranchiseeModel = new List<FranchiseeModel>();
            franchiseeModelList = new List<FranchiseeModel>();
            franchiseeModel = new FranchiseeModel();
            franchiseeGroupModel.GroupName = "PENSACOLA GROUP";
            var allUsedFranchiseeList = new List<string> { "MI-Grand Rapids", "AL-Mobile", "FL-Pensacola", "KY-Paducha", "MO-Kansas City", "MO-St.Louis", "TN-Nashville", "FL-Sarasota", "FL-Tampa Bay", "KY-Louisville", "OH-Cincinnati", "MI-Detroit", "OH-Cleveland" };
            var franchiseeListForPen = new List<string>() { "AL-Mobile", "FL-Pensacola" };
            var penGroupList = franchiseeDomainList.Where(x => franchiseeListForPen.Contains(x.Organization.Name) && x.Organization.IsActive).ToList();
            foreach (var penGroup in penGroupList)
            {
                franchiseeModel = new FranchiseeModel();
                franchiseeModel.FranchiseeName = penGroup.Organization.Name;
                franchiseeModelList.Add(franchiseeModel);
            }
            franchiseeGroupModel.FranchiseeModel.AddRange(franchiseeModelList);
            franchiseeGroupModelList.Add(franchiseeGroupModel);


            franchiseeGroupModel = new FranchiseeGroupModel();
            franchiseeGroupModel.FranchiseeModel = new List<FranchiseeModel>();
            franchiseeModelList = new List<FranchiseeModel>();
            franchiseeModel = new FranchiseeModel();
            franchiseeGroupModel.GroupName = "STLOUIS GROUP";

            var franchiseeListForST = new List<string>() { "KY-Paducha", "MO-Kansas City", "MO-St.Louis", "TN-Nashville" };
            var sTGroupList = franchiseeDomainList.Where(x => franchiseeListForST.Contains(x.Organization.Name) && x.Organization.IsActive).ToList();
            foreach (var stGroup in sTGroupList)
            {
                franchiseeModel = new FranchiseeModel();
                franchiseeModel.FranchiseeName = stGroup.Organization.Name;
                franchiseeModelList.Add(franchiseeModel);
            }
            franchiseeGroupModel.FranchiseeModel.AddRange(franchiseeModelList);
            franchiseeGroupModelList.Add(franchiseeGroupModel);


            franchiseeGroupModel = new FranchiseeGroupModel();
            franchiseeGroupModel.FranchiseeModel = new List<FranchiseeModel>();
            franchiseeModelList = new List<FranchiseeModel>();
            franchiseeModel = new FranchiseeModel();
            franchiseeGroupModel.GroupName = "TAMPA GROUP";
            var franchiseeListFortampa = new List<string>() { "FL-Sarasota", "FL-Tampa Bay" };
            var tampaGroupList = franchiseeDomainList.Where(x => franchiseeListFortampa.Contains(x.Organization.Name) && x.Organization.IsActive).ToList();

            foreach (var tampaGroup in tampaGroupList)
            {
                franchiseeModel = new FranchiseeModel();
                franchiseeModel.FranchiseeName = tampaGroup.Organization.Name;
                franchiseeModelList.Add(franchiseeModel);
            }
            franchiseeGroupModel.FranchiseeModel.AddRange(franchiseeModelList);
            franchiseeGroupModelList.Add(franchiseeGroupModel);

            var inActiveFranchisee= franchiseeDomainList.Where(x => allUsedFranchiseeList.Contains(x.Organization.Name)
             && !x.Organization.IsActive).Select(x=>x.Organization.Name).ToList();

            var activeFranchisee = franchiseeDomainList.Where(x => allUsedFranchiseeList.Contains(x.Organization.Name)
              && x.Organization.IsActive).Select(x => x.Organization.Name).ToList();

            franchiseeGroupModel = new FranchiseeGroupModel();
            franchiseeGroupModel.FranchiseeModel = new List<FranchiseeModel>();
            franchiseeModelList = new List<FranchiseeModel>();
            franchiseeModel = new FranchiseeModel();
            franchiseeGroupModel.GroupName = "NEW";
            var newFranchiseeList = franchiseeDomainList.Where(x => x.Organization.IsActive &&
            x.Organization.DataRecorderMetaData.DateCreated >= startDateTime &&
            x.Organization.DataRecorderMetaData.DateCreated <= endDateTime && !x.Organization.Name.StartsWith("I-")
            && !activeFranchisee.Contains(x.Organization.Name)).ToList();
            foreach (var franchisee in newFranchiseeList)
            {
                franchiseeModel = new FranchiseeModel();
                franchiseeModel.FranchiseeName = franchisee.Organization.Name.ToUpper();
                franchiseeModelList.Add(franchiseeModel);
            }

            franchiseeModel = new FranchiseeModel();
            franchiseeModel.FranchiseeName = "MA-Springfield";
            franchiseeModelList.Add(franchiseeModel);
            franchiseeGroupModel.FranchiseeModel.AddRange(franchiseeModelList);
            franchiseeGroupModelList.Add(franchiseeGroupModel);


            franchiseeGroupModel = new FranchiseeGroupModel();
            franchiseeGroupModel.FranchiseeModel = new List<FranchiseeModel>();
            franchiseeModelList = new List<FranchiseeModel>();
            franchiseeModel = new FranchiseeModel();
            var establishedFranchiseeList = franchiseeDomainList.Where(x => x.Organization.IsActive &&
            x.Organization.DataRecorderMetaData.DateCreated <= startDateTime &&
            x.Organization.DataRecorderMetaData.DateCreated <= endDateTime && !x.Organization.Name.StartsWith("I-")
            && !activeFranchisee.Contains(x.Organization.Name)).ToList();

            franchiseeGroupModel.GroupName = "ESTABLISHED";
            foreach (var franchisee in establishedFranchiseeList)
            {
                franchiseeModel = new FranchiseeModel();
                franchiseeModel.FranchiseeName = franchisee.Organization.Name.ToUpper();
                franchiseeModelList.Add(franchiseeModel);
            }

            franchiseeGroupModel.FranchiseeModel.AddRange(franchiseeModelList);
            franchiseeGroupModelList.Add(franchiseeGroupModel);



            var internationalCanadianFranchiseeList = franchiseeDomainList.Where(x => x.Organization.IsActive &&
            (x.Organization.Name.ToUpper().StartsWith("I-CANADA"))).ToList();
            franchiseeGroupModel = new FranchiseeGroupModel();
            franchiseeGroupModel.FranchiseeModel = new List<FranchiseeModel>();
            franchiseeModelList = new List<FranchiseeModel>();
            franchiseeModel = new FranchiseeModel();
            franchiseeGroupModel.GroupName = "INTERNATIONAL CANADIAN";
            foreach (var franchisee in internationalCanadianFranchiseeList)
            {
                franchiseeModel = new FranchiseeModel();
                franchiseeModel.FranchiseeName = franchisee.Organization.Name.ToUpper();
                franchiseeModelList.Add(franchiseeModel);
            }
            franchiseeGroupModel.FranchiseeModel.AddRange(franchiseeModelList);
            franchiseeGroupModelList.Add(franchiseeGroupModel);


            var internationalFranchiseeList = franchiseeDomainList.Where(x => x.Organization.IsActive &&
            !x.Organization.Name.ToUpper().StartsWith("I-CANADA") && x.Organization.Name.StartsWith("I-")).ToList();
            franchiseeGroupModel = new FranchiseeGroupModel();
            franchiseeModelList = new List<FranchiseeModel>();
            franchiseeGroupModel.FranchiseeModel = new List<FranchiseeModel>();
            franchiseeModel = new FranchiseeModel();
            franchiseeGroupModel.GroupName = "INTERNATIONAL";
            foreach (var franchisee in internationalFranchiseeList)
            {
                franchiseeModel = new FranchiseeModel();
                franchiseeModel.FranchiseeName = franchisee.Organization.Name.ToUpper();
                franchiseeModelList.Add(franchiseeModel);
            }
            franchiseeModel = new FranchiseeModel();
            franchiseeModel.FranchiseeName = "i-Bahamas($800.00 min)"; franchiseeModelList.Add(franchiseeModel);
            franchiseeModel = new FranchiseeModel();
            franchiseeModel.FranchiseeName = "I-UAE-Dubai"; franchiseeModelList.Add(franchiseeModel);
            franchiseeGroupModel.FranchiseeModel.AddRange(franchiseeModelList);
            franchiseeGroupModelList.Add(franchiseeGroupModel);

            var inActiveFranchiseeList = franchiseeDomainList.Where(x => !x.Organization.IsActive || inActiveFranchisee.Contains(x.Organization.Name)).ToList();
            franchiseeGroupModel = new FranchiseeGroupModel();
            franchiseeModelList = new List<FranchiseeModel>();
            franchiseeGroupModel.FranchiseeModel = new List<FranchiseeModel>();
            franchiseeModel = new FranchiseeModel();
            franchiseeGroupModel.GroupName = "CLOSED";
            foreach (var franchisee in inActiveFranchiseeList)
            {
                franchiseeModel = new FranchiseeModel();
                franchiseeModel.FranchiseeName = franchisee.Organization.Name.ToUpper();
                franchiseeModelList.Add(franchiseeModel);
            }
            //franchiseeModel = new FranchiseeModel();
            //franchiseeModel.FranchiseeName = "CA-LOS ANGELES-N"; 
            //franchiseeModelList.Add(franchiseeModel);
            franchiseeModel = new FranchiseeModel();
            franchiseeModel.FranchiseeName = "CA-SAN DIEGO";
            franchiseeModelList.Add(franchiseeModel);
            franchiseeModel = new FranchiseeModel();
            franchiseeModel.FranchiseeName = "GA-Columbus (CLOSED)";
            franchiseeModelList.Add(franchiseeModel);
            franchiseeModel = new FranchiseeModel();
            franchiseeModel.FranchiseeName = "HI-Hawaii($800.00 min)";
            franchiseeModelList.Add(franchiseeModel);
            franchiseeModel = new FranchiseeModel();
            franchiseeModel.FranchiseeName = "PA-Northeastern Pennsylvania($800.00 min)";
            franchiseeModelList.Add(franchiseeModel);
            franchiseeGroupModel.FranchiseeModel.AddRange(franchiseeModelList);
            franchiseeGroupModelList.Add(franchiseeGroupModel);

            return franchiseeGroupModelList;
        }
        private static string[] FillGroupData()
        {
            string[] str_add = {  "CINN GROUP", "MI GROUP", "PENSACOLA GROUP", "STLOUIS GROUP"
            , "TAMPA GROUP", "NEW", "ESTABLISHED","INTERNATIONAL CANADIAN","INTERNATIONAL","CLOSED"};
            return str_add;
        }

    }

}
