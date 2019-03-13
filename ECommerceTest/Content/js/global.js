var Globals = {
    TextError: null,
    TextSuccess: null
}
$(function () {
    $('header .hamburger').click(function (e) {
        e.preventDefault();
		$('header').toggleClass('nav-opened');
		$('header>.wrap').fadeToggle(300);
    });	
});