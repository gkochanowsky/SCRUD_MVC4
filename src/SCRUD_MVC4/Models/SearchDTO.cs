/*
 *	 
 * 
 *	DESC: DTO for pagination data.
 *	
 *	NOTES: Implements Search and pagination logic.
 *	
 *		1) This must work in a chain of open select dialogs and provide a cascade or
 *			search updates as data is changed.
 *		2) Searchs appear in a jquery-ui dialog.
 *			a) the div that contains the search dialog is named based on one of the _base_name or FormView value.
 *				- Use the FormView value if not empty, otherwise use _base_name
 *				- If the named div does on exist in the DOM it is created at the bottom of the body.
 *			
 * 
 *	History
 *	=================================================================================
 *	2015/08/05	G.K.	Created.
 * 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;

namespace SCRUD.Utilities
{
	public class SearchDTO
	{
		private const int defaultRecsPerPage = 10;
		private const int defaultPage = 1;
		private const int defaultPageRange = 20;

		public readonly string _base_name;

		public SearchDTO(string baseName)
		{
			if (string.IsNullOrWhiteSpace(baseName)) throw new Exception("Search requires baseName for element ids");
			_base_name = baseName.ToLower().Replace("_", "-");
			Page = 1;
			ComputeLastPage();
			sortDir = "ASC";
		}

		#region Forms

		/// <summary>
		/// optional ID of div containing search form.
		/// </summary>
		public string FormView { get; set; }

		/// <summary>
		/// Seed name to use in nameing results, dialog and functions.
		/// </summary>
		public string seedName { get { return ((string.IsNullOrWhiteSpace(FormView) ? "" : FormView) + "-" + _base_name).ToLower().Replace("_", "-"); } }

		/// <summary>
		/// ID of search form.
		/// </summary>
		public string FormID { get { return seedName + "-frm"; } }

		/// <summary>
		/// ID of div for search results.
		/// </summary>
		public string ResultView { get { return seedName + "-res"; } }

		/// <summary>
		/// ID of div for dialog
		/// </summary>
		public string DialogView { get { return seedName + "-dlg"; } }

		/// <summary>
		/// Base to use in function names
		/// </summary>
		public string funcBase { get { return seedName.Replace("-", "_"); } }

		/// <summary>
		/// Generated open dialog function name
		/// </summary>
		public string funcOpen { get { return "Open_" + funcBase + "_dlg"; } }

		/// <summary>
		/// Generate item selected function name
		/// </summary>
		public string funcSelect { get { return "Select_" + funcBase + "_OnClick"; } }

		/// <summary>
		/// Generated refresh results function name
		/// </summary>
		public string funcRefresh { get { return "Refresh_" + funcBase + "_res"; } }

		/// <summary>
		/// Function passed for refreshing parent (instantiating) view
		/// </summary>
		public string funcParentRefresh { set; get; }

		/// <summary>
		/// Function for passing a selection to a parent (instantiating) view
		/// </summary>
		public string funcParentSelect { get; set; }

		public bool hasSelect { get { return !string.IsNullOrWhiteSpace(funcParentSelect); } }

		#endregion

		#region pagination

		private int _pageRangeSize = defaultPageRange;
		/// <summary>
		/// Number of pages shown in selection. (rounded to next odd number)
		/// </summary>
		/// <remarks>
		/// Always rounded up to next odd number.
		/// </remarks>
		public int PageRangeSize { get { return _pageRangeSize; } set { _pageRangeSize = value + (value % 2 == 0 ? 1 : 0); } }

		private int PageRangeHalfSize { get { return PageRangeSize / 2; } }

		/// <summary>
		/// starting page in page range
		/// </summary>
		/// <remarks>
		/// Based on current page and last page and size of page range.
		/// </remarks>
		public int PageRangeStart { get { return Math.Min(_pageLast, Math.Max(PageFirst, ((_page / defaultPageRange) * defaultPageRange) + 1)); } }

		/// <summary>
		/// ending page in page range
		/// </summary>
		public int PageRangeEnd { get { return Math.Min(_pageLast, PageRangeStart + defaultPageRange - 1); } }

		/// <summary>
		/// Current page
		/// </summary>
		/// <remarks>
		/// Page will always be in range of 1 to last page.
		/// </remarks>
		public int Page { get; set; }
		private int _page { get { return Math.Max(1, Math.Min(Page, _pageLast)); } }

		public int PagePrev { get { return Math.Max(1, _page - 1); } }
		public int PageNext { get { return Math.Max(_page + 1, _pageLast); } }
		public int PageFirst { get { return 1; } }

		/// <summary>
		/// Return the page starting the next group
		/// </summary>
		public int GroupPageNext { get { return Math.Min(_page + defaultPageRange, _pageLast); } }

		/// <summary>
		/// Return the page starting the next group
		/// </summary>
		public int GroupPagePrev { get { return Math.Max(_page - defaultPageRange, PageFirst); } }

		private int _recsPerPage = defaultRecsPerPage;
		/// <summary>
		/// Setting for number of records per page. 
		/// </summary>
		/// <remarks>
		/// Default value of records per page is set by const defaultRecsPerPage.
		/// </remarks>
		public int recsPerPage { get { return _recsPerPage; } set { _recsPerPage = value; ComputeLastPage(); } }

		private int _recCount = 0;
		/// <summary>
		/// Number of records in search results
		/// </summary>
		/// <remarks>
		/// When this value is set the last page in the search results is computed based on current settings.
		/// </remarks>
		public int recCount { get { return _recCount; } set { _recCount = value; ComputeLastPage(); } }

		private int _pageLast = 1;
		/// <summary>
		/// Last page in collection.
		/// </summary>
		/// <remarks>
		/// Last page should be recomputed whenever recCount of recsPerPage changes.
		/// </remarks>
		public int PageLast { get { return _pageLast; } }

		/// <summary>
		/// Updates _pageLast
		/// </summary>
		private void ComputeLastPage()
		{
			double dval = (double)recCount / (double)recsPerPage;
			int ival = (int)dval;
			ival += dval - (double)ival == 0 ? 0 : 1;
			_pageLast = ival < 0 ? 1 : ival;
		}

		public int skip { get { return (Page - 1) * recsPerPage; } }

		/// <summary>
		/// Name of column to sort.
		/// </summary>
		public string sort { get; set; }

		/// <summary>
		/// Sort direction [ASC,DESC]
		/// </summary>
		public string sortDir { get; set; }

		/// <summary>
		/// Used by client side control code to tell action to include search form and search results in returned partial view HTML
		/// </summary>
		public bool refreshAll { get; set; }

		/// <summary>
		/// Generic function for sorting grid view column.
		/// </summary>
		public static Func<IQueryable<T>, bool, IOrderedQueryable<T>> CreateOrderingFunc<T, TKey>(Expression<Func<T, TKey>> keySelector)
		{
			return (source, ascending) => ascending ? source.OrderBy(keySelector) : source.OrderByDescending(keySelector);
		}

		#endregion
	}
}