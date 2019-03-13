var FaqModel = {
    FaqID: null,
    TextConfirmDelete: null,
    UrlAdd: null,
    UrlUpdate: null,
    UrlDelete: null,
    UrlSync: null,

    Create: function (Question, Answer, IsPublished) {
        $.ajax({
            type: 'POST',
            url: FaqModel.UrlAdd,
            data: { Question: Question, Answer: Answer, IsPublished: IsPublished },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (res) {
                if (res.IsSuccess) {
                    window.location = document.URL;
                }
                else {
                    alert(Globals.TextError);
                }
            },
            error: function () {
                alert(Globals.TextError);
            },
            complete: function () {
                preloader.hide();
            }
        });
    },
    Delete: function (FaqID) {
        Components63Bits.Dialog.Confirm({
            TextConfirm: FaqModel.TextConfirmDelete,
            ConfirmButtonColor: Components63Bits.Dialog.ButtonColors.Red,
            Resolve: function () {
                $.ajax({
                    type: 'POST',
                    url: FaqModel.UrlDelete,
                    data: { FaqID: FaqID },
                    dataType: 'json',
                    beforeSend: function () {
                        preloader.show();
                    },
                    success: function (res) {
                        if (res.IsSuccess) {
                            $('#faq' + FaqID).slideUp(200, function () {
                                $(this).remove();
                            });
                        }
                        else {
                            alert(Abort);
                        }
                    },
                    error: function (response) {
                        alert(Abort);
                    },
                    complete: function () {
                        preloader.hide();
                    }
                });
            }
        });
    },
    Sync: function () {
        var SortIndexes = new Array();
        
        $('.faq-items [data-id]').each(function (index, Item) {
            Item = $(Item);
            var FaqID = Item.attr('data-id');
            var SortIndex = index;

            SortIndexes.push({ ID: FaqID, SortIndex: SortIndex });
        });        

        $.ajax({
            type: 'POST',
            url: FaqModel.UrlSync,
            data: { SortIndexes: SortIndexes },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (res) {
                if (res.IsSuccess) {
                }
                else {
                    alert(Globals.TextError);
                }
            },
            error: function () {
                alert(Globals.TextError);
            },
            complete: function () {
                preloader.hide();
            }
        });         
    },
    Update(FaqID, Question, Answer, IsPublished) {
        $.ajax({
            type: 'POST',
            url: FaqModel.UrlUpdate,
            data: { FaqID: FaqID, Question: Question, Answer: Answer, IsPublished: IsPublished },
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (res) {
                if (res.IsSuccess) {
                    window.location = document.URL;
                }
                else {
                    alert(Globals.TextError);
                }
            },
            error: function () {
                alert(Globals.TextError);
            },
            complete: function () {
                preloader.hide();
            }
        });
    }
};

$(function () {
    TinyMCE.Init({ Selector: '.apply-tinymce', Width: '100%', Height: '200px' }).DisplaySimplified();

    $(document).on('focusin', function (e) {
        if ($(e.target).closest('.mce-window').length) {
            e.stopImmediatePropagation();
        }
    });

    $('.js-add-new-button').click(function () {
        FaqModel.FaqID = null;
        $('.js-modal-faq-item').modal('show');
    });

    $('.js-save-button').click(function () {
        var Question = $('#QuestionTextBox').val();
        var Answer = $('#AnswerTextBox').html();
        var IsPublished = $('#IsPublishedChechBox').is(':checked');

        if (FaqModel.FaqID) {
            FaqModel.Update(FaqModel.FaqID, Question, Answer,IsPublished);
        }
        else {
            FaqModel.Create(Question, Answer, IsPublished);
        }
    });

    $('.faq-edit').click(function (e) {
        e.preventDefault();
        var Container = $(this).closest('.well');
        var Question = Container.find('.media-heading.ge').text();
        var Answer = Container.find('.faq-answer.ge').html();
        var IsPublished = Container.find('.faq-published-status i').hasClass('fa-check');

        FaqModel.FaqID = Container.attr('id').replace('faq', '');
        $('#QuestionTextBox').val(Question);
        $('#AnswerTextBox').html(Answer);
        $('#IsPublishedChechBox').prop('checked', IsPublished);
        $('.js-modal-faq-item').modal('show');
    });

    $('.faq-delete').click(function (e) {
        e.preventDefault();
        var FaqID = $(this).closest('.well').attr('id').replace('faq', '');
        FaqModel.Delete(FaqID);
    });

    $('.faq-items').sortable({
        handle: '.js-drag-me',
        helper: function () { return $(Globals.Templates.DragHelper) },
        update: function (event, ui) {
            FaqModel.Sync();
        }
    });
});