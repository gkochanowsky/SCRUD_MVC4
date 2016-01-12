/*
 *	
 *
 *	Desc: Model Controller referencing model repository with partial view popups with Search.
 *
 *	NOTES: Created with MVC Scaffolding https://www.nuget.org/packages/MvcScaffolding/
 *			using a custom scaffolder called "Search" with the Package Manager Console command line:  
 *
 *				Scaffold Search Person -DbContextType PersonContext -ControllerName Person -Force
 *
 *	History
 *	==============================================================================
 *	2016/01/11	Scaf	Created.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SCRUD_MVC4.Models;

namespace SCRUD_MVC4.Controllers
{   
	public class PersonController : Controller
	{
		public readonly IRepository_Person Repository_person;

		public PersonController()
		{
			this.Repository_person = new Repository_Person(this);
		}

		public ViewResult Index()
		{
			return View();
		}

//		[OutputCache(Location = System.Web.UI.OutputCacheLocation.None)]
		public ActionResult Search(int id = 0, string formView = null, bool showAll = false, string funcRefresh = null, string funcSelect = null)
		{
			var dto = new Search_PersonDTO { 
				FormView = formView, 
				funcParentRefresh = funcRefresh,
				funcParentSelect = funcSelect
			};

			if(showAll) dto = Repository_person.Search(dto);

			return PartialView(dto);
		}

		[HttpPost]
		public ActionResult Search(Search_PersonDTO search)
		{
			ModelState.Clear();
			var refreshView = search.refreshAll;
			return PartialView(refreshView ? "Search" : "SearchResults", Repository_person.Search(search));
		}

		public ActionResult List(int? id = null, string formView = null, string funcRefresh = null)
		{
			ViewBag.ParentID = id;
			ViewBag.formView = formView;
			ViewBag.funcRefresh = funcRefresh;

			return PartialView(Repository_person.All);
		}

		public ActionResult Details(int id)
		{
			return PartialView(Repository_person.Find(id));
		}

		public ActionResult CreateOrEdit(int? id = null, string formView = null, string funcRefresh = null)
		{
			Person dto;
			if(id == null)
				dto = Repository_person.New(formView, funcRefresh);
			else
			{
				dto = Repository_person.Find(id.GetValueOrDefault());
				dto.formView = formView;
				dto.funcRefresh = funcRefresh;
			}

			return PartialView(dto);
		} 

		[HttpPost]
		public ActionResult CreateOrEdit(Person person)
		{
			if (ModelState.IsValid &&  Repository_person.InsertOrUpdate(person)) 
				return Content("SUCCESS");

			return PartialView(person);	
		}
         
		public ActionResult Delete(int id, string formView = null, string funcRefresh = null)
		{
			var dto = Repository_person.Find(id);
			dto.formView = formView;
			dto.funcRefresh = funcRefresh;
			return PartialView(dto);
		}

		[HttpPost, ActionName("Delete")]
		public ActionResult DeleteConfirmed(int id)
		{
			if(Repository_person.Delete(id))
				return Content("SUCCESS");

			return PartialView("Delete", Repository_person.Find(id));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				Repository_person.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}

