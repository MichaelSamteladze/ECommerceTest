var ProductModel = {
    AddProductInBasketUrl: null,
    TextBasketAddSuccess: null
}
$(function () {
    $('.product-count input').keydown(function (e) {
        // Allow: backspace, delete, tab, escape, enter and .
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
            // Allow: Ctrl+A
            (e.keyCode === 65 && e.ctrlKey === true) ||
            // Allow: Ctrl+C
            (e.keyCode === 67 && e.ctrlKey === true) ||
            // Allow: Ctrl+V
            (e.keyCode === 86 && e.ctrlKey === true) ||
            // Allow: Ctrl+X
            (e.keyCode === 88 && e.ctrlKey === true) ||
            // Allow: home, end, left, right
            (e.keyCode >= 35 && e.keyCode <= 39)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });

    $('.product-count input').keyup(function () {
        var value = $(this).val();
        if (value < 1) {
            value = 1;
        }
        if (value > 99) {
            value = 99;
        }
        $(this).val(value);
    });

    $('.product-count .controls button').click(function () {
        var container = $(this).closest('.product-count');
        var input = container.find('input');
        var range = 1;
        var oldValue = +input.val();
        var newValue;

        if ($(this).hasClass('minus')) {
            newValue = oldValue - range;
        }
        else {
            newValue = oldValue + range;
        }
        if (newValue < 1) {
            newValue = 1;
        }
        if (newValue > 99) {
            newValue = 99;
        }

        input.val(newValue);
    });

    $('.js-add-to-basket-button').click(function (e) {
        e.preventDefault();
        var ProductID = $(this).closest('.info').data('id');
        var ProductCount = $('.product-count input').val();

        $.ajax({
            type: 'POST',
            url: ProductModel.AddProductInBasketUrl,
            data: { ProductID: ProductID, ProductCount: ProductCount, ProductOptionID: ProductOptionID },
            success: function (res) {
                if (res.IsSuccess) {
                    $('.js-product-count-badge').html(res.Data.ProductCountInBasket);
                    $('.js-product-count-badge').Show();
                    $('.js-product-count-badge-container').Show();

                    $('[data-modal="alert"]').find('.text').html(ProductModel.TextBasketAddSuccess);
                    $('[data-modal="alert"]').modal('show');
                } else {
                    $('[data-modal="alert"]').find('.text').html(Globals.TextError);
                    $('[data-modal="alert"]').modal('show');
                }
            }
        });
    });    
})