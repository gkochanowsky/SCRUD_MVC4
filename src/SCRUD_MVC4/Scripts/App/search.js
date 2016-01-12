/// <reference path="../jquery-1.6.2-vsdoc.js" />
/// <reference path="../jquery-ui-1.8.11.js" />

function LoadSearchResults(page, formID, afterFunc, isRefreshAll) {
	var form = $("#" + formID);
	if (form.length == 0) return false;
	form.find("#Page").val(Boolean(page) ? page : '1');
	if (Boolean(isRefreshAll)) form.find("#refreshAll").val(true);
	$.ajaxSetup(
	{
		cache: false,
		error: SearchFailure 
	});
	var data = form.serialize();
	var url = form.attr("action");
	$.post(url, data, function (result) {
		var viewNameSource = "#ResultView";
		if (Boolean(isRefreshAll))
			viewNameSource = "#FormView";
		var viewName = form.find(viewNameSource).val();
		var view = $("#" + viewName);
		if (view == null) return false; 
		view.html(result);
		FixValidation(view);
		FixUi(view);
		if (Boolean(afterFunc))
			afterFunc();
	});
}

function SearchFailure(xhr, status, error) {
	if (confirm("View failure " + error + " in a new browser window?")) {
		var win = window.open();
		win.document.write(xhr.responseText);
	}
}

function SortSearchResults(col, formID) {
	var form = $("#" + formID);
	if (form == null) return false;
	var sorthv = form.find("#sort");
	var dirhv = form.find("#sortDir");
	var pagehv = form.find("#Page");
	if (sorthv == null || dirhv == null || pagehv == null) return false;
	var lastSort = sorthv.val();
	var lastDir = dirhv.val();
	if (col == lastSort) {
		dirhv.val(lastDir == "ASC" ? "DESC" : "ASC");
	}
	else {
		sorthv.val(col);
		dirhv.val("ASC");
	}
	LoadSearchResults(pagehv.val(), formID);
}

