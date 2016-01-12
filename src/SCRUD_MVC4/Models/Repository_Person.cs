/*
 *	
 *
 *	DESC: Controller for Person with Search
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
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using SCRUD.Utilities;

namespace SCRUD_MVC4.Models
{ 

    public class Repository_Person : IRepository_Person
    {
		PeopleEntities context;
		Controller _controller;

		private string User { get { return _controller != null ? Utilities.BaseUserName(_controller.User.Identity) : "Unknown"; } }

		public Repository_Person(Controller controller, PeopleEntities _context = null)
		{
			_controller = controller;
			context = _context == null ? new PeopleEntities() : _context;
		}

        public IQueryable<Person> All
        {
            get { return context.People; }
        }

		public Person New(string formView, string funcRefresh)
		{
			return new Person
			{
				formView = formView,
				funcRefresh = funcRefresh,
			};
		}

        public Person Find(int id)
        {
			return context.People.Where(x => x.PersonId == id).FirstOrDefault();
        }
 
		public Search_PersonDTO Search(Search_PersonDTO search)
		{
			try
			{
				search.Term1 = string.IsNullOrWhiteSpace(search.Term1) ? null : search.Term1;

				var result = from r in context.People
// ------ Edit this to apply terms to desired fields in the desired way
//							 where (search.Term1 == null || r.StringField.Contains(search.Term1))
//								&& (search.Term2 == null || r.IntField == search.Term2)
//								&& (search.Term3 == null || r.DateTimeField == search.Term3)
//								&& (search.Term4 == null || r.DecimalField == search.Term4)
							 select r;

				search.recCount = result.Count();

				// Apply sort order
				var applyOrdering = search.Orderings[search.sort];
				result = applyOrdering(result, search.sortDir == "ASC");

				// Take a page of data
				var recs = result.Skip(search.skip).Take(search.recsPerPage);
				search.results = recs.ToList();
			}
			catch (Exception ex)
			{
				Utilities.LogException(_controller, ex);
				search.results = new List<Person>();
			}

			search.refreshAll = false;

			return search;
		}

        public bool InsertOrUpdate(Person person)
        {
            if (person.PersonId == default(int)) {
                // New entity
                context.People.AddObject(person);

            } else {
                // Existing entity
				if(person.EntityState == System.Data.EntityState.Detached)
				{
					context.People.Attach(person);
					context.ObjectStateManager.ChangeObjectState(person, System.Data.EntityState.Modified);
				}
            }

			return this.Save() > 0;
        }

		public bool Delete(int id)
		{
			var person = context.People.Single(x => x.PersonId == id);
			context.People.DeleteObject(person);

			return this.Save() > 0;
		}

        public int Save()
        {
            try
            {
                return context.SaveChanges();
            }
            catch (Exception ex)
            {
                Utilities.LogException(_controller, ex);
                return 0;
            }
        }

        public void Dispose() 
        {
            context.Dispose();
        }
    }

    public interface IRepository_Person : IDisposable
    {
        IQueryable<Person> All { get; }
		Person New(string formView, string funcRefresh);
        Person Find(int id);
        bool InsertOrUpdate(Person person);
        bool Delete(int id);

		Search_PersonDTO Search(Search_PersonDTO search);
    }
}

