/*
 *	
 *
 *	DESC: Search DTO Class for data type - Person
 *
 *	NOTES: Created with MVC Scaffolding https://www.nuget.org/packages/MvcScaffolding/
 *			using a custom scaffolder called "Search" with the Package Manager Console command line:  
 *
 *				Scaffold Search Person -DbContextType PersonContext -ControllerName Person -Force
 *
 *	History
 *	==============================================================================
 *	2016/01/11	Scaf	Created.
 *
*/

using SCRUD.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SCRUD_MVC4.Models
{ 
    public class Search_PersonDTO : SearchDTO
    {
		public Search_PersonDTO()
			: base("person_srch")
		{
			sortDir = "ASC";
			sort = Person.PersonId_Display;
			recsPerPage = 10;
		}

		/// <summary>
		/// Field ordering functions used for paginated column sorting.
		/// </summary>
		public readonly IDictionary<string, Func<IQueryable<Person>, bool, IOrderedQueryable<Person>>>
		Orderings = new Dictionary<string, Func<IQueryable<Person>, bool, IOrderedQueryable<Person>>>
									{
										{Person.PersonId_Display, SearchDTO.CreateOrderingFunc<Person,System.Int32>(p=>p.PersonId)},
										{Person.FirstName_Display, SearchDTO.CreateOrderingFunc<Person,System.String>(p=>p.FirstName)},
										{Person.LastName_Display, SearchDTO.CreateOrderingFunc<Person,System.String>(p=>p.LastName)},
										{Person.DoB_Display, SearchDTO.CreateOrderingFunc<Person,System.DateTime>(p=>p.DoB)},
										{Person.GenderID_Display, SearchDTO.CreateOrderingFunc<Person,System.Int32>(p=>p.GenderID)},
									};

		public List<Person> results { get; set; }

		// Search Terms with attributes
		[Display(Name = "First Term")]
		public string Term1 { get; set; }
		[Display(Name = "Second Term")]
		public int? Term2 { get; set; }
		[Display(Name = "Third Term"), DataType(DataType.Date)]
		public DateTime? Term3 { get; set; }
		[Display(Name = "Fourth Term"), DataType(DataType.Currency)]
		public decimal? Term4 { get; set; }
	}
}

