$(function () {
	var header = $('.menu-sticky');
	var win = $(window);


	win.on('scroll', function () {
		var scroll = win.scrollTop();
		if (scroll < 1) {
			header.removeClass("sticky");
		} else {
			header.addClass("sticky");
		}
	});
});



$(function () {
	var header = $('.header');
	var win = $(window);


	win.on('scroll', function () {
		var scroll = win.scrollTop();
		if (scroll < 300) {
			header.removeClass("fixed-top");
		} else {
			header.addClass("fixed-top");
		}
	});
});