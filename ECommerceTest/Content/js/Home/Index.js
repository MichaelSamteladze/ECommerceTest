$(function(){
	$('header').addClass('home-header');
	//
	$('.intro .slider').bxSlider({
		mode: 'horizontal',
		controls: false,
		adaptiveHeight: true,
		auto: true,
		pause: '8000',
		touchEnabled: jsClient.device.isDesktop ? false : true
	});
});