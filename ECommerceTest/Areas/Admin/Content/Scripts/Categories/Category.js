var CategoryModel = {
    UrlFileManager: null
};
$(function () {
    TinyMCE.Init({
        Selector: '.apply-tinymce'
    }).DisplaySimplified();
    

    $('#SlugTextBox').change(function () {
        $(this).val($(this).ToSlug());
    });

    $('.js-save-button').click(function () {
        preloader.show();
    }); 
});