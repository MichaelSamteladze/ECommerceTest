$(function () {
    TinyMCE.Init({
        Selector: '.apply-tinymce',
        FileManagerPath: UrlFileManager

    }).DisplaySimplified();
    

    $('#SlugTextBox').change(function () {
        $(this).val($(this).ToSlug());
    });

    $('#SaveButton').click(function () {
        preloader.show();
    }); 
});