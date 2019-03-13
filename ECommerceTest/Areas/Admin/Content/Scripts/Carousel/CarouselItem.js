$(function () {
    TinyMCE.Init({
        Selector: '.apply-tinymce'
    }).DisplaySimplified();

    $('.js-save-button').click(function () {
        preloader.show();
    });
});