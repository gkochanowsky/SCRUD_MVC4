/*
 *	
 *
 *	DESC: Adds Metadata class for annotation
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace SCRUD_MVC4.Models
{ 
	[MetadataType(typeof(Person_Metadata))]
    public partial class Person
    {
		// Partial class linking MetadataType

		public const string PersonId_Display = "PersonId"; 
		public const string FirstName_Display = "FirstName"; 
		public const string LastName_Display = "LastName"; 
		public const string DoB_Display = "DoB"; 
		public const string GenderID_Display = "GenderID"; 

		// properties to support search scaffolding
		public string formView { get; set; }
		public string funcRefresh { get; set; }
	}

    public partial class Person_Metadata    {
		// Decorate metadata properties with attributes.
		[Display(Name = Person.PersonId_Display)]
		public object PersonId  { get; set; }
		[Display(Name = Person.FirstName_Display)]
		public object FirstName  { get; set; }
		[Display(Name = Person.LastName_Display)]
		public object LastName  { get; set; }
		[Display(Name = Person.DoB_Display)]
		public object DoB  { get; set; }
		[Display(Name = Person.GenderID_Display)]
		public object GenderID  { get; set; }
	}
}

