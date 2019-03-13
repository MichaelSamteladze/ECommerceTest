$(function () {
    TinyMCE.Init({
        Selector: '.apply-tinymce',
        FileManagerPath: UrlFileManager

    }).Display();


    $('#SlugTextBox').change(function () {
        $(this).val($(this).ToSlug());
    });

    $('#SaveButton').click(function () {
        preloader.show();
    });
});