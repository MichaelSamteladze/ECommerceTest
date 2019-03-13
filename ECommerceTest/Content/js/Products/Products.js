var ProductsModel = {
    AddProductInBasketUrl: null,
    TextBasketAddSuccess: null
};

$(function () {
    $('.js-add-to-basket-button').click(function () {
        var ProductID = $(this).closest('.product-list-item').data('id');
        var _this = $(this);
        $.ajax({
            type: 'POST',
            url: ProductsModel.AddProductInBasketUrl,
            data: { ProductID: ProductID },
            beforeSend: function () {
                _this.Disable();
                _this.find('.js-add-to-basket-loader').Show();
            },
            success: function (res) {
                if (res.IsSuccess) {                    
                    $('.js-product-count-badge').html(res.Data.ProductCountInBasket);
                    $('.js-product-count-badge').Show();
                    $('.js-product-count-badge-container').Show();
                    
                    $('[data-modal="alert"]').find('.text').html(ProductsModel.TextBasketAddSuccess);
                    $('[data-modal="alert"]').modal('show');
                } else {
                    $('[data-modal="alert"]').find('.text').html(Globals.TextError);
                    $('[data-modal="alert"]').modal('show');
                }
            },
            complete: function () {
                _this.Enable();
                _this.find('.js-add-to-basket-loader').Hide();
            }
        });
        return false;
    });
})