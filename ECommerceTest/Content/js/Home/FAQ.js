$(function () {
    $('.js-toggle').click(function () {
        $(this).closest('.item').children('.content').slideToggle(200);
    });
});