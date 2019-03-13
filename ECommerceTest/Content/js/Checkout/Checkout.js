$(function () {
    $('.js-credit-cart-exp-date-textbox').mask('99/99');

    $('.js-billing-same-as-shipping-checkbox').change(function () {
        $(this).is(':checked') ? $('#BillingDiv').slideUp(200) : $('#BillingDiv').slideDown(200);
    });

    $('#PaymentOptionCard,#PaymentOptionPaypal').change(function () {
        $('#PaymentOptionCard').is(':checked') ? $('.js-credit-card-row').slideDown(200) : $('.js-credit-card-row').slideUp(200);
    });    

    $('.js-checkout-button').click(function () {
        Validation.HideErrors();

        var SubmitCheckoutModel = {};

        $('.fieldset input[type=text], .fieldset select').each(function (Index, Item) {
            Item = $(Item);
            SubmitCheckoutModel[Item.attr('name')] = Item.val();
        });

        $('.fieldset input[type=checkbox]:checked,.fieldset input[type=radio]:checked').each(function (Index, Item) {
            Item = $(Item);
            SubmitCheckoutModel[Item.attr('name')] = Item.val();
        });
        SubmitCheckoutModel['IsPaymentByCard'] = $('#PaymentOptionCard').is(':checked');
       
        $.ajax({
            type: 'POST',
            url: document.URL,
            data: SubmitCheckoutModel,
            dataType: 'json',
            beforeSend: function () {
                preloader.show();
            },
            success: function (res) {
                if (res.IsSuccess && res.Data) {
                    window.location = res.Data;
                } else if (res.Data) {
                    preloader.hide();
                    Validation.Init({
                        ErrorsJson: res.Data
                    }).ShowErrors();
                }
            },
            error: function () {
                preloader.hide();
                SuccessErrorMessageObject.ShowGlobalError();
            }
        });
    });    
});