/// <reference path="../jquery-1.6.2-vsdoc.js" />
/// <reference path="../jquery-ui-1.8.11.js" />

function OpenForm(id, view, action, afterFunc, addView) {
	if ($("#" + view).length == 0) GenerateView(view);
	Load(id, action + (Boolean(id) ? '/' + id : '') + (Boolean(addView) ? '?formView=' + view : ''), LoadViewResult, view, null, afterFunc);
}

function PostForm(viewID, formID, finishFunc, failFunc, PrePostFunc, otherUrl, skipVal) {
	var view = $("#" + viewID);
	var form = $("#" + formID);
	if (form.length == 0 || view.length == 0) return;
	if (!Boolean(skipVal)) if(!form.valid()) return;
	$.ajaxSetup({
		cache: false
	});
	var data = form.serialize();
	var url = form.attr("action");
	if (Boolean(otherUrl))
		url = otherUrl
	if (Boolean(PrePostFunc)) {
		PrePostFunc();
	}
	$.post(url, data, function (result) {
		var isValid = true;
		if (result == "SUCCESS" || result == "") {
		    view.dialog("close");
			view.dialog("destroy");
			view.html("");
		}
		else {
			view.html(result);
			var isValidCtl = view.find("[id$='IsValid']");
			if (isValidCtl.length > 0)
				isValid = isValidCtl.val().toLowerCase() == "true";
			FixUi(view);
			FixValidation(view);
			if (Boolean(skipVal) && isValid)
				ClearValidation(viewID);
		}
		if (Boolean(finishFunc) && Boolean(isValid)) {
			finishFunc();
		}

		if (Boolean(failFunc) && !Boolean(isValid)) {
			failFunc(result);
		}
		return false;
	});
}

function GenerateView(view) {
	document.body.insertAdjacentHTML('beforeend', '<div id="' + view + '" />');
	$("#" + view).dialog({
		autoOpen: false,
		modal: true,
		width: 'auto',
		height: 'auto',
		position: { my: "left top", at: "left top", of: $("#body") },
		close: function (event, ui) {
			$(this).dialog("destroy").remove();
		}
	});
}

function LoadViewResult(data, textStatus, jqXHR) {
	var view = LoadView(this.view, data);
	if (Boolean(this.funcAfter))
		return this.funcAfter();
	return false;
}

function LoadView(viewName, data) {
	if (data == "") return;
	var view = $('#' + viewName);
	view.html(data);
	FixValidation(view);
	FixUi(view);
	view.dialog("open");
	return view;
}

function FixUi(view) {
    var prts = window.location.pathname.split("/");
    var hst = '';
    $.each(prts, function (idx) { if(this.length  > 0 && hst.length == 0) hst = this; });
	var imageUrl = '/' + hst + '/Images/calendar.png';
	view.find(".datefield").datepicker({
		showOn: "both",
		buttonImage: imageUrl,
		buttonImageOnly: true
	});
}

function FixValidation(view) {
	var form = view.find('form');
	$.each(form, function(i, frm) {
		$(this).removeData('validator');
		$(this).removeData('unobtrusiveValidation');
		var formID = $(this).attr('id');
		$.validator.unobtrusive.parse('#' + formID);
	});
}

function ClearValidation(viewID) {
	$('#' + viewID + ' .input-validation-error').addClass('input-validation-valid');
	$('#' + viewID + ' .input-validation-error').removeClass('input-validation-error');
	//Removes validation message after input-fields
	$('#' + viewID + ' .field-validation-error').addClass('field-validation-valid');
	$('#' + viewID + ' .field-validation-error').removeClass('field-validation-error');
	//Removes validation summary 
	$('#' + viewID + ' .validation-summary-errors').addClass('validation-summary-valid');
	$('#' + viewID + ' .validation-summary-errors').removeClass('validation-summary-errors');
}

function Load(id, url, OnSuccess, view, action, afterFun) {
	$.ajaxSetup({
		cache: false,
		success: OnSuccess,
		error: Failure,
		id: id,
		view: view,
		action: action,
		funcAfter: afterFun
	});
	$.get(url, null);
}

function Failure(xhr, status, error) {
	if (confirm("View failure " + error + " in a new browser window?")) {
		var win = window.open();
		win.document.write(xhr.responseText);
	}
}

function ConfirmForm(id, view, action, afterFunc) {
	return Load(id, action + (Boolean(id) ? '/' + id : ''), LoadConfirmViewResult, view, null, afterFunc);
}

function LoadConfirmViewResult(data, textStatus, jqXHR) {
	isValid = data.length == 0;
	var view = $(this.view);
	if (view.length == 0) return false;
	if (isValid)
		view.dialog('close');
	else
		view.html(data);
	if (Boolean(this.funcAfter)) 
		return this.funcAfter(isValid); 
	return false;
}

function UpdateDDL(ddl, id, text) {
	if (!Boolean(ddl) || ddl.length == 0 || !Boolean(id) || !Boolean(text)) return;
	var opt = ddl.find('option[value=' + id + ']');
	if (opt.length == 0) {
		ddl.append('<option value="' + id + '">' + text + '</option>');
	}
	else {
		opt.attr("text", text);
	}
	ddl.val(id);
}
