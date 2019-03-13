var Validation = {
    ErrorsJson: null,
    Init: function (Options) {
        if (typeof (Options.ErrorsJson) == 'object') {
            Validation.ErrorsJson = Options.ErrorsJson;
        }
        else if (typeof (Options.ErrorsJson) == 'string') {
            Validation.ErrorsJson = JSON.parse(Options.ErrorsJson);
        }
        else {
            Validation.ErrorsJson = null;
        }

        return Validation;
    },
    ShowErrors: function () {
        $(Validation.ErrorsJson).each(function (Index, Item) {
            var Selector = Item.Key;
            $(Selector).closest('.form-group').addClass('has-error');
            $(Selector).closest('.form-group').find('.help-block').text(Item.Value);
        });
        $('.has-error').first().ScrollTo();
    },
    GetErrorMessageFromValidationObject: function (ValidationObject) {
        var ErrorMessage = null;
        if (ValidationObject && ValidationObject.Errors && ValidationObject.Errors.length > 0) {
            ErrorMessage = ValidationObject.Errors[0].Message;
        }

        return ErrorMessage;
    },
    HideErrors: function () {
        $('.has-error').removeClass('has-error');
    }
};


$(function () {
    $('.js-custom-file-upload input').change(function () {
        var Container = $(this).closest('.js-custom-file-upload');
        var CurrentAttachment = Container.find('.js-attachment');
        var NewAttachment = Container.find('.js-new-attachment');
        var ClearButton = Container.find('.js-clear-button');
        var DeleteButton = Container.find('.js-delete-button');


        CurrentAttachment.Hide();
        NewAttachment.text($(this).val())
        NewAttachment.Show();
        ClearButton.Show();
    });

    $('.js-custom-file-upload .js-clear-button').click(function () {
        var Container = $(this).closest('.js-custom-file-upload');
        var CurrentAttachment = Container.find('.js-attachment');
        var NewAttachment = Container.find('.js-new-attachment');
        var ClearButton = Container.find('.js-clear-button');
        var DeleteButton = Container.find('.js-delete-button');

        CurrentAttachment.Show();
        NewAttachment.text('');
        NewAttachment.Hide();
        ClearButton.Hide();

        Container.find('input').val('');
    });

    $('.js-custom-file-upload .js-image-attachment').click(function (e) {
        e.preventDefault();
        FancyBox.Init({
            src: $(this).attr('href')
        }).ShowImagePopup();
    });

    $('.js-custom-file-upload .js-delete-button').click(function () {
        var _this = $(this)
        var Container = _this.closest('.js-custom-file-upload');
        var CurrentAttachment = Container.find('.js-attachment');
        var DeleteButton = Container.find('.js-delete-button');
        var TextConfirm = _this.attr('data-text-confirm');
        var UrlDelete = _this.attr('data-url');
        var Hash = _this.attr('data-hash');

        Components63Bits.Dialog.Confirm({
            TextConfirm: TextConfirm,
            Resolve: function () {
                $.ajax({
                    type: 'POST',
                    url: UrlDelete,
                    data: { Hash: Hash },
                    dataType: 'json',
                    beforeSend: function () {
                        preloader.show();
                    },
                    success: function (res) {
                        if (res.IsSuccess) {
                            DeleteButton.Hide();
                            CurrentAttachment.remove();
                        }
                    },
                    complete: function () {
                        preloader.hide();
                    }
                });
            }
        });        
    });

    $('input[type=file]').each(function (index, i) {
        var item = $(i);
        if (item.hasClass('input-error')) {
            item.parents('.input-group').addClass('input-error');
        }
    });

    $('.checkbox-list input').change(function () {
        $(this).parent().toggleClass('active');
    });
});