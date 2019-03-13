var TinyMCE = {
    Selector: 'textarea',
    Width: '100%',
    Height: 250,
    AutoSize: false,
    FileManagerPath: null,
    CustomToolbars: '',
    Setup: null,


    Init: function (Options) {
        if (Options) {
            TinyMCE.Selector = Options.Selector ? Options.Selector : TinyMCE.Selector;
            TinyMCE.Width = Options.Width ? Options.Width : TinyMCE.Width;
            TinyMCE.Height = Options.Height ? Options.Height : TinyMCE.Height;
            TinyMCE.AutoSize = Options.AutoSize ? Options.AutoSize : TinyMCE.AutoSize;
            TinyMCE.FileManagerPath = Options.FileManagerPath ? Options.FileManagerPath : TinyMCE.FileManagerPath;
            TinyMCE.Setup = Options.Setup ? Options.Setup : TinyMCE.Setup;
            TinyMCE.CustomToolbars = Options.CustomToolbars ? Options.CustomToolbars : TinyMCE.CustomToolbars;
        }

        return TinyMCE;
    },

    Display: function () {
        $(TinyMCE.Selector).tinymce({
            width: TinyMCE.Width,
            height: TinyMCE.Height,
            menubar: false,
            toolbar_items_size: 'small',
            toolbar: 'bold italic underline strikethrough style-h2 style-h3 | alignleft aligncenter alignjustify alignright superscript subscript | bullist numlist | link image | forecolor backcolor | filemanager code' + TinyMCE.CustomToolbars,
            plugins: 'link,image,textcolor,media,code,paste,FormatingToolbarButtons' + (TinyMCE.FileManagerPath == null ? '' : ',filemanager') + (TinyMCE.AutoSize ? ',autoresize' : ''),

            filemanager_path: TinyMCE.FileManagerPath,

            paste_word_valid_elements: 'b,strong,i,em,ul,li,ol,p,br,sub,sup,h2,h3',
            forced_root_block: false,
            force_p_newlines: false,
            remove_linebreaks: false,
            force_br_newlines: true,
            remove_trailing_nbsp: false,
            verify_html: false,
            apply_source_formatting: true,
            relative_urls: false,
            remove_script_host: false,
            convert_urls: false,
            autoresize_bottom_margin: 10,
            autoresize_min_height: TinyMCE.Height,

            setup: TinyMCE.Setup
        });
    },

    DisplaySimplified: function () {

        $(TinyMCE.Selector).tinymce({
            width: TinyMCE.Width,
            height: TinyMCE.Height,
            menubar: false,
            toolbar_items_size: 'small',
            toolbar: 'bold italic underline strikethrough | alignleft aligncenter alignjustify alignright sub sup | link | code ' + TinyMCE.CustomToolbars,
            plugins: 'link,textcolor,code,paste',
            paste_word_valid_elements: 'b,strong,i,em,ul,li,ol,p,br,sub,sup',
            forced_root_block: false,
            force_p_newlines: false,
            remove_linebreaks: false,
            force_br_newlines: true,
            remove_trailing_nbsp: false,
            verify_html: false,
            apply_source_formatting: true,

            setup: TinyMCE.Setup
        });
    }

}