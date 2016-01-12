/*
 *	© 2015 Florida Division of Emergency Management. All rights reserved. 
 * 
 *	DESC: Useful functions in one place to keep code DRY.
 *	
 *	History
 *	==============================================================================
 *	2015/08/05	G.K.	Created.
 *	
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Security.Principal;

namespace DEM.Utilities
{
	public class Utilities
	{
		/// <summary>
		/// Indicates if this host is among the names listed in AppSetting key="Test_Servers"
		/// </summary>
		public static bool isTestServer
		{
			get
			{
				var keys = ConfigurationManager.AppSettings.AllKeys.ToArray();
				if (!keys.Contains("Test_Servers")) return false;

				var host = Environment.MachineName;
				var testHosts = ConfigurationManager.AppSettings["Test_Servers"];
				return testHosts.Contains(host);
			}
		}

		public static string TestPostfix { get { return Utilities.isTestServer ? "_TEST" : ""; } }

		private class StatePair
		{
			public string Text { get; set; }
			public string Value { get; set; }

			public StatePair(string st) { Value = Text = st; }
		}

		public static SelectList States(string state)
		{
			var st = new List<StatePair>{	new StatePair("AL"), new StatePair("AK"), new StatePair("AZ"), new StatePair("AR"), new StatePair("CA"), 
											new StatePair("CO"), new StatePair("CT"), new StatePair("DE"), new StatePair("DC"), new StatePair("FL"), 
											new StatePair("GA"), new StatePair("HI"), new StatePair("ID"), new StatePair("IL"), new StatePair("IN"), 
											new StatePair("IA"), new StatePair("KS"), new StatePair("KY"), new StatePair("LA"), new StatePair("ME"), 
											new StatePair("MD"), new StatePair("MA"), new StatePair("MI"), new StatePair("MN"), new StatePair("MS"), 
											new StatePair("MO"), new StatePair("NE"), new StatePair("NV"), new StatePair("NH"), new StatePair("NJ"), 
											new StatePair("NM"), new StatePair("NY"), new StatePair("NC"), new StatePair("ND"), new StatePair("OH"), 
											new StatePair("OK"), new StatePair("OR"), new StatePair("PA"), new StatePair("RI"), new StatePair("SC"), 
											new StatePair("SD"), new StatePair("TN"), new StatePair("TX"), new StatePair("UT"), new StatePair("VT"), 
											new StatePair("VA"), new StatePair("WA"), new StatePair("WV"), new StatePair("WI"), new StatePair("WY"), 
											new StatePair("AS"), new StatePair("GU"), new StatePair("MP"), new StatePair("PR"), new StatePair("VI"), 
											new StatePair("UM"), new StatePair("FM"), new StatePair("MH"), new StatePair("PW"),
			};

			var sl = new SelectList(st, "Value", "Text", state);

			return sl;
		}

		public static List<int> SplitInts(string strInts)
		{
			var ints = new List<int>();
			var ids = strInts.Split(',');
			int theInt = -1;

			foreach (var i in ids)
			{
				if (int.TryParse(i, out theInt))
					ints.Add(theInt);
			}

			return ints;
		}

        //public static string ReportUrl(string report, object parm, string parmName)
        //{
        //    var rpt = AppValues.GetValue(report);
        //    var url = string.Format("http://{0}{1}%2f{2}&rs:Command=Render&{3}={4}",
        //                            AppValues.ReportServer, AppValues.ReportDirPath, rpt, parmName, parm.ToString());
        //    return url;
        //}

		public static bool LogException(Controller controller, Exception ex, string loc = null)
		{
			string errMsg = (string.IsNullOrWhiteSpace(loc) ? "" : "Location: " + loc + " - ") + ((ex == null) ? "" : ex.Message + ((ex.InnerException == null) ? "" : " - " + ex.InnerException.Message));
//			Log.Service.Write(errMsg);

			var msgs = ex.Message.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
			msgs.ForEach(m => { controller.ModelState.AddModelError(string.Empty, m); });

			if (ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message))
			{
				var exmsgs = ex.InnerException.Message.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
				exmsgs.ForEach(m => { controller.ModelState.AddModelError(string.Empty, m); });
			}

			return false;
		}

        //public static string ReportDirUrl()
        //{
        //    var url = string.Format("http://{0}/ReportServer/?{1}&rs:Command=ListChildren", AppValues.ReportServer, AppValues.ReportPath.Replace(@"/", @"%2f"));
        //    return url;
        //}

        //public static string ReportServerUrl()
        //{
        //    var url = string.Format("http://{0}/ReportServer", AppValues.ReportServer);
        //    return url;
        //}

		/// <summary>
		/// Determine if user is in at least one of the provided roles. Roles is a comma separated list of AD groups.
		/// </summary>
		/// <remarks>
		///	Function includes spoofing for development and testing purposes. This allows testing against AD roles that do not exist
		///	as well as placing the test user in a role they are not in. Only applicable to the UI layer.
		/// </remarks>
		public static bool UserIsInRoles(IPrincipal user, string roles)
		{
			var allowedRoles = roles.Split(',').ToList().Select(i => i.Trim()).ToList();
			allowedRoles = allowedRoles.Select(a => a + Utilities.TestPostfix).ToList();
			var isInRole = allowedRoles.Any(user.IsInRole);

			var spoofRoles = spoof_roles();
			var isInSpoofRole = spoofRoles.Intersect(allowedRoles).Any();
			return user.Identity.IsAuthenticated && (isInRole || isInSpoofRole);
		}

		/// <summary>
		/// Return the name of the user without domain information.
		/// </summary>
		public static string BaseUserName(IIdentity identity)
		{
			return identity == null || string.IsNullOrWhiteSpace(identity.Name) ? "Unknown" : identity.Name.Split('\\').Last().Trim();
		}

		private static List<string> spoof_roles()
		{
			var keys = ConfigurationManager.AppSettings.AllKeys.ToArray();

			if (!keys.Contains("enable_role_spoofing") || !keys.Contains("spoof_roles")) return new List<string>();

			var strEnableRoleSpoofing = ConfigurationManager.AppSettings["enable_role_spoofing"].ToString();
			bool enableRoleSpoofing = false;
			bool.TryParse(strEnableRoleSpoofing, out enableRoleSpoofing);
			if (!enableRoleSpoofing) return new List<string>();

			string spoofRoles = ConfigurationManager.AppSettings["spoof_roles"].ToString().Replace(" ", "");
			return spoofRoles.Split(',').ToList().Select(s => s + Utilities.TestPostfix).ToList();
		}
	}
}