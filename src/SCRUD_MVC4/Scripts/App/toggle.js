/// <reference path="../jquery-1.6.2-vsdoc.js" />
/// <reference path="../jquery-ui-1.8.11.js" />
/// <reference path="forms.js" />

jQuery.fn.extend({
    toggleText: function (a, b) {
        return this.text(this.text() == a ? b : a);
    }
});

jQuery.fn.extend({
    toggleID: function (id, a, b) {
        $("#" + id).toggle();
        return this.toggleText(a, b);
    }
});

function ToggleClose(id, toggleID, view) {
	var a = $("#" + toggleID);
	var kidsID = "#" + view + "_Children_" + (Boolean(id) ? id.replace(/\,/g, '_') : '');
	$(kidsID).remove(); ;
	a.attr("class", "toggle-link-show");
	return false;
}

function Toggle(id, toggleID, action, view, ToggleResultFunc, open, colspan) {
	var a = $("#" + toggleID);
	if (Boolean(open)) {
		a.attr("class", "toggle-link-hide");
		LoadToggle(id, action + (Boolean(id) ? '/' + id : ''), view, action, ToggleResultFunc, toggleID, colspan);
	}
	else {
		var c = a.attr("class");
		switch (c) {
			case "toggle-link-show":
				LoadToggle(id, action + (Boolean(id) ? '/' + id : ''), view, action, ToggleResultFunc, toggleID, colspan);
				break;
			case "toggle-link-hide":
				ToggleClose(id, toggleID, view);
				break;
		}
	}
	return false;
}

function LoadToggle(id, url, view, action, ResultFunc, toggleID, colspan) {
	$.ajaxSetup({
		cache: false,
		success: LoadDetails,
		error: Failure,
		id: id,
		view: view,
		action: action,
		resultFunc: ResultFunc,
		cspan: colspan,
		ToggleID: toggleID
	});
	$.get(url, null);
}

function LoadDetails(data, textStatus, jqXHR) {
	var viewID = this.view;
	var toggleID = this.ToggleID;
	var colspan = this.cspan;
	LoadToggleDetails(this.id, viewID, data, toggleID, this.resultFunc, colspan);
}

function LoadToggleDetails(id, viewID, html, toggleID, resultFunc, colspan)
{
	var row = $("#" + viewID);
	var cspan = row.find('td').length;
	if (Boolean(colspan)) cspan = colspan;
	var kidsID = viewID + "_Children_" + (Boolean(id) ? id.replace(/\,/g, '_') : '');
	var kidsRow = $("#" + kidsID);
	var rowHtml = "";
	if (kidsRow.length > 0) {
		rowHtml = "<td colspan='" + cspan + "'>" + html + "</td>";
		kidsRow.html(rowHtml);
	} else {
		rowHtml = "<tr id='" + kidsID + "'><td colspan='" + cspan + "'>" + html + "</td></tr>";
		row.after(rowHtml);
	}
	var a = $("#" + toggleID);
	a.attr("class", "toggle-link-hide")
	if(Boolean(resultFunc))
		resultFunc();
	return kidsID;
}