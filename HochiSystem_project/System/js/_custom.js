$(function () {

	/*Datepicker 日期*/
	$('.datepicker').datepicker({
		format: 'yyyy/mm/dd',
		autoclose: true,
		toggleActive: true,
		todayHighlight: true,
		orientation: 'bottom auto',
		//startDate: new Date(),
	});


	/*Select2 單選&多選*/
	$('._single').select2();
	$('._multiple').select2();
	$('.js-example-basic-single').select2();

	/*收合功能*/
	//$('.moreinfo').click(function (e) {
	//	e.preventDefault();
	//	var notthis = $('.active').not(this);
	//	notthis.find('.ti-angle-down').addClass('ti-angle-up').removeClass('ti-angle-down');
	//	notthis.toggleClass('active').next('.hideinfo').slideToggle(300);
	//	$(this).toggleClass('active').next().slideToggle("fast");
	//	$(this).children('i').toggleClass('ti-angle-up ti-angle-down');
	//});


/*圖片上傳*/
	//$('.dropify').dropify();

});
